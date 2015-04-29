using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Runtime.Remoting.Lifetime;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Globalization;

using DAQ;
using DAQ.Environment;

using System.Diagnostics;

namespace NavAnalysis
{
    public class Controller : MarshalByRefObject, IAnalysis
    {
        #region Initialise things

        private static string
            motMasterDataPath = (string)Environs.FileSystem.Paths["MOTMasterDataPath"];

        AnalWindow window;

        // without this method, any remote connections to this object will time out after
        // five minutes of inactivity.
        // It just overrides the lifetime lease system completely.
        public override Object InitializeLifetimeService()
        {
            return null;
        }

        public void Start()
        {
            window = new AnalWindow();
            window.controller = this;

            Application.Run(window);
        }

        #endregion

        #region Deal with window commands

        public void LoadOldRun()
        {
            Stopwatch watch = new System.Diagnostics.Stopwatch();
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MM run|*.zip";
            dialog.Title = "Load Old Run";
            dialog.InitialDirectory = motMasterDataPath;
            dialog.ShowDialog();

            if (dialog.FileName != "") {
                
                string zipPath = dialog.FileName;
                string EID = Path.GetFileNameWithoutExtension(zipPath);
                string img0 = EID + "_0.png";
                string img1 = EID + "_1.png";
                
                watch.Start();
                ComputeAbsImageFromZip(zipPath, img0, img1);
                watch.Stop();

                window.WriteToConsole("Loaded Images from " + zipPath);
                window.WriteToConsole("In " + watch.ElapsedMilliseconds + "ms");

            }
        }


        #endregion

        #region abs Image Precessing

        public void ComputeAbsImageFromZip(string zipFile, string img0, string img1)
        {
            Image<Gray, ushort> foreground = new Image<Gray, ushort>(100, 100);
            Image<Gray, ushort> background = new Image<Gray, ushort>(100, 100);

            //Get stuff from a zip file
            var zipInputStream = new ZipInputStream(File.OpenRead(zipFile));
            ZipEntry entry;
            string tmpEntry = String.Empty;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(entry.Name);
                if(fileName == img0){
                    MemoryStream stream = new MemoryStream();
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = zipInputStream.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            stream.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreground = new  Image<Gray,ushort>(new Bitmap(stream));

                    stream.Close();

                }else if(fileName == img1){
                    MemoryStream stream = new MemoryStream();
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = zipInputStream.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            stream.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    background = new  Image<Gray,ushort> (new Bitmap(stream));

                    stream.Close();
                }
              
            }
            
            ComputeAbsImageFromImage(foreground, background);
         
            zipInputStream.Close();
        }

        public void ComputeAbsImageFromFile(string foregroundFilePath, string backgroundFilePath){

            Image<Gray, ushort> foreground = new Image<Gray, ushort>(foregroundFilePath);
            Image<Gray, ushort> background = new Image<Gray, ushort>(backgroundFilePath);

            ComputeAbsImageFromImage(foreground, background);
        }

        public void ComputeAbsImageFromImage(Image<Gray, ushort> foreground, Image<Gray, ushort> background)
        {
            Image<Gray,float> absImage = GenerateAbsImage(foreground, background);

            window.ShowImages(foreground, background, absImage);
         
        }

        public Image<Gray, float> GenerateAbsImage(Image<Gray, ushort> fg, Image<Gray, ushort> bg)
        {
           
            int width = fg.Width;
            int height = fg.Height;

            Image<Gray, float> ab1 = new Image<Gray, float>(width, height);
            Image<Gray, float> ab2 = new Image<Gray, float>(width, height);

            CvInvoke.cvDiv(fg+1, bg+1, ab1, 1);
            CvInvoke.cvLog(ab1, ab2);
            
            return ab2*100;
        }

        public ushort[,] GenerateAbsImageArrayFromArrays(ushort[,] fg, ushort[,] bg)
        {
            Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            int width = fg.GetLength(0);
            int height = fg.GetLength(1);

            float[,] absImage = new float[width, height];

            float max = -1000;
            float min = 1000;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double bkg = fg[x, y];
                    double abs = bg[x, y];

                    absImage[x, y] = (float)Math.Log((abs + 1) / (bkg + 1));

                    max = Math.Max(absImage[x, y], max);
                    min = Math.Min(absImage[x, y], min);
                }
            }


            ushort[,] returnArray = new ushort[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnArray[x, y] = (ushort)Math.Floor(65536 * (absImage[x, y] - min) / (max - min));
                }
            }

            watch.Stop();
            window.WriteToConsole("Array Processing completed in " + watch.ElapsedMilliseconds + "ms");

            return returnArray;
        }

        public List<Matrix<float>> SumPixels(Image<Gray, float> absImage, int xmin, int xmax, int ymin, int ymax)
        {
            //Get the ROI Image
            Rectangle roi = new Rectangle(xmin,ymin, xmax - xmin, ymax - ymin);
            CvInvoke.cvSetImageROI(absImage,roi);

            //Build Matricies to project onto
            Matrix<float> imgMatHorizontal = new Matrix<float>(absImage.Height, 1, 1);
            Matrix<float> imgMatVertical = new Matrix<float>(1, absImage.Width, 1);

            // Project
            absImage.Reduce<float>(imgMatHorizontal, 
                Emgu.CV.CvEnum.REDUCE_DIMENSION.SINGLE_COL, 
                Emgu.CV.CvEnum.REDUCE_TYPE.CV_REDUCE_SUM);

            absImage.Reduce<float>(imgMatVertical, 
                Emgu.CV.CvEnum.REDUCE_DIMENSION.SINGLE_ROW, 
                Emgu.CV.CvEnum.REDUCE_TYPE.CV_REDUCE_SUM);

            //Return image ROI to original size before continuing
            CvInvoke.cvResetImageROI(absImage);

            return new List<Matrix<float>> { imgMatVertical, imgMatHorizontal };

            /*
            float[] horizdata = new float[xmax - xmin];
            float[] vertdata = new float[ymax - ymin];

            //Rectangle roi = new Rectangle(xmin,xmax,ymin,ymax);
            //CvInvoke.cvSetImageROI(absImage,roi);

            //Image<Gray,float> absRegion = new CvInvoke.cvGetRectSubPix(absImage);
            //CvInvoke.cvSum(
            //int[] horizsum = new int[xmax-xmin];
            //int[] vertsum = new int[ymax-ymin];

            //for (int x = xmin; x < xmax; x++)
            //{
            //    horizsum[x]+=1;
            //}

            //for (int y = ymin; y < ymax; y++)
            //{
            //    vertsum[y]+=1;
            //}

           List <float[]>sumdata = new List<float[]>(); 
            

            for (int x = xmin; x < xmax; x++)
            {
                for (int y = ymin; y < ymax; y++)
                {
                    horizdata[x-xmin] += (float)absImage[x,y].Intensity;
                    vertdata[y-ymin] += (float)absImage[x,y].Intensity;
                }
            }

            sumdata.Add(horizdata);
            sumdata.Add(vertdata);
            return sumdata;
             */
        }
    
        #endregion

        #region boring image stuff

        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        private Bitmap BitmapFromArray (ushort[,] img)
        {
            int width = img.GetLength(0);
            int height = img.GetLength(1);

            ushort[] returnArray = new ushort[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnArray[y * width + x] = img[x,y];
                }
            }

            BitmapPalette myPalette = BitmapPalettes.Gray256Transparent;

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed8,
                myPalette,
                returnArray,
                width);

            return BitmapFromSource(image);
        }

        private Image<Gray, ushort> ImageFromArray(ushort[,] img)
        {
            return new Image<Gray, ushort>(BitmapFromArray(img));
        }

        #endregion

        #region running the anaysis
        public Dictionary<string, object> GetReport(ushort[][,] imageData)
        {
            Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            ushort[,] absImage = GenerateAbsImageArrayFromArrays(imageData[0], imageData[1]);
            watch.Stop();
            window.WriteToConsole("Images Analysed in " + watch.ElapsedMilliseconds + "ms");
            watch.Reset();
            watch.Start();

            //window.ShowImages(ImageFromArray(imageData[0]),
            //    ImageFromArray(imageData[1]),
            //    ImageFromArray(absImage));
            watch.Stop();
            window.WriteToConsole("Images Displayed in " + watch.ElapsedMilliseconds + "ms");

            Dictionary<string,object> returnReport = new Dictionary<string,object>();

            returnReport.Add("absImage", absImage);

            return returnReport;
        }
        #endregion

        #region Python Stuff

        public void ComputeAbsImage(string path, string EID)
        {
            string img0 = EID + "_0.png";
            string img1 = EID + "_1.png";

            ComputeAbsImageFromZip(path, img0, img1);
        }

        public void CloseIt()
        {
            window.Close();
        }

        #endregion
    }
}

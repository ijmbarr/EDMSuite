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


namespace NavAnalysis
{
    public class Controller : MarshalByRefObject, IAnalysis
    {
        #region Initialise things

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

        #region abs Image Precessing

        public void ComputeAbsImageFromZip(string zipFile, string img0, string img1)
        {
            Image<Gray, Byte> foreground = new Image<Gray, Byte>(1, 1);
            Image<Gray, Byte> background = new Image<Gray, Byte>(1, 1);

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

                    foreground = new  Image<Gray,Byte>(new Bitmap(stream));

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

                    background = new  Image<Gray,Byte> (new Bitmap(stream));

                    stream.Close();
                }
              
            }
         ComputeAbsImageFromImage(foreground, background);
         
            zipInputStream.Close();

           
        }

        public void ComputeAbsImageFromFile(string foregroundFilePath, string backgroundFilePath){

            Image<Gray, Byte> foreground = new Image<Gray, Byte>(foregroundFilePath);
            Image<Gray, Byte> background = new Image<Gray, Byte>(backgroundFilePath);

            ComputeAbsImageFromImage(foreground, background);
        }

        public void ComputeAbsImageFromImage(Image<Gray, Byte> foreground, Image<Gray, Byte> background)
        {
            Image<Gray,Byte> absImage = GenerateAbsImage(foreground, background);

            window.ShowImages(foreground, background, absImage);
         
        }

        public Image<Gray, Byte> GenerateAbsImage(Image<Gray, Byte> fg, Image<Gray, Byte> bg)
        {
           
            int width = fg.Width;
            int height = fg.Height;

            float[,] absImage = new float[width,height];

            float max = -1000;
            float min = 1000;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double bkg = fg[y,x].Intensity;
                    double abs = bg[y,x].Intensity;

                    absImage[x, y] = (float)Math.Log((abs + 1) / (bkg + 1));

                    max = Math.Max(absImage[x, y], max);
                    min = Math.Min(absImage[x, y], min);
                }
            }
         

            byte[] returnArray = new byte[width*height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnArray[y*width + x] = (byte)Math.Floor(256 * (absImage[x,y] - min) / (max - min));
                }
            }


            // Define the image palette
            BitmapPalette myPalette = BitmapPalettes.Gray256Transparent;

            // Creates a new empty image with the pre-defined palette

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed8,
                myPalette,
                returnArray,
                width);



            Image<Gray, Byte> returnImage = new Image<Gray, Byte>(BitmapFromSource(image));

           
            return returnImage;

        }

        public byte[,] GenerateAbsImageByteArrayFromByteArrays(byte[,] fg, byte[,] bg)
        {

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


            byte[,] returnArray = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnArray[x, y] = (byte)Math.Floor(256 * (absImage[x, y] - min) / (max - min));
                }
            }

            return returnArray;
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

        private Bitmap BitmapFromByteArray (byte[,] img)
        {
            int width = img.GetLength(0);
            int height = img.GetLength(1);

            byte[] returnArray = new byte[width * height];

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

        private Image<Gray, Byte> ImageFromByteArray(byte[,] img)
        {
            return new Image<Gray, Byte>(BitmapFromByteArray(img));
        }

        #endregion

        #region running the anaysis
        public Dictionary<string, object> GetReport(Byte[][,] imageData)
        {

            byte[,] absImage = GenerateAbsImageByteArrayFromByteArrays(imageData[0], imageData[1]);

            window.ShowImages(ImageFromByteArray(imageData[0]),
                ImageFromByteArray(imageData[1]),
                ImageFromByteArray(absImage));

            Dictionary<string,object> returnReport = new Dictionary<string,object>();

            returnReport.Add("absImage", absImage);

            return returnReport;
        }
        #endregion
    }
}

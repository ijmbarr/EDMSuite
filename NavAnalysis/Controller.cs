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

namespace NavAnalysis
{
    public class Controller : MarshalByRefObject, IAnalysis
    {
        AnalWindow window;

        public void Start()
        {
            window = new AnalWindow();
            window.controller = this;

            Application.Run(window);
        }

        public void ComputeAbsImageFromZip(string zipFile, string img0, string img1)
        {
            Bitmap foreground = new Bitmap(1,1);
            Bitmap background = new Bitmap(1,1);

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

                    foreground = new Bitmap(stream);

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

                    background = new Bitmap(stream);

                    stream.Close();
                }
              
            }
            zipInputStream.Close();

            ComputeAbsImageFromImage(foreground, background);
        }

        public void ComputeAbsImageFromFile(string foregroundFilePath, string backgroundFilePath){
            Bitmap foreground = new Bitmap(foregroundFilePath);
            Bitmap background = new Bitmap(backgroundFilePath);

            ComputeAbsImageFromImage(foreground, background);
        }

        public void ComputeAbsImageFromImage(Bitmap foreground, Bitmap background)
        {
            Bitmap absImage = GenerateAbsImage(foreground, background);

            window.ShowImages(foreground, background, absImage);
        }

        public Bitmap GenerateAbsImage(Bitmap fg, Bitmap bg)
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
                    double bkg = fg.GetPixel(x,y).GetBrightness();
                    double abs = bg.GetPixel(x,y).GetBrightness();

                    absImage[x,y] = (float)Math.Log((abs + 1) / (bkg + 1));

                    max = Math.Max(absImage[x, y], max);
                    min = Math.Min(absImage[x, y], min);
                }
            }

            byte[,] returnArray = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnArray[x,y] = (byte)Math.Floor(256 * (absImage[x,y] - min) / (max - min));
                }
            }

            Bitmap returnImage = Convert2Bitmap(returnArray);

            return returnImage;

        }

        public Bitmap Convert2Bitmap(byte[,] DATA)
        {
            int width = DATA.GetLength(0);
            int height = DATA.GetLength(1);
            Bitmap Bm = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var b = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int Value = DATA[x,y];
                    Color C = ncp.Entries[Value];
                    Bm.SetPixel(x, y, C);
                }
            }
            return Bm;
        }


    }
}

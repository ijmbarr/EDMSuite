using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;

namespace NavAnalysis
{
    public partial class AnalWindow : Form
    {
        //Hackish ROI stuff
        bool ROIselecting = false;
        private int x0, y0, x1, y1;
        private int xmin, xmax, ymin, ymax;
        int width, height;
        Bitmap ROIbkg;

        Image<Gray, float> absImage;

        public Controller controller;

        public AnalWindow()
        {
            InitializeComponent();

            //ROI box
            pictureBox3.Controls.Add(ROIbox);
            ROIbox.Location = new Point(0, 0);
            ROIbox.BackColor = Color.Transparent;

        }

        #region Console

        public void WriteToConsole(string text)
        {
            setRichTextBox(windowConsole, ">> " + text + "\n");

        }

        private void setRichTextBox(RichTextBox box, string text)
        {
            box.Invoke(new setRichTextDelegate(setRichTextHelper), new object[] { box, text });
        }

        private delegate void setRichTextDelegate(RichTextBox box, string text);

        private void setRichTextHelper(RichTextBox box, string text)
        {
            box.AppendText(text);
            box.ScrollToCaret();
        }

        #endregion

        #region Image Boxes
        public void ShowImages(Image<Gray, ushort> one, Image<Gray, ushort> two, Image<Gray, float> three)
        {
            pictureBox1.Image = one;
            pictureBox2.Image = two;
            pictureBox3.Image = three;

            //Post Processing
            absImage = three;
            width = absImage.Width;
            height = absImage.Height;
            ROIbkg = new Bitmap(width, height);
        }

        public void ShowOne(Image<Gray, ushort> one)
        {
            pictureBox1.Image = one;
        }

        public void ShowTwo(Image<Gray, ushort> two)
        {
            pictureBox2.Image = two;
        }

        public void ShowThree(Image<Gray, ushort> three)
        {
            pictureBox2.Image = three;
        }
        #endregion

        #region buttons

        private void LoadOldRun_Click(object sender, EventArgs e)
        {
            controller.LoadOldRun();
        }

        #endregion

        #region draw roi

        public void ROIbox_MouseDown(object sender, MouseEventArgs e)
        {
                ROIselecting = true;
                x0 = e.X;
                y0 = e.Y;
        }

        public void ROIbox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ROIselecting)
            {
                return;
            }

            x1 = e.X;
            y1 = e.Y;

            // Draw the rectangle.
            using (Graphics gr = Graphics.FromImage(ROIbkg))
            {
                gr.Clear(Color.Transparent);

                gr.DrawRectangle(Pens.Red,
                    Math.Min(x0, x1), Math.Min(y0, y1),
                    Math.Abs(x0 - x1), Math.Abs(y0 - y1));
            }

            ROIbox.Image = ROIbkg;

        }

        public void ROIbox_MouseUp(object sender, MouseEventArgs e)
        {
          
            //ToDo: make it so the values are dynamic. e.g. what happens if I rescale the picture box?
            ROIselecting = false;
            xmin = Math.Min(x0, x1) * 2452 / 330;
            xmax = Math.Max(x0, x1) * 2452 / 330;
            ymin = Math.Min(y0, y1) * 2054 / 330;
            ymax = Math.Max(y0, y1) * 2054 / 330;
            WriteToConsole("x min = " +xmin + ", x max = " +xmax + ", y min = " + ymin + ", y max = " + ymax);
     
        }



        #endregion

        public void PlotPixelDist(Image<Gray, float> absImage)
        {
            string htitle = "hTitle";
            string vtitle = "vTitle";
            if (horizChart.Series.IndexOf("Horizontal") != -1)
            {
                horizChart.Series.Clear();
                WriteToConsole("Analysing New Region");
            }
            
            horizChart.Series.Add("Horizontal");
          //  horizChart.Titles.Add("Horiztonal");
        

            if (vertChart.Series.IndexOf("Vertical") != -1)
            {
                vertChart.Series.Clear();
              
            }
            vertChart.Series.Add("Vertical");
          //  vertChart.Titles.Add("Vertical");
          
            //This doesn't yet work. Why?
            if (xmin <= 0 && ymin <= 0)
            {
                xmin = 0;
                ymin = 0;
                xmax = absImage.Width - 1;
                ymax = absImage.Height - 1;
            }

            List<Matrix<float>> pixelData = controller.SumPixels(absImage, xmin, xmax, ymin, ymax);
          

            for (int hval = 0; hval < xmax - xmin; hval++)
            {
                horizChart.Series["Horizontal"].Points.AddXY(hval, pixelData[0][0, hval]);
               
            }

            for (int vval = 0; vval < ymax - ymin; vval++)
            {
                vertChart.Series["Vertical"].Points.AddXY(vval, pixelData[1][vval, 0]);
            }  


        }
        private void plotButton_Click(object sender, EventArgs e)
        {
            PlotPixelDist(absImage);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;

namespace NavAnalysis
{
    public partial class AnalWindow : Form
    {
        public Controller controller;

        public AnalWindow()
        {
            InitializeComponent();
        }

        public void ShowImages(Image<Gray, Byte> one, Image<Gray, Byte> two, Image<Gray, Byte> three)
        {
            pictureBox1.Image = one;
            pictureBox2.Image = two;
            pictureBox3.Image = three;
        }

        public void ShowOne(Image<Gray, Byte> one)
        {
            pictureBox1.Image = one;
        }

        public void ShowTwo(Image<Gray, Byte> two)
        {
            pictureBox2.Image = two;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NavAnalysis
{
    public partial class AnalWindow : Form
    {
        public Controller controller;

        public AnalWindow()
        {
            InitializeComponent();
        }

        public void ShowImages(Bitmap one, Bitmap two, Bitmap three)
        {
            pictureBox1.Image = one;
            pictureBox2.Image = two;
            pictureBox3.Image = three;
        }

    }
}

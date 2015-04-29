namespace NavAnalysis
{
    partial class AnalWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new Emgu.CV.UI.ImageBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new Emgu.CV.UI.ImageBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ROIbox = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new Emgu.CV.UI.ImageBox();
            this.LoadOldRun = new System.Windows.Forms.Button();
            this.windowConsole = new System.Windows.Forms.RichTextBox();
            this.horizChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.vertChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.plotButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ROIbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertChart)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 36);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(333, 349);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(325, 323);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Foreground";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(330, 330);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pictureBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(325, 323);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Background";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.pictureBox2.Location = new System.Drawing.Point(6, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(330, 330);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ROIbox);
            this.tabPage3.Controls.Add(this.pictureBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(325, 323);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Absorption";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ROIbox
            // 
            this.ROIbox.Location = new System.Drawing.Point(0, 0);
            this.ROIbox.Name = "ROIbox";
            this.ROIbox.Size = new System.Drawing.Size(330, 330);
            this.ROIbox.TabIndex = 2;
            this.ROIbox.TabStop = false;
            this.ROIbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ROIbox_MouseDown);
            this.ROIbox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ROIbox_MouseMove);
            this.ROIbox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ROIbox_MouseUp);
            // 
            // pictureBox3
            // 
            this.pictureBox3.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.pictureBox3.Location = new System.Drawing.Point(6, 6);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(313, 311);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // LoadOldRun
            // 
            this.LoadOldRun.Location = new System.Drawing.Point(413, 58);
            this.LoadOldRun.Name = "LoadOldRun";
            this.LoadOldRun.Size = new System.Drawing.Size(128, 23);
            this.LoadOldRun.TabIndex = 0;
            this.LoadOldRun.Text = "Load Old Run";
            this.LoadOldRun.UseVisualStyleBackColor = true;
            this.LoadOldRun.Click += new System.EventHandler(this.LoadOldRun_Click);
            // 
            // windowConsole
            // 
            this.windowConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.windowConsole.ForeColor = System.Drawing.Color.Red;
            this.windowConsole.Location = new System.Drawing.Point(1, 405);
            this.windowConsole.Name = "windowConsole";
            this.windowConsole.Size = new System.Drawing.Size(742, 193);
            this.windowConsole.TabIndex = 1;
            this.windowConsole.Text = "";
            // 
            // horizChart
            // 
            chartArea1.Name = "ChartArea1";
            this.horizChart.ChartAreas.Add(chartArea1);
            this.horizChart.Location = new System.Drawing.Point(413, 101);
            this.horizChart.Name = "horizChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Series1";
            series1.Points.Add(dataPoint1);
            this.horizChart.Series.Add(series1);
            this.horizChart.Size = new System.Drawing.Size(320, 134);
            this.horizChart.TabIndex = 2;
            this.horizChart.Text = "Horizontal";
            title1.Name = "Horizontal";
            this.horizChart.Titles.Add(title1);
            // 
            // vertChart
            // 
            chartArea2.Name = "ChartArea1";
            this.vertChart.ChartAreas.Add(chartArea2);
            this.vertChart.Location = new System.Drawing.Point(414, 241);
            this.vertChart.Name = "vertChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "Series1";
            series2.Points.Add(dataPoint2);
            this.vertChart.Series.Add(series2);
            this.vertChart.Size = new System.Drawing.Size(319, 144);
            this.vertChart.TabIndex = 3;
            this.vertChart.Text = "chart2";
            title2.Name = "Vertical";
            this.vertChart.Titles.Add(title2);
            // 
            // plotButton
            // 
            this.plotButton.Location = new System.Drawing.Point(570, 58);
            this.plotButton.Name = "plotButton";
            this.plotButton.Size = new System.Drawing.Size(128, 23);
            this.plotButton.TabIndex = 4;
            this.plotButton.Text = "Plot Selected Region";
            this.plotButton.UseVisualStyleBackColor = true;
            this.plotButton.Click += new System.EventHandler(this.plotButton_Click);
            // 
            // AnalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 598);
            this.Controls.Add(this.plotButton);
            this.Controls.Add(this.vertChart);
            this.Controls.Add(this.horizChart);
            this.Controls.Add(this.windowConsole);
            this.Controls.Add(this.LoadOldRun);
            this.Controls.Add(this.tabControl1);
            this.Name = "AnalWindow";
            this.Text = "Navigator Analysis";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ROIbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Emgu.CV.UI.ImageBox pictureBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private Emgu.CV.UI.ImageBox pictureBox2;
        private System.Windows.Forms.TabPage tabPage3;
        private Emgu.CV.UI.ImageBox pictureBox3;
        private System.Windows.Forms.Button LoadOldRun;
        private System.Windows.Forms.RichTextBox windowConsole;
        private System.Windows.Forms.PictureBox ROIbox;
        private System.Windows.Forms.DataVisualization.Charting.Chart horizChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart vertChart;
        private System.Windows.Forms.Button plotButton;
    }
}


using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;

namespace NavHardwareControl
{
    /// <summary>
    /// UPDATE FOR NAV CONTROLLER:
    /// This is more or less a direct copy of SHC, addapted to our needs. 
    /// 
    /// Front panel for the sympathetic hardware controller. Everything is just stuffed in there. No particularly
    /// clever structure. This class just hands everything straight off to the controller. It has a few
    /// thread safe wrappers so that remote calls can safely manipulate the front panel.
    /// </summary>
    public class ControlWindow : System.Windows.Forms.Form
    {
        #region Setup

        public Controller controller;
        private Dictionary<string, TextBox> AOTextBoxes = new Dictionary<string, TextBox>();
        private Dictionary<string, CheckBox> DOCheckBoxes = new Dictionary<string, CheckBox>();
            

        public ControlWindow()
        {
            InitializeComponent();
            AOTextBoxes["testAnalogChannel"] = testAnalogLine;

            DOCheckBoxes["testDigitalChannel"] = testChannel;

        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            controller.ControllerStopping();
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            controller.ControllerLoaded();
            
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.shcTabs = new System.Windows.Forms.TabControl();
            this.tabCamera = new System.Windows.Forms.TabPage();
            this.stopStreamButton = new System.Windows.Forms.Button();
            this.streamButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.testChannel = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.testAnalogLine = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.updateHardwareButton = new System.Windows.Forms.Button();
            this.consoleRichTextBox = new System.Windows.Forms.RichTextBox();
            this.shcTabs.SuspendLayout();
            this.tabCamera.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // shcTabs
            // 
            this.shcTabs.AllowDrop = true;
            this.shcTabs.Controls.Add(this.tabCamera);
            this.shcTabs.Controls.Add(this.tabPage1);
            this.shcTabs.Controls.Add(this.tabPage2);
            this.shcTabs.Location = new System.Drawing.Point(3, 27);
            this.shcTabs.Name = "shcTabs";
            this.shcTabs.SelectedIndex = 0;
            this.shcTabs.Size = new System.Drawing.Size(666, 235);
            this.shcTabs.TabIndex = 0;
            // 
            // tabCamera
            // 
            this.tabCamera.Controls.Add(this.stopStreamButton);
            this.tabCamera.Controls.Add(this.streamButton);
            this.tabCamera.Controls.Add(this.snapshotButton);
            this.tabCamera.Location = new System.Drawing.Point(4, 22);
            this.tabCamera.Name = "tabCamera";
            this.tabCamera.Padding = new System.Windows.Forms.Padding(3);
            this.tabCamera.Size = new System.Drawing.Size(658, 209);
            this.tabCamera.TabIndex = 0;
            this.tabCamera.Text = "Camera Control";
            this.tabCamera.UseVisualStyleBackColor = true;
            // 
            // stopStreamButton
            // 
            this.stopStreamButton.Location = new System.Drawing.Point(168, 6);
            this.stopStreamButton.Name = "stopStreamButton";
            this.stopStreamButton.Size = new System.Drawing.Size(75, 23);
            this.stopStreamButton.TabIndex = 18;
            this.stopStreamButton.Text = "Stop";
            this.stopStreamButton.UseVisualStyleBackColor = true;
            this.stopStreamButton.Click += new System.EventHandler(this.stopStreamButton_Click);
            // 
            // streamButton
            // 
            this.streamButton.Location = new System.Drawing.Point(87, 6);
            this.streamButton.Name = "streamButton";
            this.streamButton.Size = new System.Drawing.Size(75, 23);
            this.streamButton.TabIndex = 17;
            this.streamButton.Text = "Stream";
            this.streamButton.UseVisualStyleBackColor = true;
            this.streamButton.Click += new System.EventHandler(this.streamButton_Click);
            // 
            // snapshotButton
            // 
            this.snapshotButton.Location = new System.Drawing.Point(6, 6);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(75, 23);
            this.snapshotButton.TabIndex = 15;
            this.snapshotButton.Text = "Snapshot";
            this.snapshotButton.UseVisualStyleBackColor = true;
            this.snapshotButton.Click += new System.EventHandler(this.snapshotButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.testChannel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(658, 209);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Digital Line";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // testChannel
            // 
            this.testChannel.AutoSize = true;
            this.testChannel.Location = new System.Drawing.Point(19, 18);
            this.testChannel.Name = "testChannel";
            this.testChannel.Size = new System.Drawing.Size(89, 17);
            this.testChannel.TabIndex = 0;
            this.testChannel.Text = "Test Channel";
            this.testChannel.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.testAnalogLine);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(658, 209);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Analog Lines";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Test Analog Line";
            // 
            // testAnalogLine
            // 
            this.testAnalogLine.Location = new System.Drawing.Point(99, 15);
            this.testAnalogLine.Name = "testAnalogLine";
            this.testAnalogLine.Size = new System.Drawing.Size(100, 20);
            this.testAnalogLine.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 24);
            this.checkBox1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(794, 24);
            this.menuStrip.TabIndex = 15;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadParametersToolStripMenuItem,
            this.saveParametersToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveImageToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadParametersToolStripMenuItem
            // 
            this.loadParametersToolStripMenuItem.Name = "loadParametersToolStripMenuItem";
            this.loadParametersToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.loadParametersToolStripMenuItem.Text = "Load parameters";
            this.loadParametersToolStripMenuItem.Click += new System.EventHandler(this.loadParametersToolStripMenuItem_Click);
            // 
            // saveParametersToolStripMenuItem
            // 
            this.saveParametersToolStripMenuItem.Name = "saveParametersToolStripMenuItem";
            this.saveParametersToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveParametersToolStripMenuItem.Text = "Save parameters on UI";
            this.saveParametersToolStripMenuItem.Click += new System.EventHandler(this.saveParametersToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveImageToolStripMenuItem.Text = "Save image";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageViewerToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // openImageViewerToolStripMenuItem
            // 
            this.openImageViewerToolStripMenuItem.Name = "openImageViewerToolStripMenuItem";
            this.openImageViewerToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.openImageViewerToolStripMenuItem.Text = "Start camera and open image viewer";
            this.openImageViewerToolStripMenuItem.Click += new System.EventHandler(this.openImageViewerToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(675, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Remote Control";
            // 
            // updateHardwareButton
            // 
            this.updateHardwareButton.Location = new System.Drawing.Point(678, 62);
            this.updateHardwareButton.Name = "updateHardwareButton";
            this.updateHardwareButton.Size = new System.Drawing.Size(102, 23);
            this.updateHardwareButton.TabIndex = 21;
            this.updateHardwareButton.Text = "Update hardware";
            this.updateHardwareButton.UseVisualStyleBackColor = true;
            this.updateHardwareButton.Click += new System.EventHandler(this.updateHardwareButton_Click);
            // 
            // consoleRichTextBox
            // 
            this.consoleRichTextBox.BackColor = System.Drawing.Color.Black;
            this.consoleRichTextBox.ForeColor = System.Drawing.Color.Lime;
            this.consoleRichTextBox.Location = new System.Drawing.Point(3, 264);
            this.consoleRichTextBox.Name = "consoleRichTextBox";
            this.consoleRichTextBox.ReadOnly = true;
            this.consoleRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.consoleRichTextBox.Size = new System.Drawing.Size(791, 154);
            this.consoleRichTextBox.TabIndex = 23;
            this.consoleRichTextBox.Text = "";
            // 
            // ControlWindow
            // 
            this.ClientSize = new System.Drawing.Size(794, 419);
            this.Controls.Add(this.consoleRichTextBox);
            this.Controls.Add(this.updateHardwareButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shcTabs);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "ControlWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Navigator Hardware Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WindowClosing);
            this.Load += new System.EventHandler(this.WindowLoaded);
            this.shcTabs.ResumeLayout(false);
            this.tabCamera.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region ThreadSafe wrappers

        private void setCheckBox(CheckBox box, bool state)
        {
            box.Invoke(new setCheckDelegate(setCheckHelper), new object[] { box, state });
        }
        private delegate void setCheckDelegate(CheckBox box, bool state);
        private void setCheckHelper(CheckBox box, bool state)
        {
            box.Checked = state;
        }

        private void setTabEnable(TabControl box, bool state)
        {
            box.Invoke(new setTabEnableDelegate(setTabEnableHelper), new object[] { box, state });
        }
        private delegate void setTabEnableDelegate(TabControl box, bool state);
        private void setTabEnableHelper(TabControl box, bool state)
        {
            box.Enabled = state;
        }

        private void setTextBox(TextBox box, string text)
        {
            box.Invoke(new setTextDelegate(setTextHelper), new object[] { box, text });
        }
        private delegate void setTextDelegate(TextBox box, string text);
        private void setTextHelper(TextBox box, string text)
        {
            box.Text = text;
        }
        
        private void setRichTextBox(RichTextBox box, string text)
        {
            box.Invoke(new setRichTextDelegate(setRichTextHelper), new object[] { box, text });
        }
        private delegate void setRichTextDelegate(RichTextBox box, string text);
        private void setRichTextHelper(RichTextBox box, string text)
        {
            box.AppendText(text);
            consoleRichTextBox.ScrollToCaret();
        }

        #endregion

        #region Declarations
        //Declare stuff here
        public TabControl shcTabs;
        public TabPage tabCamera;
        private Button button1;
        public CheckBox checkBox1;
        public TextBox textBox1;
        public TextBox textBox2;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadParametersToolStripMenuItem;
        private ToolStripMenuItem saveParametersToolStripMenuItem;
        private ToolStripMenuItem saveImageToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private Button snapshotButton;
        private Label label1;
        private Button updateHardwareButton;
        private ToolStripMenuItem windowsToolStripMenuItem;
        private RichTextBox consoleRichTextBox;
        private ToolStripMenuItem openImageViewerToolStripMenuItem;
        private Button streamButton;
        private Button stopStreamButton;

        private TabPage tabPage1;
        private CheckBox testChannel;
        private TabPage tabPage2;
        private Label label2;
        private TextBox testAnalogLine;

        #endregion

        #region Click Handlers

        private void saveParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.SaveParametersWithDialog();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.SaveImageWithDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void updateHardwareButton_Click(object sender, EventArgs e)
        {
            controller.UpdateHardware();
        }
        private void loadParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.LoadParametersWithDialog();
        }

        #endregion

        #region Public properties for controlling UI.
        //This gets/sets the values on the GUI panel
        public void WriteToConsole(string text)
        {
            setRichTextBox(consoleRichTextBox, ">> " + text + "\n");
            
           
        }
        public double ReadAnalog(string channelName)
        {
            return double.Parse(AOTextBoxes[channelName].Text);
        }
        public void SetAnalog(string channelName, double value)
        {
            setTextBox(AOTextBoxes[channelName], Convert.ToString(value));
        }
        public bool ReadDigital(string channelName)
        {
            return DOCheckBoxes[channelName].Checked;
        }
        public void SetDigital(string channelName, bool value)
        {
            setCheckBox(DOCheckBoxes[channelName], value);
        }
        #endregion

        #region Camera Control

        private void snapshotButton_Click(object sender, EventArgs e)
        {
            controller.CameraSnapshot();         
        }
        private void streamButton_Click(object sender, EventArgs e)
        {
            controller.CameraStream();
        }
        private void stopStreamButton_Click(object sender, EventArgs e)
        {
            controller.StopCameraStream();
        }

        #endregion

        #region UI state
        
        public void UpdateUIState(Controller.HCUIControlState state)
        {
            switch (state)
            {
                case Controller.HCUIControlState.OFF:

                    setTabEnable(shcTabs, true);

                    break;

                case Controller.HCUIControlState.LOCAL:

                    setTabEnable(shcTabs, true);
                    break;

                case Controller.HCUIControlState.REMOTE:

                    setTabEnable(shcTabs, false) ;

                    break;
            }
        }

       
        #endregion

        #region Other Windows


        private void openImageViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.StartCameraControl();
        }
        #endregion




        

      

        

    



















    }
}

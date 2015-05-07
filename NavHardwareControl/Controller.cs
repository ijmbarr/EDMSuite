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
using System.Linq;

using NationalInstruments;
using NationalInstruments.DAQmx;
using NationalInstruments.VisaNS;

using DAQ;
using DAQ.HAL;
using DAQ.Environment;
using DAQ.TransferCavityLock;

using IMAQ;
using NavAnalysis;

using Newtonsoft.Json;


namespace NavHardwareControl
{
    /// <summary>
    /// This is the interface to the sympathetic specific hardware.
    /// 
    /// Flow chart (under normal conditions): UI -> controller -> hardware. Basically, that's it.
    /// Exceptions: 
    /// -controller can set values displayed on UI (only meant for special cases, like startup or re-loading a saved parameter set)
    /// -Some public functions allow the HC to be controlled remotely (see below).
    /// 
    /// HardwareState:
    /// There are 3 states which the controller can be in: OFF, LOCAL and REMOTE.
    /// OFF just means the HC is idle.
    /// The state is set to LOCAL when this program is actively upating the state of the hardware. It does this by
    /// reading off what's on the UI, finding any discrepancies between the current state of the hardware and the values on the UI
    /// and by updating the hardware accordingly.
    /// After finishing with the update, it resets the state to OFF.
    /// When the state is set to REMOTE, the UI is disactivated. The hardware controller saves all the parameter values upon switching from
    /// LOCAL to REMOTE, then does nothing. When switching back, it reinstates the hardware state to what it was before it switched to REMOTE.
    /// Use this when you want to control the hardware from somewhere else (e.g. MOTMaster).
    ///
    /// Remoting functions (SetValue):
    /// Having said that, you'll notice that there are also public functions for modifying parameter values without putting the HC in REMOTE.
    /// I wrote it like this because you want to be able to do two different things:
    /// -Have the hardware controller take a back seat and let something else control the hardware for a while (e.g. MOTMaster)
    /// This is when you should use the REMOTE state.
    /// -You still want the HC to keep track of the hardware (hence remaining in LOCAL), but you want to send commands to it remotely 
    /// (say from a console) instead of from the UI. This is when you would use the SetValue functions.
    /// 
    /// The Hardware Report:
    /// The Hardware report is a way of passing a dictionary (gauges, temperature measurements, error signals) to another program
    /// (MotMaster, say). MOTMaster can then save the dictionary along with the data. I hope this will help towards answering questions
    /// like: "what was the source chamber pressure when we took this data?". At the moment, the hardware state is also included in the report.
    /// 
    /// </summary>
    public class Controller : MarshalByRefObject, CameraControllable, ExperimentReportable, IHardwareRelease
    {
        #region Constants
        //Put any constants and stuff here

        private static string cameraAttributesPath = (string)Environs.FileSystem.Paths["CameraAttributesPath"];
        private static string profilesPath = (string)Environs.FileSystem.Paths["settingsPath"] + "\\NavigatorHardwareController\\";

        private static Hashtable calibrations = Environs.Hardware.Calibrations;
        #endregion

        #region Setup



        // table of all digital analogTasks
        Hashtable digitalTasks = new Hashtable();
        HSDIOStaticChannelController HSchannels;

        //Cameras
        public CameraController ImageController;
        public bool cameraLoaded = false;
        
        // Declare that there will be a controlWindow
        ControlWindow controlWindow;

        //Add the window for analysis
        AnalWindow analWindow;

        //private bool sHCUIControl;
        public enum HCUIControlState { OFF, LOCAL, REMOTE };
        public HCUIControlState HCState = new HCUIControlState();

        private class cameraNotFoundException : ArgumentException { };

        HardwareState stateRecord;

        private Dictionary<string, Task> analogTasks;

        // without this method, any remote connections to this object will time out after
        // five minutes of inactivity.
        // It just overrides the lifetime lease system completely.
        public override Object InitializeLifetimeService()
        {
            return null;
        }


        public void Start()
        {

            // make the digital analogTasks. The function "CreateDigitalTask" is defined later
            //e.g   CreateDigitalTask("notEOnOff");
            //      CreateDigitalTask("eOnOff");

            //This is to keep track of the various things which the HC controls.
            analogTasks = new Dictionary<string, Task>();
            var temp = Environs.Info["HSDIOBoard"];
            HSchannels = new HSDIOStaticChannelController((string)Environs.Hardware.GetInfo("HSDIOBoard"), "0-31");
            stateRecord = new HardwareState();

            CreateHSDigitalTask("do00");
            CreateHSDigitalTask("do01");

            CreateDigitalTask("testDigitalChannel");


            // make the analog output analogTasks. The function "CreateAnalogOutputTask" is defined later
            //e.g.  bBoxAnalogOutputTask = CreateAnalogOutputTask("b");
            //      steppingBBiasAnalogOutputTask = CreateAnalogOutputTask("steppingBBias");

            CreateAnalogOutputTask("motCoil");
            CreateAnalogOutputTask("aom1freq");
            CreateAnalogOutputTask("motShutter");
            CreateAnalogOutputTask("imagingShutter");
            CreateAnalogOutputTask("rfSwitch");
            CreateAnalogOutputTask("rfAtten");
         
            //CreateAnalogInputTask("testInput", -10, 10);


            // make the control controlWindow
            controlWindow = new ControlWindow();
            controlWindow.controller = this;
            
            
        
            HCState = HCUIControlState.OFF;

             Application.Run(controlWindow);

        }

        // this method runs immediately after the GUI sets up
        internal void ControllerLoaded()
        {
            HardwareState loadedState = loadParameters(profilesPath + "StoppedParameters.json");

            if (!loadedState.Equals(stateRecord))
            {
                foreach (KeyValuePair<string, double> pair in loadedState.analogs)
                {
                    if (stateRecord.analogs.ContainsKey(pair.Key))
                    {
                        stateRecord.analogs[pair.Key] = pair.Value;
                    }
                }
                foreach (KeyValuePair<string, bool> pair in loadedState.digitals)
                {
                    if (stateRecord.digitals.ContainsKey(pair.Key))
                    {
                        stateRecord.digitals[pair.Key] = pair.Value;
                    }
                }
            }
         
            setValuesDisplayedOnUI(stateRecord);
            ApplyRecordedStateToHardware();
        }

        public void ControllerStopping()
        {
            // things like saving parameters, turning things off before quitting the program should go here
            
            StoreParameters(profilesPath + "StoppedParameters.json");

        }
        #endregion

        #region private methods for creating un-timed Tasks/channels
        // a list of functions for creating various analogTasks
        private void CreateAnalogInputTask(string channel)
        {
            analogTasks[channel] = new Task(channel);
            ((AnalogInputChannel)Environs.Hardware.AnalogInputChannels[channel]).AddToTask(
                analogTasks[channel],
                0,
                10
            );
            analogTasks[channel].Control(TaskAction.Verify);
        }

        // an overload to specify input range
        private void CreateAnalogInputTask(string channel, double lowRange, double highRange)
        {
            analogTasks[channel] = new Task(channel);
            ((AnalogInputChannel)Environs.Hardware.AnalogInputChannels[channel]).AddToTask(
                analogTasks[channel],
                lowRange,
                highRange
            );
            analogTasks[channel].Control(TaskAction.Verify);
        }


        private void CreateAnalogOutputTask(string channel)
        {
            stateRecord.analogs[channel] = (double)0.0;
            analogTasks[channel] = new Task(channel);
            AnalogOutputChannel c = ((AnalogOutputChannel)Environs.Hardware.AnalogOutputChannels[channel]);
            c.AddToTask(
                analogTasks[channel],
                c.RangeLow,
                c.RangeHigh
                );
            analogTasks[channel].Control(TaskAction.Verify);
        }

        // setting an analog voltage to an output
        public void SetAnalogOutput(string channel, double voltage)
        {
            SetAnalogOutput(channel, voltage, false);
        }
        //Overload for using a calibration before outputting to hardware
        public void SetAnalogOutput(string channelName, double voltage, bool useCalibration)
        {
            
            AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(analogTasks[channelName].Stream);
            double output;
            if (useCalibration)
            {
                try
                {
                    output = ((Calibration)calibrations[channelName]).Convert(voltage);
                }
                catch (DAQ.HAL.Calibration.CalibrationRangeException)
                {
                    MessageBox.Show("The number you have typed is out of the calibrated range! \n Try typing something more sensible.");
                    throw new CalibrationException();
                }
                catch
                {
                    MessageBox.Show("Calibration error");
                    throw new CalibrationException();
                }
            }
            else
            {
                output = voltage;
            }
            try
            {
                writer.WriteSingleSample(true, output);
                analogTasks[channelName].Control(TaskAction.Unreserve);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public class CalibrationException : ArgumentOutOfRangeException { };
        // reading an analog voltage from input
        public double ReadAnalogInput(string channel)
        {
            return ReadAnalogInput(channel, false);
        }
        public double ReadAnalogInput(string channelName, bool useCalibration)
        {
            AnalogSingleChannelReader reader = new AnalogSingleChannelReader(analogTasks[channelName].Stream);
            double val = reader.ReadSingleSample();
            analogTasks[channelName].Control(TaskAction.Unreserve);
            if (useCalibration)
            {
                try
                {
                    return ((Calibration)calibrations[channelName]).Convert(val);
                }
                catch
                {
                    MessageBox.Show("Calibration error");
                    return val;
                }
            }
            else
            {
                return val;
            }
        }
        
        // overload for reading multiple samples
        public double ReadAnalogInput(string channel, double sampleRate, int numOfSamples, bool useCalibration)
        {
            //Configure the timing parameters of the task
            analogTasks[channel].Timing.ConfigureSampleClock("", sampleRate,
                SampleClockActiveEdge.Rising, SampleQuantityMode.FiniteSamples, numOfSamples);

            //Read in multiple samples
            AnalogSingleChannelReader reader = new AnalogSingleChannelReader(analogTasks[channel].Stream);
            double[] valArray = reader.ReadMultiSample(numOfSamples);
            analogTasks[channel].Control(TaskAction.Unreserve);

            //Calculate the average of the samples
            double sum = 0;
            for (int j = 0; j < numOfSamples; j++)
            {
                sum = sum + valArray[j];
            }
            double val = sum / numOfSamples;
            if (useCalibration)
            {
                try
                {
                    return ((Calibration)calibrations[channel]).Convert(val);
                }
                catch
                {
                    MessageBox.Show("Calibration error");
                    return val;
                }
            }
            else
            {
                return val;
            }
        }


        private void CreateDigitalTask(String name)
        {
            stateRecord.digitals[name] = false;
            Task digitalTask = new Task(name);
            ((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels[name]).AddToTask(digitalTask);
            digitalTask.Control(TaskAction.Verify);
            digitalTasks.Add(name, digitalTask);
        }

        public void SetDigitalLine(string name, bool value)
        {
            Task digitalTask = ((Task)digitalTasks[name]);
            DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalTask.Stream);
            writer.WriteSingleSampleSingleLine(true, value);
            digitalTask.Control(TaskAction.Unreserve);
        }

        private void CreateHSDigitalTask(String name)
        {
            DigitalOutputChannel channel = (DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels[name];
            stateRecord.HSdigitals[name] = false;
            HSchannels.CreateHSDigitalTask(name, channel.BitNumber);
        }

        public void SetHSDigitalLine(string name, bool value)
        {
            HSchannels.SetHSDigitalLine(name, value);
        }

        #endregion

        #region keeping track of the state of the hardware!
        /// <summary>
        /// There's this thing I've called a hardware state. It's something which keeps track of digital and analog values.
        /// I then have something called stateRecord (defines above as an instance of hardwareState) which keeps track of 
        /// what the hardware is doing.
        /// Anytime the hardware gets modified by this program, the stateRecord get updated. Don't hack this. 
        /// It's useful to know what the hardware is doing at all times.
        /// When switching to REMOTE, the updates no longer happen. That's why we store the state before switching to REMOTE and apply the state
        /// back again when returning to LOCAL.
        /// </summary>
        [Serializable]
        public class HardwareState
        {
            public Dictionary<string, double> analogs;
            public Dictionary<string, bool> digitals;
            public Dictionary<string, bool> HSdigitals;

            public HardwareState()
            {
                analogs = new Dictionary<string, double>();
                digitals = new Dictionary<string, bool>();
                HSdigitals = new Dictionary<string, bool>();
            }
        }
        

        #endregion

        #region Saving and loading experimental parameters
        // Saving the parameters when closing the controller
        public void SaveParametersWithDialog()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "shc parameters|*.json";
            saveFileDialog1.Title = "Save parameters";
            saveFileDialog1.InitialDirectory = profilesPath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    StoreParameters(saveFileDialog1.FileName);
                }
            }
        }

        private void StoreParameters(String dataStoreFilePath)
        {
            stateRecord = readValuesOnUI();
            string record = JsonConvert.SerializeObject(stateRecord);
            System.IO.File.WriteAllText(dataStoreFilePath, record);
            controlWindow.WriteToConsole("Saved parameters to " + dataStoreFilePath);
        }

        //Load parameters when opening the controller
        public void LoadParametersWithDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Nav HC|*.json";
            dialog.Title = "Load parameters";
            dialog.InitialDirectory = profilesPath;
            dialog.ShowDialog();
            if (dialog.FileName != "") stateRecord = loadParameters(dialog.FileName);
            setValuesDisplayedOnUI(stateRecord);
            controlWindow.WriteToConsole("Parameters loaded from " + dialog.FileName);
        }

        private HardwareState loadParameters(String dataStoreFilePath)
        {
            if (!File.Exists(dataStoreFilePath))
            {
                controlWindow.WriteToConsole("Can't Find File");
                return stateRecord;
            }

            HardwareState state;

            using (StreamReader r = new StreamReader(dataStoreFilePath))
            {

                    string json = r.ReadToEnd();
                    state = JsonConvert.DeserializeObject<HardwareState>(json);
            }

            return state;
        }
        #endregion

        #region Controlling hardware and UI.
        //This gets/sets the values on the GUI panel

        #region Updating the hardware

        public void ApplyRecordedStateToHardware()
        {
            applyToHardware(stateRecord);          
        }


        public void UpdateHardware()
        {
            HardwareState uiState = readValuesOnUI();

            DoUpdateHardware(uiState);
        }

        public void DoUpdateHardware(HardwareState newState)
        {

            HardwareState changes = getDiscrepancies(stateRecord, newState);

            applyToHardware(changes);

            updateStateRecord(changes);

            setValuesDisplayedOnUI(stateRecord);

        }

        private void applyToHardware(HardwareState state)
        {
            if (state.analogs.Count != 0 || state.digitals.Count != 0 || state.HSdigitals.Count != 0)
            {
                if (HCState == HCUIControlState.OFF)
                {

                    HCState = HCUIControlState.LOCAL;
                    controlWindow.UpdateUIState(HCState);

                    applyAnalogs(state);
                    applyDigitals(state);
                    applyHSDigitals(state);

                    HCState = HCUIControlState.OFF;
                    controlWindow.UpdateUIState(HCState);
                }
            }
            else
            {
                controlWindow.WriteToConsole("The values on the UI are identical to those on the controller's records. Hardware must be up to date.");
            }
        }

        private HardwareState getDiscrepancies(HardwareState oldState, HardwareState newState)
        {
            HardwareState state = new HardwareState();
            state.analogs = new Dictionary<string, double>();
            state.digitals = new Dictionary<string, bool>();
            state.HSdigitals = new Dictionary<string, bool>();

            foreach(KeyValuePair<string, double> pairs in oldState.analogs)
            {
                if (oldState.analogs[pairs.Key] != newState.analogs[pairs.Key])
                {
                    state.analogs[pairs.Key] = newState.analogs[pairs.Key];
                }
            }

            foreach (KeyValuePair<string, bool> pairs in oldState.digitals)
            {
                if (oldState.digitals[pairs.Key] != newState.digitals[pairs.Key])
                {
                    state.digitals[pairs.Key] = newState.digitals[pairs.Key];
                }
            }

            foreach (KeyValuePair<string, bool> pairs in oldState.HSdigitals)
            {
                if (oldState.HSdigitals[pairs.Key] != newState.HSdigitals[pairs.Key])
                {
                    state.HSdigitals[pairs.Key] = newState.HSdigitals[pairs.Key];
                }
            }

            return state;
        }

        private void updateStateRecord(HardwareState changes)
        {
            foreach (KeyValuePair<string, double> pairs in changes.analogs)
            {
                stateRecord.analogs[pairs.Key] = changes.analogs[pairs.Key];
            }

            foreach (KeyValuePair<string, bool> pairs in changes.digitals)
            {
                stateRecord.digitals[pairs.Key] = changes.digitals[pairs.Key];
            }

            foreach (KeyValuePair<string, bool> pairs in changes.HSdigitals)
            {
                stateRecord.HSdigitals[pairs.Key] = changes.HSdigitals[pairs.Key];
            }
        }

        
        private void applyAnalogs(HardwareState state)
        {
            List<string> toRemove = new List<string>();  //In case of errors, keep track of things to delete from the list of changes.
            foreach (KeyValuePair<string, double> pairs in state.analogs)
            {
                try
                {
                    if (calibrations.ContainsKey(pairs.Key))
                    {
                        SetAnalogOutput(pairs.Key, pairs.Value, true);

                    }
                    else
                    {
                        SetAnalogOutput(pairs.Key, pairs.Value);
                    }
                    controlWindow.WriteToConsole("Set channel '" + pairs.Key.ToString() + "' to " + pairs.Value.ToString());
                }
                catch (CalibrationException)
                {
                    controlWindow.WriteToConsole("Failed to set channel '"+ pairs.Key.ToString() + "' to new value");                    
                    toRemove.Add(pairs.Key);
                }
            }
            foreach (string s in toRemove)  //Remove those from the list of changes, as nothing was done to the Hardware.
            {
                state.analogs.Remove(s);
            }
        }

        private void applyDigitals(HardwareState state)
        {
            foreach (KeyValuePair<string, bool> pairs in state.digitals)
            {
                SetDigitalLine(pairs.Key, pairs.Value);
                controlWindow.WriteToConsole("Set channel '" + pairs.Key.ToString() + "' to " + pairs.Value.ToString());
            }
        }

        private void applyHSDigitals(HardwareState state)
        {
            foreach (KeyValuePair<string, bool> pairs in state.HSdigitals)
            {
                SetHSDigitalLine(pairs.Key, pairs.Value);
                controlWindow.WriteToConsole("Set channel '" + pairs.Key.ToString() + "' to " + pairs.Value.ToString());
            }
        }
        #endregion 

        #region Reading and Writing to UI

        private HardwareState readValuesOnUI()
        {
            HardwareState state = new HardwareState();
            state.analogs = readUIAnalogs(stateRecord.analogs.Keys);
            state.digitals = readUIDigitals(stateRecord.digitals.Keys);
            state.HSdigitals = readUIHSDigitals(stateRecord.HSdigitals.Keys);
            return state;
        }
        private Dictionary<string, double> readUIAnalogs(Dictionary<string, double>.KeyCollection keys)
        {
            Dictionary<string, double> analogs = new Dictionary<string, double>();
            string[] keyArray = new string[keys.Count];
            keys.CopyTo(keyArray, 0);
            for (int i = 0; i < keys.Count; i++)
            {
                analogs[keyArray[i]] = controlWindow.ReadAnalog(keyArray[i]);
            }
            return analogs;
        }

        private Dictionary<string, bool> readUIDigitals(Dictionary<string, bool>.KeyCollection keys)
        {
            Dictionary<string, bool> digitals = new Dictionary<string,bool>();
            string[] keyArray = new string[keys.Count];
            keys.CopyTo(keyArray, 0);
            for (int i = 0; i < keys.Count; i++)
            {
                digitals[keyArray[i]] = controlWindow.ReadDigital(keyArray[i]);
            }
            return digitals;
        }

        private Dictionary<string, bool> readUIHSDigitals(Dictionary<string, bool>.KeyCollection keys)
        {
            Dictionary<string, bool> digitals = new Dictionary<string, bool>();
            string[] keyArray = new string[keys.Count];
            keys.CopyTo(keyArray, 0);
            for (int i = 0; i < keys.Count; i++)
            {
                digitals[keyArray[i]] = controlWindow.ReadHSDigital(keyArray[i]);
            }
            return digitals;
        }
       

        private void setValuesDisplayedOnUI(HardwareState state)
        {
            setUIAnalogs(state);
            setUIDigitals(state);
            setUIHSDigitals(state);
        }
        private void setUIAnalogs(HardwareState state)
        {
            foreach (KeyValuePair<string, double> pairs in state.analogs)
            {
                try
                {
                    controlWindow.SetAnalog(pairs.Key, (double)pairs.Value);
                }
                catch 
                {
                    MessageBox.Show("Some parameters couldn't be loaded, check the parameter file in the settings folder");
                }
            }
        }

        private void setUIDigitals(HardwareState state)
        {
            foreach (KeyValuePair<string, bool> pairs in state.digitals)
            {
                controlWindow.SetDigital(pairs.Key, (bool)pairs.Value);
            }
        }

        private void setUIHSDigitals(HardwareState state)
        {
            foreach (KeyValuePair<string, bool> pairs in state.HSdigitals)
            {
                controlWindow.SetHSDigital(pairs.Key, (bool)pairs.Value);
            }
        }

        #endregion

        #region Remoting stuff 

        /// <summary>
        /// This is used when you want another program to take control of some/all of the hardware. The hc then just saves the
        /// last hardware state, then prevents you from making any changes to the UI. Use this if your other program wants direct control of hardware.
        /// </summary>
        public void StartRemoteControl()
        {
            if (HCState == HCUIControlState.OFF)
            {
                if (!ImageController.IsCameraFree())
                {
                    StopCameraStream();
                }             
                StoreParameters(profilesPath + "tempParameters.json");
                HCState = HCUIControlState.REMOTE;
                controlWindow.UpdateUIState(HCState);
                controlWindow.WriteToConsole("Remoting Started!");
            }
            else
            {
                MessageBox.Show("Controller is busy");
            }

        }
        public void StopRemoteControl()
        {
            try
            {
                controlWindow.WriteToConsole("Remoting Stopped!");
                setValuesDisplayedOnUI(loadParameters(profilesPath + "tempParameters.json"));
                
                if (System.IO.File.Exists(profilesPath + "tempParameters.json"))
                {
                    System.IO.File.Delete(profilesPath + "tempParameters.json");
                }
            }
            catch (Exception)
            {
                controlWindow.WriteToConsole("Unable to load Parameters.");
            }
            HCState = HCUIControlState.OFF;
            controlWindow.UpdateUIState(HCState);
            ApplyRecordedStateToHardware();
        }

        /// <summary>
        /// These SetValue functions are for giving commands to the hc from another program, while keeping the hc in control of hardware.
        /// Use this if you want the HC to keep control, but you want to control the HC from some other program
        /// </summary>
        public void SetValue(string channel, double value)
        {
            HCState = HCUIControlState.LOCAL;
            stateRecord.analogs[channel] = value;
            SetAnalogOutput(channel, value, false);
            setValuesDisplayedOnUI(stateRecord);
            HCState = HCUIControlState.OFF;

        }
        public void SetValue(string channel, double value, bool useCalibration)
        {
            stateRecord.analogs[channel] = value;
            HCState = HCUIControlState.LOCAL;
            SetAnalogOutput(channel, value, useCalibration);
            setValuesDisplayedOnUI(stateRecord);
            HCState = HCUIControlState.OFF;

        }
        public void SetValue(string channel, bool value)
        {
            HCState = HCUIControlState.LOCAL;
            if (stateRecord.digitals.ContainsKey(channel))
            {
                stateRecord.digitals[channel] = value;
                SetDigitalLine(channel, value);
                setValuesDisplayedOnUI(stateRecord);
            }
            else if (stateRecord.HSdigitals.ContainsKey(channel))
            {
                stateRecord.digitals[channel] = value;
                SetHSDigitalLine(channel, value);
                setValuesDisplayedOnUI(stateRecord);
            }

            HCState = HCUIControlState.OFF;

        }
        #endregion

        #endregion

        #region Local camera control

        public void StartCameraControl()
        {
            try
            {
                ImageController = new CameraController("cam0");
                ImageController.Initialize();
                ImageController.SetCameraAttributes(cameraAttributesPath);
                ImageController.PrintCameraAttributesToConsole();
                cameraLoaded = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Camera Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();

            }
        }
        public void CameraStream()
        {
            try
            {
                ImageController.Stream(cameraAttributesPath);
            }
            catch { }
        }

        public void StopCameraStream()
        {
            try
            {
                ImageController.StopStream();
            }
            catch { }
        }

        public void CameraSnapshot()
        {
            try
            {
                ImageController.SingleSnapshot(cameraAttributesPath);
            }
            catch { }
        }

        public ushort[,] TempCameraSnapshot()
        {
            return ImageController.SingleSnapshot(cameraAttributesPath);
        }
       
        #endregion

        #region Release/Reclaim Hardware

        public void ReleaseHardware()
        {
            HSchannels.ReleaseHardware();
        }

        public void ReclaimHardware()
        {
            HSchannels.ReclaimHardware();
        }

        #endregion

        #region Remote Camera Control
        //Written for taking images triggered by TTL. This "Arm" sets the camera so it's expecting a TTL.

        public ushort[,] GrabSingleImage(string cameraAttributesPath)
        {
            return ImageController.SingleSnapshot(cameraAttributesPath);
        }

        public ushort[][,] GrabMultipleImages(string cameraAttributesPath, int numberOfShots)
        {
            try
            {
                ushort[][,] images = ImageController.MultipleSnapshot(cameraAttributesPath, numberOfShots);
                return images;
            }

            catch (TimeoutException)
            {
                FinishRemoteCameraControl();
                return null;
            }

        }

        public bool IsReadyForAcquisition()
        {
            return ImageController.IsReadyForAcqisition();
        }

        public void PrepareRemoteCameraControl()
        {
            StartRemoteControl();
        }
        public void FinishRemoteCameraControl()
        {
            StopRemoteControl();
        }

        public bool doesCameraExist()
        {
            return cameraLoaded;
        }

        #endregion

        #region Saving Images

        public void SaveImageWithDialog()
        {
            ImageController.SaveImageWithDialog();
        }
        public void SaveImage(string path)
        {
            ImageController.SaveImage(path);
        }
        #endregion

        #region Hardware Monitor

        #region Remote Access for Hardware Monitor

        public Dictionary<String, Object> GetExperimentReport()
        {
            Dictionary<String, Object> report = new Dictionary<String, Object>();
            report["testReport"] = "Report?";
            foreach (KeyValuePair<string, double> pair in stateRecord.analogs)
            {
                report[pair.Key] = pair.Value;
            }
            foreach (KeyValuePair<string, bool> pair in stateRecord.digitals)
            {
                report[pair.Key] = pair.Value;
            }
            return report;
        }
        #endregion

        #endregion

        #region Stuff To Access from python
        ///In theory you can access any public method from python. 
        ///In practice we only really want to access a small number of them.
        ///This region contains them. Hopefully by only putting functions in here we
        ///want to use we can get an idea of what, exactly we want this control 
        ///software to do.
        ///
        public void RemoteTest(object input)
        {
            controlWindow.WriteToConsole(input.ToString());
        }

        public string RemoteSetChannel(String channelName, object value)
        {
            if (digitalTasks.Contains(channelName))
            {
                if (value.GetType() != typeof(bool))
                {
                    return "Can't set digital channel to non bool value";
                }

                HardwareState state = readValuesOnUI();
                state.digitals[channelName] = (bool)value;
                DoUpdateHardware(state);
                return "";
            }

            if (analogTasks.ContainsKey(channelName))
            {
                float outValue;
                bool didItParse = float.TryParse(value.ToString(), out outValue);

                if (!didItParse)
                {
                    return "Cant't set an analog channel to non numeric value";
                }

                HardwareState state = readValuesOnUI();
                state.analogs[channelName] = outValue;
                DoUpdateHardware(state);
                return "";
            }

            return "Channel name not found";
        }

        public string[] RemoteGetChannels()
        {
            List<string> channels = new List<string>();
            channels.AddRange(stateRecord.digitals.Keys);
            channels.AddRange(stateRecord.analogs.Keys);
            return channels.ToArray();
        }

        public string RemoteLoadParameters(string file)
        {
            if (!File.Exists(file))
            {
                return "file doesn't exist";
            }

            stateRecord = loadParameters(file);
            setValuesDisplayedOnUI(stateRecord);
            controlWindow.WriteToConsole("Loaded parameters from " + file);
            return "";
            
        }

        public void CloseIt()
        {
            controlWindow.Close();
        }

        #endregion
    }
}

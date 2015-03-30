﻿using System;
using System.Collections;
using System.Diagnostics;
using NationalInstruments.DAQmx;
using DAQ.Pattern;

namespace DAQ.HAL
{
    public class NavigatorHardware : DAQ.HAL.Hardware
    {
        public NavigatorHardware()
        {
            //add the boards
            Boards.Add("multiDAQ","/MultiDAQ");
            Boards.Add("digIO", "/DigitalIO");
            Boards.Add("analogOut", "/AnalogOut");

            //Temporary stuff to deal with unknown EDMS bullshit
            string daq = (string)Boards["multiDAQ"];
            Info.Add("pgBoard", "/MultiDAQ");
            Info.Add("PGClockCounter", "/ctr0");
            Info.Add("PGType", "integrated");
            Info.Add("Element", "Rb");
            AddDigitalOutputChannel("testDigitalChannel", daq, 0, 0);
            AddDigitalOutputChannel("AnalogPatternTrigger", daq, 0, 1);
          
            //map the digital channels
            string daqBoard = (string)Boards["digIO"];
            //General channels
            AddDigitalOutputChannel("coolingSwitch", daqBoard, 0, 6);
            AddDigitalOutputChannel("repumpSwitch", daqBoard, 0, 7);
            AddDigitalOutputChannel("cameraTrigger", daqBoard, 1, 6);

            //map the analogue channels
            string analogBoard = (string)Boards["analogOut"];
            //Timing and trigger
            Info.Add("AOClockSource", analogBoard + "/PFI1");
            Info.Add("AOPatternTrigger", analogBoard + "/PFI0");
            //Other stuff
            AddAnalogOutputChannel("testAnalogChannel", analogBoard + "/ao0");
            

            AddAnalogOutputChannel("aom1freq", analogBoard + "/ao1");
            AddCalibration("aom1freq", new LinearCalibration((1/8.876),40.343,0,0,130));

            AddAnalogOutputChannel("motShutter", analogBoard + "/ao2");
            AddAnalogOutputChannel("imagingShutter", analogBoard + "/ao3");
            //add the camera
            Instruments.Add("motCam", "/cam0");

            //map the camera
            string motCamera = (string)Instruments["motCam"];

      
        }
    }
}

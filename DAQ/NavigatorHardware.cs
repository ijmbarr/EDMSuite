using System;
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
            Info.Add("pgBoard", "/DigitalIO");
            Info.Add("PGClockCounter", "/ctr0");
            Info.Add("PGType", "dedicated");
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
            //AddDigitalInputChannel("AOPatternTrigger", analogBoard, 0, 0);
            //AddDigitalInputChannel("AOClockSource", analogBoard, 0, 1);
            Info.Add("AOClockSource", analogBoard + "/PFI1");
            Info.Add("AOPatternTrigger", analogBoard + "/PFI0");
            //Other stuff
            AddAnalogOutputChannel("testAnalogChannel", analogBoard + "/ao0");
            AddAnalogOutputChannel("xbiasCurrent", analogBoard + "/ao1");
            AddAnalogOutputChannel("ybiasCurrent", analogBoard + "/ao2");
            AddAnalogOutputChannel("zbiasCurrent", analogBoard + "/ao3");
            AddAnalogOutputChannel("coolingDetuning", analogBoard + "/ao4");
            AddAnalogOutputChannel("coolingPower", analogBoard + "/ao5");
            AddAnalogOutputChannel("repumpDetuning", analogBoard + "/ao6");
            AddAnalogOutputChannel("repumpPower", analogBoard + "/ao7");

            //add the camera
            Instruments.Add("motCam", "/cam0");

            //map the camera
            string motCamera = (string)Instruments["motCam"];

      
        }
    }
}

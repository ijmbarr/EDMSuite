using System;
using System.Collections;

using NationalInstruments.DAQmx;

using DAQ.Pattern;

namespace DAQ.HAL
{
	/// <summary>
	/// This is the specific hardware that the edm machine has. This class conforms
	/// to the Hardware interface.
	/// </summary>
	public class EDMHardware : DAQ.HAL.Hardware
	{

		public EDMHardware()
		{

			// add the boards
			Boards.Add("daq", "/dev1");
			Boards.Add("pg", "/dev2");
			Boards.Add("counter", "/dev3");

			// YAG laser
			yag = new BrilliantLaser("ASRL1::INSTR");

			// add the synths
			GPIBInstruments.Add("green", new HP8657ASynth("GPIB0::7::INSTR"));
			GPIBInstruments.Add("red", new HP3325BSynth("GPIB0::12::INSTR"));
			GPIBInstruments.Add("4861", new ICS4861A("GPIB0::4::INSTR"));
			GPIBInstruments.Add("bCurrentMeter", new HP34401A("GPIB0::22::INSTR"));

			// map the digital channels
			string pgBoard = (string)Boards["pg"];
			// these channels are generally switched by the pattern generator
			// they're all in the lower half of the pg
			AddDigitalOutputChannel("valve", pgBoard, 0, 0);
			AddDigitalOutputChannel("flash", pgBoard, 0, 1);
			AddDigitalOutputChannel("q", pgBoard, 0, 2);
			AddDigitalOutputChannel("detector", pgBoard, 0, 3);
			AddDigitalOutputChannel("detectorprime", pgBoard, 1, 2); // this trigger is for switch scanning
																	// see ModulatedAnalogShotGatherer.cs
																	// for details.
			AddDigitalOutputChannel("rf1Switch", pgBoard, 0, 4);
			AddDigitalOutputChannel("rf2Switch", pgBoard, 0, 5);
			AddDigitalOutputChannel("greenFM", pgBoard, 1, 0);
			AddDigitalOutputChannel("piFlip", pgBoard, 1, 1);
			AddDigitalOutputChannel("ttlSwitch", pgBoard, 1, 3);	// This is the output that the pg
																	// will switch if it's switch scanning

			// these channel are usually software switched - they should not be in
			// the lower half of the pattern generator
			AddDigitalOutputChannel("b", pgBoard, 2, 0);
			AddDigitalOutputChannel("notB", pgBoard, 2, 1);
			AddDigitalOutputChannel("db", pgBoard, 2, 2);
			AddDigitalOutputChannel("notDB", pgBoard, 2, 3);
			AddDigitalOutputChannel("notEOnOff", pgBoard, 2, 4);  // this line seems to be broken on our pg board
			AddDigitalOutputChannel("eOnOff", pgBoard, 2, 5);
			AddDigitalOutputChannel("ePol", pgBoard, 2, 6);
			AddDigitalOutputChannel("notEPol", pgBoard, 2, 7);
			AddDigitalOutputChannel("eBleed", pgBoard, 3, 0);
			AddDigitalOutputChannel("piFlipEnable", pgBoard, 3, 1);
			AddDigitalOutputChannel("notPIFlipEnable", pgBoard, 3, 2);

			// map the analog channels
			string daqBoard = (string)Boards["daq"];
			AddAnalogInputChannel("pmt",  daqBoard + "/ai0", AITerminalConfiguration.Differential);
			AddAnalogInputChannel("magnetometer", daqBoard + "/ai1", AITerminalConfiguration.Differential);
			AddAnalogInputChannel("iodine", daqBoard + "/ai2", AITerminalConfiguration.Nrse);
			AddAnalogInputChannel("cavity", daqBoard + "/ai3", AITerminalConfiguration.Nrse);
			AddAnalogOutputChannel("laser", daqBoard + "/ao0");
			AddAnalogOutputChannel("b", daqBoard + "/ao1");

			// map the counter channels
			string counterBoard = (string)Boards["counter"];
			AddCounterChannel("phaseLockOscillator", counterBoard + "/ctr7");
			AddCounterChannel("phaseLockReference", counterBoard + "/pfi10");
			AddCounterChannel("currentLeakageMonitorNorth-C", counterBoard +"/ctr1");
			AddCounterChannel("currentLeakageMonitorNorth-G", counterBoard +"/ctr7");
			AddCounterChannel("currentLeakageMonitorSouth-C", counterBoard +"/ctr0");
			AddCounterChannel("currentLeakageMonitorSouth-G", counterBoard +"/ctr7");

		}

	}
}
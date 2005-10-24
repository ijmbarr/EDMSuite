using System;

using DAQ.Environment;
using DAQ.HAL;
using DAQ.Pattern;

namespace ScanMaster.Acquire.Patterns
{
	/// <summary>
	/// 
	/// </summary>
	public class PumpProbePatternBuilder : DAQ.Pattern.PatternBuilder32
	{
		public PumpProbePatternBuilder()
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		private const int FLASH_PULSE_LENGTH = 100;
		private const int Q_PULSE_LENGTH = 100;
		private const int DETECTOR_TRIGGER_LENGTH = 20;
	
		public int ShotSequence( int startTime, int numberOfOnOffShots, int padShots, int flashlampPulseInterval,
			int valvePulseLength, int valveToQ, int flashToQ, int aomStart1, int aomDuration1,
			int aomStart2, int aomDuration2, int delayToDetectorTrigger,
			int ttlSwitchPort, int ttlSwitchLine) 
		{
		
			int time = 0;

			
		
			for (int i = 0 ; i < numberOfOnOffShots ; i++ ) 
			{
				
				int switchChannel = PatternBuilder32.ChannelFromNIPort(ttlSwitchPort,ttlSwitchLine);
				// first the pulse with the switch line high
				AddEdge(switchChannel, time, true);
				Shot( time, valvePulseLength, valveToQ, flashToQ, aomStart1, aomDuration1, aomStart2, aomDuration2, delayToDetectorTrigger );
				time += flashlampPulseInterval;
				for (int p = 0 ; p < padShots ; p++)
				{
					FlashlampPulse(time, valveToQ, flashToQ);
					time += flashlampPulseInterval;
				}
				// now with the switch line low
				
				AddEdge(switchChannel, time, false);
				Shot( time, valvePulseLength, valveToQ, flashToQ, aomStart1, aomDuration1, aomStart2, aomDuration2, delayToDetectorTrigger );
				time += flashlampPulseInterval;
				for (int p = 0 ; p < padShots ; p++)
				{
					FlashlampPulse(time, valveToQ, flashToQ);
					time += flashlampPulseInterval;
				}
			} 
		
			return time;
		}

		public int FlashlampPulse( int startTime, int valveToQ, int flashToQ )
		{
			return Pulse(startTime, valveToQ - flashToQ, FLASH_PULSE_LENGTH,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["flash"]).BitNumber);
		}

		public int Shot( int startTime, int valvePulseLength, int valveToQ, int flashToQ, int aomStart1, int aomDuration1,
            int aomStart2, int aomDuration2, int delayToDetectorTrigger)  
		{
			int time = 0;
			int tempTime = 0;

			// valve pulse
			tempTime = Pulse(startTime, 0, valvePulseLength,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["valve"]).BitNumber);
			if (tempTime > time) time = tempTime;
			// Flash pulse
			tempTime = Pulse(startTime, valveToQ - flashToQ, FLASH_PULSE_LENGTH, 
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["flash"]).BitNumber);
			if (tempTime > time) time = tempTime;
			// Q pulse
			tempTime = Pulse(startTime, valveToQ, Q_PULSE_LENGTH,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["q"]).BitNumber);
			if (tempTime > time) time = tempTime;
			// aom pulse 1
			tempTime = Pulse(startTime, aomStart1 + valveToQ, aomDuration1,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["aom"]).BitNumber);
			if (tempTime > time) time = tempTime;
			// aom pulse 2
			tempTime = Pulse(startTime, aomStart2 + valveToQ, aomDuration2,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["aom"]).BitNumber);
			if (tempTime > time) time = tempTime;
			// Detector trigger
			tempTime = Pulse(startTime, delayToDetectorTrigger + valveToQ, DETECTOR_TRIGGER_LENGTH,
				((DigitalOutputChannel)Environs.Hardware.DigitalOutputChannels["detector"]).BitNumber);
			if (tempTime > time) time = tempTime;

		
			return time;
		}
	
	}
}
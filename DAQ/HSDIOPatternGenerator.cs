using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.Interop;

using DAQ.Environment;

namespace DAQ.HAL
{
    public class HSDIOPatternGenerator : PatternGenerator
    {
        niHSDIO my_niHSDIO;
        string resourceName;
        string channelList;
        string clockSource;
        double clockRate;
        uint[] waveFormArray;
        int numberOfSamples; 

        public HSDIOPatternGenerator(string name)
        {
            resourceName = name;
        }

        public void Configure(double clockFrequency, bool loop, bool fullWidth, bool lowGroup,
                    int length, bool internalClock, bool triggered)
        {
            clockRate = clockFrequency;
            clockSource = (internalClock) ? "OnBoardClock" : "OnBoardClock"; 
            channelList = (fullWidth) ? "0-31" : ((lowGroup) ? "0-15" : "16-31");

            my_niHSDIO = niHSDIO.InitGenerationSession(resourceName, true, false, "");
            my_niHSDIO.AssignDynamicChannels(channelList);
            my_niHSDIO.ConfigureSampleClock(clockSource, clockRate);
        }

        public void OutputPattern(UInt32[] pattern)
        {
            my_niHSDIO.WriteNamedWaveformU32("pattern", pattern.Count(), pattern);
            numberOfSamples = pattern.Length;
            my_niHSDIO.Initiate();
        }

        public void StopPattern()
        {
            my_niHSDIO.WaitUntilDone((int)(clockRate * numberOfSamples));
            my_niHSDIO.Dispose();
        }

        public void Dispose()
        {
            my_niHSDIO.Dispose();
        }
    }
}

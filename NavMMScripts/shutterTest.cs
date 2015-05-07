using MOTMaster;
using MOTMaster.SnippetLibrary;

using System;
using System.Collections.Generic;

using DAQ.Pattern;
using DAQ.Analog;

// This script can be used to test any aspect of the absorption imaging. 

public class Patterns : MOTMasterScript
{


    public Patterns()
    {
        Parameters = new Dictionary<string, object>();
        //Required Stuff
        Parameters["PatternLength"] = 35000;

        Parameters["NumberOfFrames"] = 2;
        Parameters["AbsAnalysis"] = true;
        Parameters["NeedsCamera"] = false;
        //Pattern Stuff
        Parameters["motDetuning"] = 102.0;
        Parameters["imagingDetuning"] = 108.0;
        Parameters["imageDelay"] = -500;
        //Delays for Opening and Closing Shutters

        Parameters["openDelay"] = 26;
        Parameters["closeDelay"] = 122;

    }

    public override PatternBuilder32 GetHSDigitialPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();

        return pattern;
    }

    public override PatternBuilder32 GetDigitalPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();

        pattern.Pulse(0, 0, 1, "AnalogPatternTrigger");

        pattern.Pulse(10000 + (int)Parameters["imageDelay"], 0, 100, "testDigitalChannel");
      //  pattern.Pulse(30000, 0, 100, "testDigitalChannel");


        return pattern;
    }

    public override AnalogPatternBuilder GetAnalogPattern()
    {
        AnalogPatternBuilder pattern = new AnalogPatternBuilder((int)Parameters["PatternLength"]);

        pattern.AddChannel("testAnalogChannel");
        pattern.AddChannel("motShutter");
        pattern.AddChannel("imagingShutter");
        pattern.AddChannel("aom1freq");

        //Load MOT state
        pattern.AddAnalogValue("imagingShutter", 0, 5);
        pattern.AddAnalogValue("motShutter", 0, 0);
        pattern.AddAnalogValue("aom1freq", 0, (double)Parameters["motDetuning"]);

        //Abs Image
        pattern.AddAnalogValue("motShutter", 10000 - (int)Parameters["closeDelay"], 5);

         pattern.AddAnalogValue("aom1freq", 10000+3, (double)Parameters["imagingDetuning"]);

        pattern.AddAnalogValue("imagingShutter", 10000 - (int)Parameters["openDelay"] + (int)Parameters["imageDelay"], 0);

        //Bkg Image

        pattern.SwitchAllOffAtEndOfPattern();
        return pattern;
    }
}
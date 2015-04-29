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
        Parameters["PatternLength"] = 32000;
        Parameters["NeedsCamera"] = true;
        Parameters["NumberOfFrames"] = 2;
        Parameters["AbsAnalysis"] = true;
        //Pattern Stuff
        Parameters["motDetuning"] = 103.0;
        Parameters["imagingDetuning"] = 109.0;
        Parameters["imageDelay"] = 10;
        Parameters["motCoilCurrent"] = 5.0;
        //Delays for Opening and Closing Shutters
        Parameters["openDelay"] = 40;
        Parameters["closeDelay"] = 32;
        
    }

    public override PatternBuilder32 GetDigitalPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();

        pattern.Pulse(0, 0, 1, "AnalogPatternTrigger");

        pattern.Pulse(30000 + (int)Parameters["imageDelay"], 0, 100, "testDigitalChannel");
        pattern.Pulse(31000 + (int)Parameters["imageDelay"], 0, 100, "testDigitalChannel");


        return pattern;
    }


    public override AnalogPatternBuilder GetAnalogPattern()
    {
        AnalogPatternBuilder pattern = new AnalogPatternBuilder((int)Parameters["PatternLength"]);

        pattern.AddChannel("motCoil");
        pattern.AddChannel("motShutter");
        pattern.AddChannel("imagingShutter");
        pattern.AddChannel("aom1freq");
        pattern.AddChannel("rfSwitch");

        //Load MOT state
        pattern.AddAnalogValue("motCoil", 0, (double)Parameters["motCoilCurrent"]);
        pattern.AddAnalogValue("imagingShutter", 0, 5);
        pattern.AddAnalogValue("motShutter", 0, 0);
        pattern.AddAnalogValue("aom1freq", 0, (double)Parameters["motDetuning"]);
        pattern.AddAnalogValue("rfSwitch", 0, 0);

        //Abs Image
        pattern.AddAnalogValue("motCoil", 30000 - 70, 0.0); //Turn off coils
        pattern.AddAnalogValue("rfSwitch", 30000, 5.0); //Turn off light

        //switch shutter positon (takes ~1ms) + changes detuning for imaging
        pattern.AddAnalogValue("motShutter", 30000 - (int)Parameters["closeDelay"], 5);
        pattern.AddAnalogValue("imagingShutter", 30000 - (int)Parameters["openDelay"], 0);
        pattern.AddAnalogValue("aom1freq", 30000+3, (double)Parameters["imagingDetuning"]);

        //Turns light on for imaging
        pattern.AddAnalogPulse("rfSwitch", 30000 + (int)Parameters["imageDelay"], 5, 0.0, 5);
        
        //Bkg Image
        pattern.AddAnalogPulse("rfSwitch", 31000 + (int)Parameters["imageDelay"], 5, 0.0, 5);

        pattern.SwitchAllOffAtEndOfPattern();
        return pattern;
    }

}

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
        Parameters["PatternLength"] = 10000;
       
        Parameters["startFrequency"] = 102.0;
        Parameters["endFrequency"] = 72.0;

        Parameters["startAttenuation"] = 10.0;
        Parameters["endAttenuation"] = 10.0;

        Parameters["calibFreq"] = true;
        Parameters["calibAtten"] = false;

        Parameters["stepNo"] = 1000;


    }

    public override PatternBuilder32 GetHSDigitialPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();

        return pattern;
    }

    public override PatternBuilder32 GetDigitalPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();
        //The trigger fires when the ramps start
        pattern.Pulse(10, 0, 1, "AnalogPatternTrigger");

        return pattern;
    }

    public override AnalogPatternBuilder GetAnalogPattern()
    {
        AnalogPatternBuilder pattern = new AnalogPatternBuilder((int)Parameters["PatternLength"]);

        pattern.AddChannel("aom1freq");
        pattern.AddChannel("rfAtten");
        
        pattern.AddAnalogValue("aom1freq",0,(double)Parameters["startFrequency"]);
        pattern.AddAnalogValue("rfAtten",0,(double)Parameters["startAttenuation"]);
        if ((bool)Parameters["calibFreq"])
        {
            pattern.AddLinearRamp("aom1freq", 10, (int)Parameters["stepNo"], (double)Parameters["endFrequency"]);
        }
        else if ((bool)Parameters["calibAtten"])
        {
            pattern.AddLinearRamp("rfAtten", 10, (int)Parameters["stepNo"], (double)Parameters["endAttenuation"]);
        }
        pattern.SwitchAllOffAtEndOfPattern();
        return pattern;
    }

}

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
        Parameters["PatternLength"] = 50000;


    }

    public override PatternBuilder32 GetDigitalPattern()
    {
        PatternBuilder32 pattern = new PatternBuilder32();

        pattern.Pulse(0, 0, 1, "AnalogPatternTrigger");
        pattern.Pulse(100, 0, 50, "testDigitalChannel");
        pattern.Pulse(200, 0, 50, "testDigitalChannel");
        pattern.Pulse(300, 0, 50, "testDigitalChannel");  

        return pattern;
    }

    public override AnalogPatternBuilder GetAnalogPattern()
    {
        AnalogPatternBuilder pattern = new AnalogPatternBuilder((int)Parameters["PatternLength"]);

        pattern.AddChannel("motCoil");
        pattern.AddAnalogValue("motCoil", 0, 4.0);
        pattern.AddLinearRamp("motCoil", 1, 40000, 7);
      

        pattern.SwitchAllOffAtEndOfPattern();
        return pattern;
    }

}

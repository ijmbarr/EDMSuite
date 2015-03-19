using System;
using System.Collections.Generic;
using System.Text;

namespace DAQ
{
    class NavCompFileSystem : DAQ.Environment.FileSystem
    {
        public NavCompFileSystem()
        {
            //add some paths to settings and data
            Paths.Add("mathPath", "c:\\Program Files\\Wolfram Research\\Mathematica\\10.0\\mathkernel.exe");
            Paths.Add("settingsPath", "c:\\Data\\Settings");
            Paths.Add("MOTMasterDataPath", "c:\\Data\\Nav\\data\\");
            Paths.Add("fakeData", "c:\\Data\\Examples");
            Paths.Add("CameraAttributesPath", "c:\\Data\\Settings\\CameraAttributes.icd");
            Paths.Add("CameraAttributesPathTrigger", "c:\\Data\\Settings\\CameraAttributesTrigger.icd");

            //Hardware
            Paths.Add("profilesPath", Paths["settingsPath"] + "\\NavigatorHardwareController");
            Paths.Add("calibrations", Paths["settingsPath"] + "\\Calibrations");

            //stuff
            Paths.Add("root", "C:\\EDMSuite");
            Paths.Add("MOTMasterEXEPath", Paths["root"] + "\\MOTMaster\\bin\\Navigator");
            Paths.Add("daqDLLPath", Paths["MOTMasterEXEPath"] + "\\DAQ.dll");
            Paths.Add("scriptListPath", Paths["root"] + "\\NavMMScripts");
            Paths.Add("HardwareClassPath", Paths["root"] + "\\DAQ\\NavigatorHardware.cs");

            DataSearchPaths.Add(Paths["navDataPath"]);

            SortDataByDate = false;

        }

    }
}

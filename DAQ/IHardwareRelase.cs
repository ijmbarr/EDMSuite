using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAQ
{
    public interface IHardwareRelease
    {
        void ReleaseHardware();
        void ReclaimHardware();
    }
}

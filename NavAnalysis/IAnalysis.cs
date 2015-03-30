using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NavAnalysis
{
    public interface IAnalysis
    {
        void ComputeAbsImageFromZip(string zipFile, string img0, string img1);
    }
}

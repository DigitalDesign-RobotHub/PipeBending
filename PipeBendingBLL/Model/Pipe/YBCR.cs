using MathNet.Spatial.Units;
using OCCTK.OCC.gp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace PipeBendingBLL.Model.Pipe;
public readonly struct YBCR {
    public double Y { get; }
    public double B { get; }
    public double C { get; }
    public decimal R { get; }

    public YBCR(double y, double b, double c, decimal r) {
        Y = y; B = b; C = c; R = r;
    }
}

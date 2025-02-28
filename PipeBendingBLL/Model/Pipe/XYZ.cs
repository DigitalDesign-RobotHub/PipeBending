using MathNet.Spatial.Units;
using OCCTK.OCC.gp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace PipeBendingBLL.Model.Pipe;
public readonly struct XYZ {
    public Pnt GpPnt { get; }
    public decimal? Radius { get; }
    public double? Angle { get; }

    public XYZ(Pnt gpPnt, decimal? radius, double? angle) {
        GpPnt = gpPnt;
        Radius = radius;
        Angle = angle;
    }

    // 更新 GpPnt
    public XYZ UpdateGpPnt(Pnt newGpPnt) {
        return new XYZ(newGpPnt, Radius, Angle);
    }

    // 更新 Angle
    public XYZ UpdateAngle(double? newAngle) {
        return new XYZ(GpPnt, Radius, newAngle);
    }

    public override string ToString() => $"XYZ(Pnt: {GpPnt}, Radius: {Radius}, Angle: {Angle})";
}

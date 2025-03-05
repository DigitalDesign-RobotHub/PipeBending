using IMKernel.Utils;

using OCCTK.OCC.gp;

namespace PipeBendingBLL.Model.Pipe;

/// <summary>
/// 用于描述弯管的XYZ数据，包含 点位值、折弯半径、折弯角
/// </summary>
/// <remarks>首、末端点没有折弯半径和折弯角</remarks>
public readonly struct XYZR {
	/// <summary>
	/// XYZ点
	/// </summary>
	public Pnt Pnt { get; }
	/// <summary>
	/// 弯管半径
	/// </summary>
	public decimal Radius { get; }
	/// <summary>
	/// 弯管角度
	/// </summary>
	public RAD Angle { get; }

	public XYZR( Pnt point = default, decimal radius = 0, RAD angle = default ) {
		Pnt = point;
		Radius = radius;
		Angle = angle;
	}

	// 更新 Pnt
	public XYZR UpdatePnt( Pnt newGpPnt ) {
		return new XYZR(newGpPnt, Radius, Angle);
	}

	// 更新 Angle
	public XYZR UpdateRadius( decimal radius ) {
		return new XYZR(Pnt, radius, Angle);
	}

	// 更新 Angle
	public XYZR UpdateAngle( double newAngle ) {
		return new XYZR(Pnt, Radius, newAngle);
	}

	public override string ToString( ) => $"XYZR(点: {Pnt}, 折弯半径: {Radius}, 折弯角: {Angle})";
}

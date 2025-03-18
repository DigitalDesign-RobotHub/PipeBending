using IMKernel.Utils;

using log4net;

using OCCTK.OCC.BRepAdaptor;
using OCCTK.OCC.GCPnts;
using OCCTK.OCC.GeomAbs;
using OCCTK.OCC.gp;
using OCCTK.OCC.Topo;

namespace PipeBendingBLL.Model.Analyzer;

internal class LineEdge {
	private static readonly ILog log = LogManager.GetLogger(typeof(LineEdge));

	public double Length { get; }

	//todo 是否加入构造函数
	public PipeFace? ParentFace { get; set; }

	private readonly TEdge _topoEdge;

	/// <summary>
	/// 隐式转换为 TEdge
	/// </summary>
	public static implicit operator TEdge( LineEdge edge ) => edge._topoEdge;

	public LineEdge( TEdge edge ) {
		this._topoEdge = edge;

		var brepEdgeAdaptor = new Curve(this._topoEdge);
		if( brepEdgeAdaptor.GetType( ) != CurveType.Line ) {
			log.Debug($"入参不是直线: {edge}");
			throw new Exception("入参不是直线");
		}
		var startParam = brepEdgeAdaptor.FirstParameter();
		var endParam = brepEdgeAdaptor.LastParameter();
		this.Length = endParam - startParam;
	}
}
internal class CircleEdge {
	private static readonly ILog log = LogManager.GetLogger(typeof(CircleEdge));

	public PipeFace? ParentFace { get; set; }
	public double CircleRadius { get; }
	public Pnt CircleCenter { get; }

	private readonly TEdge _topoEdge;

	/// <summary>
	/// 隐式转换为 TEdge
	/// </summary>
	public static implicit operator TEdge( CircleEdge edge ) => edge._topoEdge;

	public CircleEdge( TEdge edge ) {
		this._topoEdge = edge;
		var brepEdgeAdaptor = new Curve(this._topoEdge);
		if( brepEdgeAdaptor.GetType( ) == CurveType.Circle ) {
			var circle = brepEdgeAdaptor.Circle();
			this.CircleCenter = circle.Pose.Location;
			this.CircleRadius = circle.Radius;
			return;
		}

		if( brepEdgeAdaptor.GetType( ) == CurveType.BSplineCurve ) {

			var aCurve = new Curve(this._topoEdge);
			var gcUA = new UniformAbscissa(aCurve, 6, -1);
			List<Pnt> left = [aCurve.Value(gcUA.Parameter(1)), aCurve.Value(gcUA.Parameter(2)), aCurve.Value(gcUA.Parameter(3))];
			// 取前三和后三个点进行三点定圆
			var (leftCircleCenter, leftRadius, _) = ShapeHandler.ThreePointFixedCircle(left[0], left[1], left[2]);

#if DEBUG
			// 理论上如果样条曲线是圆弧，则左右两边的圆心和半径应该相等
			List<Pnt> right = [aCurve.Value(gcUA.Parameter(4)), aCurve.Value(gcUA.Parameter(5)), aCurve.Value(gcUA.Parameter(6))];
			var (rightCircleCenter, rightRadius, _) = ShapeHandler.ThreePointFixedCircle(right[0], right[1], right[2]);

			if( leftCircleCenter != rightCircleCenter || Math.Abs(leftRadius - rightRadius) >= 1e-4 ) {
				log.Debug($"!!! 计算出的圆不相等 left|right: {leftCircleCenter}|{rightCircleCenter} || {leftRadius}|{rightRadius}");
			}
#endif
			this.CircleCenter = leftCircleCenter;
			this.CircleRadius = leftRadius;
			return;
		}

		var DebugType= brepEdgeAdaptor.GetType();
		log.Debug($"入参不是圆弧: {edge} | {DebugType}");
		throw new Exception("入参不是圆弧");
	}
}
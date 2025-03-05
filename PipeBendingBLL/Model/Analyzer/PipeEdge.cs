using OCCTK.OCC.BRepAdaptor;
using OCCTK.OCC.GeomAbs;
using OCCTK.OCC.gp;
using OCCTK.OCC.Topo;

namespace PipeBendingBLL.Model.Pipe;
public class PipeEdge {
	private static readonly double LINEAR_TOLERANCE = 1e-2;
	public Pnt Center { get; set; }
	public CurveType Type { get; set; }
	public Pnt CircleCenter { get; set; }
	public double CircleRadius { get; set; }
	public int Index { get; set; }
	public TEdge TopoEdge { get; set; }
	public PipeFace? PipeFace { get; set; }
	public HashSet<PipeEdge> Edges { get; set; }
	public HashSet<Pnt> Points { get; set; }
	public double Length { get; set; }


	public PipeEdge( int index, TEdge topoEdge, PipeFace? pipeFace = null ) {
		this.Index = index;
		this.PipeFace = pipeFace;
		this.TopoEdge = topoEdge;
		this.Edges = new HashSet<PipeEdge>( );
		this.Points = new HashSet<Pnt>( );
		this.Length = 0.0;
		this.FindEdgeData( );
	}

	// 只能处理圆面，如果是方形边线，则无法处理
	public void FindEdgeData( ) {
		var brepEdgeAdaptor = new Curve(this.TopoEdge);
		// 曲线类型
		this.Type = brepEdgeAdaptor.GetType( );
		// 直线参数（直线长度）
		if( this.Type == CurveType.Line ) {
			var startParam = brepEdgeAdaptor.FirstParameter();
			var endParam = brepEdgeAdaptor.LastParameter();
			var curveLength = endParam - startParam;
			this.Length = curveLength;
		} else if( this.Type == CurveType.Circle ) {
			var circle = brepEdgeAdaptor.Circle();
			this.CircleCenter = circle.Pose.Location; // todo 获取圆心坐标
			this.CircleRadius = circle.Radius; // 获取圆半径
			this.Length = new GCPnts_AbscissaPoint.Length(brepEdgeAdaptor) // todo 曲线长度

		} else if( this.Type == CurveType.BSplineCurve ) {
			var aCurve = new Curve(this.TopoEdge);
			var gcUA = new GCPnts_UniformAbscissa(aCurve, 6, -1); // todo
			var left = [aCurve.Value(gcUA..Parameter(1)), aCurve.Value(gcUA.Parameter(2)), aCurve.Value(gcUA.Parameter(3))];
			var right = [aCurve.Value(gcUA..Parameter(4)), aCurve.Value(gcUA.Parameter(5)), aCurve.Value(gcUA.Parameter(6))];
			// 取前三和后三个点进行三点定圆
			var leftCircleCenter, leftRadius, _ = makeCircleByThreePoints(left[0], left[1], left[2]); // todo 
			var rightCircleCenter, rightRadius, _ = makeCircleByThreePoints(right[0], right[1], right[2]); // todo

			// 需要更新
			if( Math.Abs(leftRadius - rightRadius) < LINEAR_TOLERANCE ) {
				this.CircleCenter = new Pnt(leftCircleCenter);
				this.CircleRadius = leftRadius;
			}
		}
	}
}

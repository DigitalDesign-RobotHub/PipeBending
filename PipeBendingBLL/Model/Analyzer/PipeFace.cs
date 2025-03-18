using log4net;

using OCCTK.OCC.BRepAdaptor;
using OCCTK.OCC.GeomAbs;
using OCCTK.OCC.gp;
using OCCTK.OCC.Topo;

namespace PipeBendingBLL.Model.Analyzer;
internal class PipeFace {
	private static readonly ILog log = LogManager.GetLogger(typeof(PipeFace));
	//private static readonly double LINEAR_TOLERANCE = 1e-2;

	public SurfaceType Type { get; set; }
	public HashSet<TEdge> ChildEdges { get; } = [ ];
	/// <summary>
	/// 邻面需要在对象外部添加
	/// </summary>
	public HashSet<PipeFace> AdjacentPFaces { get; } = [ ];
	public List<Pnt> CenterPoints { get; } = [ ];
	private readonly TFace _topoShape;
	/// <summary>
	/// 隐式转换为 TShape
	/// </summary>
	public static implicit operator TShape( PipeFace face ) => face._topoShape;
	public PipeFace( TFace face ) {
		this._topoShape = face;
		this.Type = SurfaceType.OtherSurface;
		GetFaceData( );
	}

	private void GetFaceData( ) {
		Surface brepFaceAdaptor = new (this._topoShape);
		this.Type = brepFaceAdaptor.GetType( );

		#region 获取所有的子边
		//Explorer explorer =new(this,ShapeEnum.EDGE);
		//foreach( var item in explorer ) {
		//	var edge=item.AsEdge();
		//	var brepEdgeAdaptor = new Curve(edge);
		//	if( brepEdgeAdaptor.GetType( ) == CurveType.Line ) {
		//		//LineEdge pEdge = new(edge) {
		//		//	ParentFace = this
		//		//};
		//		//this.ChildEdges.Add(pEdge);
		//	}
		//	if( brepEdgeAdaptor.GetType( ) == CurveType.Circle || brepEdgeAdaptor.GetType( ) == CurveType.BSplineCurve ) {
		//		//CircleEdge pEdge = new(edge) {
		//		//	ParentFace = this
		//		//};
		//		//this.ChildEdges.Add(pEdge);
		//	}
		//}
		#endregion

		#region 找 CenterPoints
		//if( this.Type == SurfaceType.Torus ) {
		//	//++ 通过 UV 计算中心点
		//	// 通用的V
		//	var firstV = brepFaceAdaptor.FirstVParameter();
		//	var lastV = brepFaceAdaptor.LastVParameter();
		//	var addV = lastV - firstV;
		//	// 不能让取到的几个点过于接近
		//	var v1 = firstV;
		//	var v2 = firstV + addV / 3;
		//	var v3 = firstV + addV * 2 / 3;
		//	//+ 第一个点
		//	// U 表示大环
		//	var firstU = brepFaceAdaptor.FirstUParameter();
		//	// UV转换成点
		//	var circle1P1 = brepFaceAdaptor.Value(firstU, v1);
		//	var circle1P2 = brepFaceAdaptor.Value(firstU, v2);
		//	var circle1P3 = brepFaceAdaptor.Value(firstU, v3);
		//	//定圆1
		//	var (centerPoint1, _, _) = ShapeHandler.ThreePointFixedCircle(circle1P1, circle1P2, circle1P3);
		//	//+ 第二个点
		//	// U 表示大环
		//	var lastU = brepFaceAdaptor.LastUParameter();
		//	// UV转换成点
		//	circle1P1 = brepFaceAdaptor.Value(lastU, v1);
		//	circle1P2 = brepFaceAdaptor.Value(lastU, v2);
		//	circle1P3 = brepFaceAdaptor.Value(lastU, v3);
		//	//定圆2
		//	var (centerPoint2, _, _) = ShapeHandler.ThreePointFixedCircle(circle1P1, circle1P2, circle1P3);
		//	//! 圆环有两个中心点
		//	this.CenterPoints.Add(centerPoint1);
		//	this.CenterPoints.Add(centerPoint2);
		//	log.Info($"环面 {GetHashCode( )} 类型 {this.Type} 中心点 {this.CenterPoints.Count} 个");
		//} else {
		//	//++ 圆柱面直接取两个中心点
		//	foreach( var circle in this.ChildEdges.OfType<CircleEdge>( ) ) {
		//		//排除相同的点
		//		if( !this.CenterPoints.Any(p => circle.CircleCenter == p) ) {
		//			this.CenterPoints.Append(circle.CircleCenter);
		//		}
		//	}
		//	log.Info($"圆柱面 {GetHashCode( )} 类型 {this.Type} 中心点 {this.CenterPoints.Count} 个");
		//}
		if( this.CenterPoints.Count == 0 )
			log.Warn($"面 {GetHashCode( )} 类型 {this.Type} 未找到中心点");

		#endregion
	}
}

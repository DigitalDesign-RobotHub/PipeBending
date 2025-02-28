//using log4net;
//using OCCTK.OCC.BRepAdaptor;
//using OCCTK.OCC.GeomAbs;
//using OCCTK.OCC.gp;
//using OCCTK.OCC.Topo;
//using PipeBendingBLL.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PipeBendingBLL.Model.Pipe;
//public class PipeFace {
//    private static double LINEAR_TOLERANCE = 1e-2;
//    private static readonly ILog log = LogManager.GetLogger(typeof(PipeFace));
//    public SurfaceType Type { get; set; }
//    public Pnt OriginPoint { get; set; }
//    public int Index { get; set; }
//    public PipeShape? PipeShape { get; set; }
//    public TFace TopoFace { get; set; }
//    public HashSet<PipeEdge> PipeEdges { get; set; }
//    public HashSet<PipeFace> AdjacentPFaces { get; set; }
//    public List<Pnt> CenterPoints { get; set; }

//    public PipeFace(int index, TFace topoShape, PipeShape? pipeShape = null) {
//        this.Index = index;
//        this.PipeShape = pipeShape;
//        this.TopoFace = topoShape;
//        this.PipeEdges = new HashSet<PipeEdge>();
//        this.CenterPoints = new List<Pnt>();
//    }

//    public override string ToString() {
//        return $"{this.Index}";
//    }

//    public PipeShape? GetFaceData() {
//        var brepFaceAdaptor = new Surface(this.TopoFace);
//        this.Type = brepFaceAdaptor.GetType();
//        if (this.Type == SurfaceType.Torus) {
//            // 通过 UV 计算中心点
//            // 第一个点
//            var firstU = brepFaceAdaptor.FirstUParameter(); // U 表示大环
//            var firstV = brepFaceAdaptor.FirstVParameter();
//            var lastV = brepFaceAdaptor.LastVParameter();
//            var addV = lastV - firstV;
//            // 不能让取到的几个点过于接近
//            var v1 = firstV;
//            var v2 = firstV + addV / 3;
//            var v3 = firstV + addV * 2 / 3;
//            var circle1P1 = brepFaceAdaptor.Value(firstU, v1);
//            var circle1P2 = brepFaceAdaptor.Value(firstU, v2);
//            var circle1P3 = brepFaceAdaptor.Value(firstU, v3);
//            // todo 三点定圆方法找不到
//            var circleCenter = MakeCircleByThreePoints(circle1P1, circle1P2, circle1P3);
//            var centerPoint1 = new Pnt(circleCenter.X, circleCenter.Y, circleCenter.Z);

//            // 第二个点
//            var lastU = brepFaceAdaptor.LastUParameter();   // U 表示大环
//            circle1P1 = brepFaceAdaptor.Value(lastU, v1);
//            circle1P2 = brepFaceAdaptor.Value(lastU, v2);
//            circle1P3 = brepFaceAdaptor.Value(lastU, v3);
//            // todo 三点定圆方法找不到
//            circleCenter = MakeCircleByThreePoints(circle1P1, circle1P2, circle1P3);
//            var centerPoint2 = new Pnt(circleCenter.X, circleCenter.Y, circleCenter.Z);

//            this.CenterPoints.Add(centerPoint1);
//            this.CenterPoints.Add(centerPoint2);
//        }
//        // todo 
//        else {
//            // 找 centerPoints
//            foreach (var aEdge in this.PipeEdges) {
//                if (aEdge.GetType().GetProperty("CircleCenter") != null
//                    && !this.CenterPoints.Any(p => aEdge.CircleCenter.Equals(p, LINEAR_TOLERANCE))) {
//                    this.CenterPoints.Append(aEdge.CircleCenter);
//                }
//                log.Debug($"{aEdge.Type.ToString()}");
//            }
//            log.Info($"面 {this.Index} 类型 {this.Type.ToString()} 中心点 {this.CenterPoints.Count} 个");
//        }
//        if (this.CenterPoints == null || this.CenterPoints.Count == 0) {
//            log.Warn($"面 {this.Index} 类型 {this.Type.ToString()} 未找到中心点");
//            return this.PipeShape;
//        }
//        return null;
//    }
//}

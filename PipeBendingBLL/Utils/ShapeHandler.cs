using IMKernel.OCCExtension.GeometryTool;
using log4net;
using log4net.Repository.Hierarchy;
using MathNet.Numerics;
using OCCTK.OCC.AIS;
using OCCTK.OCC.BRep;
using OCCTK.OCC.BRepBuilderAPI;
using OCCTK.OCC.BRepOffsetAPI;
using OCCTK.OCC.gp;
using OCCTK.OCC.TopExp;
using OCCTK.OCC.Topo;
using OCCTK.OCC.TopoAbs;
using PipeBendingBLL.Model.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeBendingBLL.Utils;
public static class ShapeHandler {
    private static readonly ILog log = LogManager.GetLogger(typeof(ShapeHandler));
    private static double TOLERANCE = 1e-6;

    /// <summary>
    /// 创建一个圆面
    /// </summary>
    /// <param name="radius">圆面的半径</param>
    /// <param name="Center">（可选）圆的圆心，默认为原点</param>
    /// <param name="normal">（可选）圆的法向量，默认为X轴</param>
    /// <param name="onlyPlane">（可选）是否为平面，默认为true</param>
    /// <returns>个圆面</returns>
    public static TShape MakeCircle(decimal radius, Pnt? center = null, Dir? normal = null) {
        #region 参数初始化和参数验证
        if (radius <= 0) {
            log.Error("创建圆面失败，圆面半径必须大于0。");
            throw new ArgumentException("创建圆面失败，圆面半径必须大于0。");
        }

        center ??= new Pnt(0, 0, 0);  // 默认圆心为 (0,0,0)
        normal ??= new Dir(1, 0, 0);   // 默认法向量为 X 轴方向 (1,0,0)

        if (normal.Equals(new Dir(0, 0, 0))) {
            log.Error("创建圆面失败，法向量不能为0。");
            throw new ArgumentException("创建圆面失败，法向量不能为0。");
        }
        #endregion

        #region 生成 Edge
        var circlePlane = new Ax2(center.Value, normal.Value);  // 创建 Ax2 坐标系（圆心 & 法向量）
        var circle = new Circle(circlePlane, (double)radius);
        var edge = new MakeEdge(circle).Edge();
        if (edge == null) {
            log.Error("创建圆面失败，创建圆的edge失败。");
            throw new InvalidOperationException("创建圆面失败，创建圆的edge失败。");
        }
        #endregion

        #region 生成 Wire
        var wireMaker = new MakeWire(edge);
        var wire = wireMaker.Wire();
        if (wire == null) {
            log.Error("创建圆面失败，创建圆的wire失败。");
            throw new InvalidOperationException("创建圆面失败，创建圆的wire失败。");
        }
        #endregion

        #region 生成 Face
        var faceMaker = new MakeFace(wire);  // todo face_1 = BRepBuilderAPI_MakeFace(wire_1, True).Shape()  onlyPlane=true 是否需要
        var face = faceMaker.Shape();
        if (face == null) {
            log.Error("创建圆面失败，创建圆的face失败。");
            throw new InvalidOperationException("创建圆面失败，创建圆的face失败");
        }
        #endregion

        return face;
    }

    public static TWire MergeTwoEdges(TWire firstShape, TWire secondShape, double LinearTolerance = 1) {
        var mkWire = new MakeWire();
        mkWire.Add(firstShape);
        mkWire.Add(secondShape);

        if (!mkWire.IsDone) {
            log.Warn("合并失败");
            // 合并失败的额外处理
            var firstShapeExplorer = new Explorer(firstShape, ShapeEnum.VERTEX);
            bool isOk = false;
            // 遍历第一个边的顶点
            while (firstShapeExplorer.More()) {
                var vertex1 = firstShapeExplorer.Current();
                var p1 = Tool.Pnt((TVertex)vertex1);
                Debug.WriteLine($"p1: {p1.X:F4}, {p1.Y:F4}, {p1.Z:F4}");
                firstShapeExplorer.Next();
                var secondShapeExplorer = new Explorer(secondShape, ShapeEnum.VERTEX);
                while (secondShapeExplorer.More()) {
                    var vertex2 = secondShapeExplorer.Current();    // 遍历得到每一个点
                    var p2 = Tool.Pnt((TVertex)vertex2);
                    log.Warn($"p2-p1:{(p2.X - p1.X):F4}, {(p2.Y - p1.Y):F4}, {(p2.Z - p1.Z):F4}");
                    Debug.WriteLine($"p2 = gp_Pnt({p2.X:F4},{p2.Y:F4},{p2.Z:F4})");
                    secondShapeExplorer.Next();
                    // 检查两个顶点是否相等
                    if (p1.Equals(p2, LinearTolerance)) {
                        log.Warn($"p1:{p1.X:F4},{p1.Y:F4},{p1.Z:F4}, p2:{p2.X:F4},{p2.Y:F4},{p2.Z:F4}");
                    }
                    // 如果相等，进行平移变换
                    var theTransformation = new Trsf(new Vec(p2,p1));
                    var myBrepTransformationShape = new Transform(secondShape, theTransformation).Shape();
                    secondShape = (TWire)myBrepTransformationShape;
                    isOk = true;
                    break;
                }
                if (isOk) {
                    break;
                }
            }
        }
        // 再次将两个边添加到一个新的线
        var mkWireT = new MakeWire();
        mkWireT.Add(firstShape);
        mkWire.Add(secondShape);

        // 返回最终合并后的线
        return mkWireT.Wire();
    }

    // todo 假设了关键的R均一致的前提（误差范围内）
    public static TShape MakePipeFromYBCRList(List<YBCR> ybcrList, TShape crossSectionShape) {
        TWire previousWire = null;
        var edgeStartPnt = Pnt.Default;    // 从原点开始构建
        var direction = new Vec(1, 0, 0); // 从原点的 X 方向开始构建
        var rotationAxis = new Ax1(Pnt.Default, new Dir(0, 0, 1));    // 初始旋转轴，方向为 Z 轴
        decimal tempR = 0;  // 当前的R值
        // 首先构建出全部的曲线
        foreach (var ybcr in ybcrList) {
            if (ybcrList.IndexOf(ybcr) < ybcrList.Count() - 1) {
                // Y 直线
                
                // 移动线段长度
                direction = direction.Normalized();
                direction = direction.Multiplied(ybcr.Y);
                var yMove = new Trsf(direction);
                var edgeEndPnt = edgeStartPnt.Transformed(yMove);
                // direction，作为下一段的起点
                direction = direction.Transformed(yMove);
                // rotation_axis 同步移动
                rotationAxis = rotationAxis.Transformed(yMove);
                // 构建直线
                TWire yWire = new MakeWire(new MakeEdge(edgeStartPnt, edgeEndPnt));
                // 保存直线
                if (previousWire != null) {
                    previousWire = MergeTwoEdges(previousWire, yWire);
                }
                else {
                    previousWire = yWire;
                }
                // 先不更新 edge_start_pnt，计算完 BC 再更新
                //  BC 圆弧和 R
                // 先根据 R 往外平移 rotation_axis
                // 一般一个管子的 R 均一致，用来处理中间 R 变化的情况
                if (tempR != ybcr.R) {
                    tempR = ybcr.R;
                    var axisTranslate = Trsf.Default;
                    if (edgeEndPnt.Distance(rotationAxis.Location) <= TOLERANCE) {
                        axisTranslate=new Trsf(new Vec(0, (double)tempR, 0));
                    }
                    else {
                        var aVec = new Vec(edgeEndPnt, rotationAxis.Location);
                        aVec = aVec.Normalized().Multiplied((double)(tempR - ybcr.R));  // 移动的距离
                        axisTranslate = new Trsf(aVec);
                    }
                    rotationAxis = rotationAxis.Transformed(axisTranslate);
                }
                if (ybcr.B != 0.0) {
                    // 根据 B 旋转（符合右手定则，因此旋转轴均在前一段直线的上方 (第一象限))
                    // 绕着直线旋转 rotation_axis
                    var rtAixAxis = new Ax1(edgeEndPnt, new Dir(new Vec(edgeStartPnt, edgeEndPnt)));  // 旋转轴的旋转轴 (Y 段的直线)
                    var bRota = new Trsf(new Quat(rtAixAxis, ybcr.B));
                    rotationAxis = rotationAxis.Transformed(bRota);
                    direction = direction.Transformed(bRota);
                }
                // 根据 C 构建圆弧
                var cRota = new Trsf(new Quat(rotationAxis, ybcr.C));
                // 构建圆弧
                edgeStartPnt = edgeStartPnt;    //上一段的终点为这段的起点
                edgeEndPnt = edgeStartPnt.Transformed(cRota);
                direction = direction.Transformed(cRota);

                var aCircle = new Circle(new Ax2(rotationAxis.Location, rotationAxis.Direction), (double)tempR);
                TWire bcrCircle = new MakeWire(new MakeEdge(aCircle, edgeStartPnt, edgeEndPnt));

                // 保存圆弧
                if (previousWire != null) {
                    previousWire = MergeTwoEdges(previousWire, bcrCircle);
                }
                else {
                    throw new InvalidOperationException("不能以圆弧段作为开始");
                }
                edgeStartPnt = edgeEndPnt; // 这一段的终点为下段的起点
            }
            else {
                // 最后一段只有 Y 直线
                direction = direction.Normalized().Multiplied(ybcr.Y);
                var yMove = new Trsf(direction);
                var edgeEndPnt = edgeStartPnt.Transformed(yMove);
                // 构建直线
                TWire yWire = new MakeWire(new MakeEdge(edgeStartPnt, edgeEndPnt));
                // 保存直线
                if (previousWire != null) {
                    previousWire = MergeTwoEdges(previousWire, yWire);
                }
                else {
                    log.Info("只有一段直管");
                    // 得到线段长度
                    direction = direction.Normalized().Multiplied(ybcr.Y);
                    yMove = new Trsf(direction);
                    edgeEndPnt = edgeStartPnt.Transformed(yMove);
                    var anEdge = new MakeEdge(edgeStartPnt, edgeEndPnt).Edge();
                    previousWire = new MakeWire(anEdge).Wire();
                    break;
                }
            }
        }
        return new MakePipe(previousWire, crossSectionShape).Shape();
    }
}

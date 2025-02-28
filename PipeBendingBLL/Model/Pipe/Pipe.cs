using IMKernel.Model;
using OCCTK.OCC.AIS;
using OCCTK.OCC.gp;
using OCCTK.OCC.Topo;
using PipeBendingBLL.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PipeBendingBLL.Model.Pipe;
public class Pipe {
    /// <summary>
    /// 管件名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 管件存在的源路径
    /// </summary>
    public string FilePath { get; set; }
    /// <summary>
    /// 管件的类型
    /// </summary>
    private PipeType type;
    public PipeType Type {
        get {
            if (type == PipeType.Unknown) {
                if (this.YBCRList.Count == 1 && this.YBCRList.First().B == 0 && this.YBCRList.First().C == 0) {
                    return PipeType.Straight;
                }
                else {
                    return PipeType.Circle;
                }

            }
            else {
                return type;
            }
        }
        set { type = value; }
    }
    /// <summary>
    /// 管件对应的原料
    /// </summary>
    public string Material { get; set; }    // todo 是为管件排料匹配做的，检查是否有存在的必要
    /// <summary>
    /// 管件对应的加工材料
    /// </summary>
    public string ProcessingMaterial { get; set; }    // todo 需要进一步确认使用情况
    /// <summary>
    /// 管件截面的形状
    /// </summary>
    public TShape CrossSectionShape { get => ShapeHandler.MakeCircle(OuterRadius * 0.8M); }   // 为了干涉计算的方便，截面尺寸比实际管径小 20%
    /// <summary>
    /// 管件的直线段
    /// </summary>
    public OrderedDictionary WireDict { get; set; }   // todo 需要进一步确认使用情况,set里面设置了length为100
    /// <summary>
    /// 管件的中心线展开长度
    /// </summary>
    public double FitLength { get; set; }
    /// <summary>
    /// 管件的长度
    /// </summary>
    public double Length { get; set; } // todo 后续调整全部转移到 FitLength 上
    /// <summary>
    /// 管件的首段管接头
    /// </summary>
    public string FirstFitting { get; set; } // todo 未使用，后续删除
    /// <summary>
    /// 管件的末段管接头
    /// </summary>
    public string LastFitting { get; set; } // todo 未使用，后续删除
    /// <summary>
    /// 管件的内半径
    /// </summary>
    // todo 是否可以改成readonly
    public decimal InnerRadius {
        get {
            if (Type == PipeType.Circle) {
                return OuterRadius - Thickness;
            }
            else {
                return 0.0M;
            }
        }
    }
    /// <summary>
    /// 管件的外半径
    /// </summary>
    public decimal OuterRadius { get; set; }
    /// <summary>
    /// 管径的外直径
    /// </summary>
    /// <remarks>Caliber</remarks>
    public decimal Diameter => OuterRadius * 2;
    /// <summary>
    /// 管径的厚度
    /// </summary>
    public decimal Thickness { get; set; }
    /// <summary>
    /// 夹持长度
    /// </summary>
    public double ClampLength { get; set; }
    /// <summary>
    /// 原始的step模型
    /// </summary>
    public TShape OriginStepModel { get; set; }
    /// <summary>
    /// 管件的最终shape
    /// </summary>
    private TShape shape;
    public TShape Shape {
        get {
            if (shape != null) {
                return shape;
            }
            else {
                return ShapeHandler.MakePipeFromYBCRList(YBCRList, CrossSectionShape);
            }
        }
        set { shape = value; }
    }
    /// <summary>
    /// 首直线段长度
    /// </summary>
    public double FirstSegLength { get => this.YBCRList.Count > 1 ? this.YBCRList.First().Y : 0.0; }
    /// <summary>
    /// 最短中间直线段长
    /// </summary>
    public double MinMidSegLength { get => this.YBCRList.Count > 2 ? this.YBCRList.Skip(1).Take(YBCRList.Count - 2).Min(i => i.Y) : 0.0; }
    /// <summary>
    /// 末直线段长度
    /// </summary>
    public double LastSegLength { get => this.YBCRList.Count > 1 ? this.YBCRList.Last().Y : 0.0; }
    /// <summary>
    /// 折弯次数
    /// </summary>
    public int BendingNum { get => this.YBCRList.Count > 1 ? this.YBCRList.Count - 1 : 0; }
    /// <summary>
    /// 折弯半径
    /// </summary>
    public List<decimal> BendingRadius {
        get {
            var bendingRadiusList = new HashSet<decimal>();
            foreach (var oneXYZ in XYZList) {
                if (oneXYZ.Radius != null) {
                    bendingRadiusList.Add((decimal)oneXYZ.Radius);
                }
            }
            return bendingRadiusList.ToList();
        }
    }
    /// <summary>
    /// 首端二次切割长度/工艺加长量
    /// </summary>
    public double FirstLengthening { get; set; }
    /// <summary>
    /// 末端二次切割长度/工艺加长量
    /// </summary>
    public double LastLengthening { get; set; }
    /// <summary>
    /// 将解析的管件移动到原点的变换
    /// </summary>
    public Trsf Transform { get; set; }
    /// <summary>
    /// 最终的XYZ列表
    /// </summary>
    public List<XYZ> XYZList { get; set; }  // todo 弄清楚这个XYZ存储的是什么
    /// <summary>
    /// 原始的XYZ列表
    /// </summary>
    public List<XYZ> OriginXYZList { get; set; }
    /// <summary>
    /// XYZ补偿值的列表
    /// </summary>
    public List<XYZ> XtYtZtList { get; set; }
    /// <summary>
    /// 最终的YBCR列表
    /// </summary>
    public List<YBCR> YBCRList { get; set; }  // todo 弄清楚这个YBCR存储的是什么
    /// <summary>
    /// 原始的YBCR列表
    /// </summary>
    public List<YBCR> OriginYBCRList { get; set; }  // todo 弄清楚这个YBCR存储的是什么
    /// <summary>
    /// YBCR补偿值的列表
    /// </summary>
    public List<YBCR> YtBtCtRtList { get; set; }  // todo 弄清楚这个YBCR存储的是什么
    /// <summary>
    /// 端到端长度
    /// </summary>
    public double EndToEndLength {
        get {
            if (XYZList.Count > 0) {
                var firstPnt = XYZList.FirstOrDefault().GpPnt;
                var lastPnt = XYZList.LastOrDefault().GpPnt;

                var distance = firstPnt.Distance(lastPnt);
                return distance;
            }
            return 0.0;
        }
    }


    public Pipe(string name, PipeType pipeType, double clampLength, TShape? stepModel) {
        Name = name;
        FilePath = "";
        Type = pipeType;
        //this.CrossSectionShape = null;
        ProcessingMaterial = null;
        WireDict = new OrderedDictionary();
        Material = "";
        FitLength = 0.0;
        Length = 0.0;
        FirstFitting = "";
        LastFitting = "";
        //this.InnerRadius = 0.0;
        OuterRadius = 0.0M;
        Thickness = 0.0M;
        ClampLength = clampLength;
        OriginStepModel = stepModel;
        Shape = stepModel;
        //FirstSegLength = 0.0;
        //MinMidSegLength = 0.0;
        //LastSegLength = 0.0;
        //BendingNum = 0;
        //this.BendingRadius = 0.0M;
        FirstLengthening = 0.0;
        LastLengthening = 0.0;
        Transform = new Trsf();
        XYZList = new List<XYZ>();
        OriginXYZList = new List<XYZ>();
        XtYtZtList = new List<XYZ>();
        YBCRList = new List<YBCR>();
        OriginYBCRList = new List<YBCR>();
        YtBtCtRtList = new List<YBCR>();
    }

    // todo 对应原来的_repr_方法
    public override string ToString() { return ""; }

    // 管件首段的方向始终沿着 X 方向，第二段折弯方向一定符合右手定则，旋转轴平行于 Z 轴正方向
    private void Reverse() {
        XYZList.Reverse();
        ToStandardPipe(false);
    }

    /// <summary>
    /// 将 XYZ 坐标变为标准情况下的坐标
    /// </summary>
    /// <param name="needStandardise"> 是否进行标准化（让最后一段长度大于第一段）</param>
    private void ToStandardPipe(bool needStandardise = true) {
        // todo YBCRList和XYZlist为空的处理
        if (YBCRList == null || !YBCRList.Any()) {
            GenerateYBCRListByXYZList();
        }
        if (needStandardise && YBCRList.First().Y > YBCRList.Last().Y) {
            XYZList.Reverse();
            YBCRList = new List<YBCR>();
        }

        // 把 XYZ 移动到原点
        var firstXYZPntTrsf = new Trsf();
        var firstGpPnt = XYZList.First().GpPnt;
        var secondGpPnt = XYZList[1].GpPnt;

        firstXYZPntTrsf=new Trsf(new Vec(firstGpPnt, Pnt.Default));
        for (int i = 0; i < XYZList.Count; i++) {
            var xyz = XYZList[i];
            // 使用索引更新列表中的元素
            XYZList[i] = xyz.UpdateGpPnt(xyz.GpPnt.Transformed(firstXYZPntTrsf));
        }

        // 把第二个点的方向挪动到 X 轴上
        var firstToSecondPntDistance = firstGpPnt.Distance(secondGpPnt);    // 第一第二点间距离
        var pntOnX = new Pnt(firstToSecondPntDistance, 0, 0);   // 作等腰三角形
        // 当等腰三角形的两个点距离大于容差，认为不重叠
        if (secondGpPnt.Distance(pntOnX) > 0.1) {
            var secondQuat = new Quat(new Vec(firstGpPnt, secondGpPnt), new Vec(1, 0, 0));
            var secondXYZPntTrsf = new Trsf(secondQuat);
            for (int i = 0; i < XYZList.Count; i++) {
                var xyz = XYZList[i];
                // 使用索引更新列表中的元素
                XYZList[i] = xyz.UpdateGpPnt(xyz.GpPnt.Transformed(secondXYZPntTrsf));
            }
        }

        // 把第三个点旋转到 xy 平面第一象限
        if (XYZList.Count > 2) {
            var thirdGpPnt = XYZList[2].GpPnt;

            var fromVec = new Vec( Pnt.Default, thirdGpPnt);
            var angeleValue = -Math.Atan2(fromVec.Z, fromVec.Y);
            var thirdQuat = new Quat(new Vec(1, 0, 0), angeleValue);
            var thirdXYZPntTrsf = new Trsf(thirdQuat);
            for (int i = 0; i < XYZList.Count; i++) {
                var xyz = XYZList[i];
                // 使用索引更新列表中的元素
                XYZList[i] = xyz.UpdateGpPnt(xyz.GpPnt.Transformed(thirdXYZPntTrsf));
            }
        }

        GenerateYBCRListByXYZList();
    }

    /// <summary>
    /// 根据 XYZList 构造 YBCRList (YBC 比 XYZ 少一个点)
    /// </summary>
    private void GenerateYBCRListByXYZList() {
        YBCRList.Clear();
        bool firstB = false;

        var lastPnt =  Pnt.Default;
        Vec? lastVec = null;

        for (int i = 0; i < XYZList.Count; i++) {
            var xyz = XYZList[i];

            // 第一个 XYZ 点，没有值
            if (i == 0) {
                lastPnt = xyz.GpPnt;
                lastVec = null;
            }
            // 最后一个点 没有 BCR 值
            else if (i == XYZList.Count - 1) {
                var lastXYZ = XYZList[i - 1];
                // todo minusY 的计算逻辑可以抽成一个方法
                var minusY = (double)lastXYZ.Radius * Math.Tan((double)(lastXYZ.Angle / 2));
                var paraY = xyz.GpPnt.Distance(lastPnt) - minusY;
                YBCRList.Append(new YBCR(paraY, 0.0, 0.0, 0.0M));
            }
            // 中间点有 B 值
            else {
                var lastXYZ = XYZList[i - 1];
                double paraY = 0.0;
                double paraB = 0.0;
                double paraC = 0.0;
                decimal paraR = 0.0M;

                if (i == 1) {
                    paraY = xyz.GpPnt.Distance(lastPnt) - (double)xyz.Radius * Math.Tan((double)xyz.Angle / 2);
                }
                else {
                    paraY = xyz.GpPnt.Distance(lastPnt) - (double)xyz.Radius * Math.Tan((double)xyz.Angle / 2) - (double)lastXYZ.Radius * Math.Tan((double)lastXYZ.Angle / 2);
                }
                // 求解 B 
                var nextPnt = XYZList[i + 1].GpPnt;
                var vec1 = new Vec(lastPnt, xyz.GpPnt);
                var vec2 = new Vec(xyz.GpPnt, nextPnt);
                var aVec = vec1.Crossed(vec2);

                if (lastVec != null) {
                    // 旋转的方向需要判断正负
                    paraB = aVec.AngleWithRef((Vec)lastVec, new Vec(xyz.GpPnt, lastPnt));
                    // 根据第一个 B 绕 X 轴旋转
                    if (firstB == null) {
                        var rotaX = new Trsf(new Quat(paraB, 0, 0, EulerSequence.Extrinsic_XYZ));
                        Transform = Transform.Multiplied(rotaX);  // todo 为什么会在这里更新 this.Transform 
                        firstB = true;
                    }
                }
                else {
                    // 第一个点 B 为 0
                    paraB = 0.0;
                }
                // 求C
                if (xyz.Angle != null) {
                    paraC = (double)xyz.Angle;
                }
                else {
                    var v1 = new Vec(xyz.GpPnt, XYZList[i - 1].GpPnt);
                    var v2 = new Vec(xyz.GpPnt, XYZList[i + 1].GpPnt);
                    xyz = xyz.UpdateAngle(Math.PI - v1.Angle(v2));
                    paraC = (double)xyz.Angle;
                }
                paraR = (decimal)xyz.Radius;
                YBCRList.Append(new YBCR(paraY, paraB, paraC, paraR));
                lastPnt = xyz.GpPnt;
                lastVec = aVec;
            }
        }
    }

    private void GetPipeInfo() {

    }
}

using IMKernel.Utils;

using OCCTK.OCC.gp;
using OCCTK.OCC.Topo;

using PipeBendingBLL.Utils;

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
			if( type == PipeType.Unknown ) {
				if( this.YBCR.Count == 1 && this.YBCR.First( ).B == 0 && this.YBCR.First( ).C == 0 ) {
					return PipeType.Straight;
				} else {
					return PipeType.Circle;
				}

			} else {
				return type;
			}
		}
		set { type = value; }
	}
	#region ReadOnly

	/// <summary>
	/// 管件的内半径
	/// </summary>
	public decimal InnerRadius {
		get {
			if( Type == PipeType.Circle ) {
				return OuterRadius - Thickness;
			} else {
				return 0.0M;
			}
		}
	}

	/// <summary>
	/// 管径的外直径
	/// </summary>
	/// <remarks>管径 (Caliber)</remarks>
	public decimal Diameter => OuterRadius * 2;

	/// <summary>
	/// 折弯次数
	/// </summary>
	public int BendingNum { get => this.YBCR.Count > 1 ? this.YBCR.Count - 1 : 0; }

	public TShape GetPipeShape( ) {
		//! 最终管件的管件为实际管径的 80%
		return ShapeHandler.MakePipeFromYBCRList(YBCR, ShapeHandler.MakeCircle(OuterRadius * 0.8M));
	}

	#endregion

	/// <summary>
	/// 管径的厚度
	/// </summary>
	public decimal Thickness { get; set; }

	/// <summary>
	/// 管件的外半径
	/// </summary>
	public decimal OuterRadius { get; set; }

	/// <summary>
	/// XYZ列表
	/// </summary>
	/// <remarks>转换后的 标准XYZ 列表 !!! 需要在业务逻辑中处理XYZ和YBC列表的相互转换 !!!</remarks>
	public List<XYZR> XYZ => YBCR2XYZR(YBCR);

	/// <summary>
	/// YBCR列表
	/// </summary>
	/// <remarks>管件的唯一描述</remarks>
	public List<YBCR> YBCR { get; set; }
	/// <summary>
	/// 原始的YBCR列表
	/// </summary>
	public List<YBCR> OriginYBCRList { get; set; }  // todo 弄清楚这个YBCR存储的是什么



	public Pipe( string name, PipeType pipeType, double clampLength, TShape? stepModel ) {
		Name = name;
		FilePath = "";
		Type = pipeType;
		//this.CrossSectionShape = null;
		//ProcessingMaterial = null;
		//WireDict = new OrderedDictionary( );
		//Material = "";
		//FitLength = 0.0;
		//Length = 0.0;
		//FirstFitting = "";
		//LastFitting = "";
		//this.InnerRadius = 0.0;
		OuterRadius = 0.0M;
		Thickness = 0.0M;
		//ClampLength = clampLength;
		YBCR = new List<YBCR>( );
		OriginYBCRList = new List<YBCR>( );
		//OriginStepModel = stepModel;
		//Shape = stepModel;
		//FirstSegLength = 0.0;
		//MinMidSegLength = 0.0;
		//LastSegLength = 0.0;
		//BendingNum = 0;
		//this.BendingRadius = 0.0M;
		//FirstLengthening = 0.0;
		//LastLengthening = 0.0;
		//Transform = new Trsf( );
		//XYZ = new List<XYZR>( );
		//OriginXYZList = new List<XYZR>( );
		//XtYtZtList = new List<XYZR>( );
		//YtBtCtRtList = new List<YBCR>( );
	}

	// todo 对应原来的_repr_方法
	public override string ToString( ) { return ""; }

	//// 管件首段的方向始终沿着 X 方向，第二段折弯方向一定符合右手定则，旋转轴平行于 Z 轴正方向
	//private void Reverse( ) {
	//	XYZ.Reverse( );
	//	ToStandardPipe(false);
	//}

	/// <summary>
	/// 将 XYZR 坐标变为标准情况下的坐标
	/// </summary>
	private static List<XYZR> ToStandardPipe( List<XYZR> xyzrList ) {
		#region 错误处理
		if( xyzrList.Count == 0 ) {
			throw new Exception("输入的列表为空");
		}
		if( xyzrList.Count == 1 ) {
			throw new Exception("XYZ列表长度至少为2个点");
		}
		#endregion
		List<XYZR>result=[..xyzrList];
		List<YBCR> tempYBCR=XYZR2YBCR(xyzrList );

		if( tempYBCR.First( ).Y > tempYBCR.Last( ).Y ) {
			result.Reverse( );
		}

		//+ 把 XYZR 移动到原点
		//计算Trsf
		Trsf toOriginT;
		var firstPnt = result.First().Pnt;
		var secondPnt = result[1].Pnt;
		toOriginT = new Trsf(new Vec(firstPnt, Pnt.Default));

		//+ 把第二个点的方向挪动到 X 轴上
		//计算Trsf
		// 容差处理
		// 第一第二点间距离
		var firstToSecondPntDistance = firstPnt.Distance(secondPnt);
		// 作等腰三角形
		Pnt pntOnX = new (firstToSecondPntDistance, 0, 0);
		// 当等腰三角形的两个点距离大于容差，认为不重叠
		Trsf secondXYZPntTrsf=Trsf.Default;
		if( secondPnt.Distance(pntOnX) > 0.1 ) {
			var secondQuat = new Quat(new Vec(firstPnt, secondPnt), new Vec(1, 0, 0));
			secondXYZPntTrsf = new Trsf(secondQuat);
		}

		//+ 把第三个点旋转到 xy 平面第一象限
		Trsf thirdXYZPntTrsf=Trsf.Default;
		if( result.Count > 2 ) {
			var thirdGpPnt = result[2].Pnt;

			Vec fromVec = new ( Pnt.Default, thirdGpPnt);
			var angeleValue = -Math.Atan2(fromVec.Z, fromVec.Y);
			Quat thirdQuat = new (new Vec(1, 0, 0), angeleValue);
			thirdXYZPntTrsf = new Trsf(thirdQuat);
		}

		//应用Trsf
		for( int i = 0; i < result.Count; i++ ) {
			// 使用索引更新列表中的元素
			result[i] = result[i].UpdatePnt(result[i].Pnt.
				Transformed(toOriginT).Transformed(secondXYZPntTrsf).Transformed(thirdXYZPntTrsf));
		}
		return result;
	}

	/// <summary>
	/// 根据 XYZR 构造 YBCR (YBC 比 XYZR 少一个点)
	/// </summary>
	private static List<YBCR> XYZR2YBCR( List<XYZR> xyzrList ) {
		if( xyzrList.Count < 3 ) {
			throw new Exception("XYZR列表长度至少为3个点");
		}

		List<YBCR> result = new ();
		//bool firstB = false;

		Pnt lastPnt;
		Vec lastVec;
		XYZR lastXYZ;

		//+ 处理第一个点
		//记录第一个点
		lastPnt = xyzrList[0].Pnt;
		//XYZR lastXYZ=xyzrList[0];
		//+ 处理第二个点
		//计算B的旋转轴
		Vec vec11 = new (lastPnt, xyzrList[1].Pnt);
		Vec vec22 = new (xyzrList[1].Pnt, xyzrList[2].Pnt);
		lastVec = vec11.Crossed(vec22);
		lastXYZ = xyzrList[1];


		//+ 处理中间段(去掉首尾)
		for( int i = 1; i < xyzrList.Count - 1; i++ ) {
			var currentXYZ = xyzrList[i];
			var nextXYZ = xyzrList[i + 1].Pnt;

			// 中间点有 B 值
			lastXYZ = xyzrList[i - 1];
			double paraY = 0.0;
			RAD paraB = 0.0;
			RAD paraC = 0.0;
			decimal paraR = 0.0M;

			if( i == 1 ) {
				paraY = currentXYZ.Pnt.Distance(lastPnt) - (double)currentXYZ.Radius * Math.Tan((RAD)currentXYZ.Angle / 2);
			} else {
				paraY = currentXYZ.Pnt.Distance(lastPnt) - (double)currentXYZ.Radius * Math.Tan((RAD)currentXYZ.Angle / 2) - (double)lastXYZ.Radius * Math.Tan((RAD)lastXYZ.Angle / 2);
			}
			// 求解 B 
			Vec vec1 = new (lastPnt, currentXYZ.Pnt);
			Vec vec2 = new (currentXYZ.Pnt, nextXYZ);
			var currentVec = vec1.Crossed(vec2);

			if( i == 2 ) {
				// 第二个点 B 为 0
				paraB = 0.0;
			} else {
				// 旋转的方向需要判断正负
				paraB = currentVec.AngleWithRef((Vec)lastVec, new Vec(currentXYZ.Pnt, lastPnt));
				// todo 为什么会在这里更新 this.Transform 
				//// 根据第一个 B 绕 X 轴旋转
				//if( firstB == null ) {
				//	var rotaX = new Trsf(new Quat(paraB, 0, 0, EulerSequence.Extrinsic_XYZ));
				//	//Transform = Transform.Multiplied(rotaX);  
				//	firstB = true;
				//}
			}
			// 求C
			if( currentXYZ.Angle != 0.0 ) {
				paraC = (double)currentXYZ.Angle;
			} else {
				var v1 = new Vec(currentXYZ.Pnt, xyzrList[i - 1].Pnt);
				var v2 = new Vec(currentXYZ.Pnt, xyzrList[i + 1].Pnt);
				currentXYZ = currentXYZ.UpdateAngle(Math.PI - v1.Angle(v2));
				paraC = (double)currentXYZ.Angle;
			}
			paraR = (decimal)currentXYZ.Radius;
			result.Append(new YBCR(paraY, paraB, paraC, paraR));
			lastPnt = currentXYZ.Pnt;
			lastVec = currentVec;

		}

		//+ 最后一个点 没有 BCR 值
		var finalXYZ = xyzrList[ ^1];
		var minusHalfY = (double)lastXYZ.Radius * Math.Tan((double)(lastXYZ.Angle ))/ 2;
		var lastY = finalXYZ.Pnt.Distance(lastPnt) - minusHalfY;
		result.Append(new YBCR(lastY, 0.0, 0.0, 0.0M));
		return result;
	}

	private static List<XYZR> YBCR2XYZR( List<YBCR> ybcr ) {
		List<XYZR> result =     [new(default)];
		//! x为平移方向，z为旋转轴方向
		Ax2 currentMoveAxis = new(new Pnt(), new(0, 0, 1), new(1, 0, 0));
		foreach( var ybc in ybcr ) {
			//! 平移Y->旋转B->计算旋转轴（->移动C/2得到点并记录）->旋转C
			//移动Y距离
			Trsf tY = new();
			tY = new(currentMoveAxis.XDir.ToVec(ybc.Y));
			currentMoveAxis = currentMoveAxis.Transformed(tY);
			if( ybc.R == 0 ) {
				result.Add(new(currentMoveAxis.Location, ybc.R));
				continue;
			}
			currentMoveAxis.Transformed(tY);
			//旋转B
			Trsf tB = new();
			tB = new(new Quat(currentMoveAxis.XAxis, ybc.B));
			currentMoveAxis = currentMoveAxis.Transformed(tB);
			//旋转轴是临时轴
			Trsf axisT = new();
			axisT = new(currentMoveAxis.XDir.Crossed(currentMoveAxis.ZDir).Reversed( ).ToVec((double)ybc.R));
			Ax1 bendingAxis = new Ax1(currentMoveAxis.Location, currentMoveAxis.ZDir).Transformed(
				axisT
			);
			//移动C/2对应直段的距离
			var addedY = (double)ybc.R * Math.Abs(Math.Tan(ybc.C / 2));
			Trsf tC =new(currentMoveAxis.XDir.ToVec(addedY));
			Pnt xyzPnt = currentMoveAxis.Location.Transformed(tC);
			result.Add(new(xyzPnt, ybc.R));
			Trsf tR =  new(new Quat(bendingAxis, ybc.C));
			currentMoveAxis = currentMoveAxis.Transformed(tR);
		}
		return result;
	}

	#region 不由Pipe处理的属性

	//! 以下属性应该由业务逻辑控制

	///// <summary>
	///// 首直线段长度
	///// </summary>
	//public double FirstSegLength { get => this.YBCR.Count > 1 ? this.YBCR.First( ).Y : 0.0; }
	///// <summary>
	///// 最短中间直线段长
	///// </summary>
	//public double MinMidSegLength { get => this.YBCR.Count > 2 ? this.YBCR.Skip(1).Take(YBCR.Count - 2).Min(i => i.Y) : 0.0; }
	///// <summary>
	///// 末直线段长度
	///// </summary>
	//public double LastSegLength { get => this.YBCR.Count > 1 ? this.YBCR.Last( ).Y : 0.0; }

	///// <summary>
	///// 夹持长度
	///// </summary>
	//public double ClampLength { get; set; }

	///// <summary>
	///// 首端二次切割长度/工艺加长量
	///// </summary>
	//public double FirstLengthening { get; set; }
	///// <summary>
	///// 末端二次切割长度/工艺加长量
	///// </summary>
	//public double LastLengthening { get; set; }

	//! Pipe中不保存原始的Step模型
	///// <summary>
	///// 原始的step模型
	///// </summary>
	//public TShape OriginStepModel { get; set; }
	///// <summary>
	///// 将解析的管件移动到原点的变换
	///// </summary>
	//public Trsf Transform { get; set; }

	//! 不由Pipe记录原始的XYZ，由业务逻辑控制
	///// <summary>
	///// 原始的XYZ列表
	///// </summary>
	///// <remarks>未补偿</remarks>
	//public List<XYZR> OriginXYZList { get; set; }

	//! XYZ应该不补偿，只补偿YBC
	///// <summary>
	///// XYZ补偿值的列表
	///// </summary>
	//public List<XYZR> XtYtZtList { get; set; }

	//! 不记录YBCR的补偿，由业务逻辑控制
	///// <summary>
	///// YBCR补偿值的列表
	///// </summary>
	//public List<YBCR> YtBtCtRtList { get; set; }  // todo 弄清楚这个YBCR存储的是什么

	///// <summary>
	///// 端到端长度
	///// </summary>
	//public double EndToEndLength {
	//	get {
	//		if( XYZ.Count > 0 ) {
	//			var firstPnt = XYZ.FirstOrDefault().Pnt;
	//			var lastPnt = XYZ.LastOrDefault().Pnt;

	//			var distance = firstPnt.Distance(lastPnt);
	//			return distance;
	//		}
	//		return 0.0;
	//	}
	//}

	///// <summary>
	///// 折弯半径
	///// </summary>
	//public List<decimal> BendingRadius {
	//	get {
	//		var bendingRadiusList = new HashSet<decimal>();
	//		foreach( var oneXYZ in XYZ ) {
	//			if( oneXYZ.Radius != null ) {
	//				bendingRadiusList.Add((decimal)oneXYZ.Radius);
	//			}
	//		}
	//		return bendingRadiusList.ToList( );
	//	}
	//}

	#region 暂未使用的字段
	///// <summary>
	///// 管件对应的原料
	///// </summary>
	//public string Material { get; set; } // todo 是为管件排料匹配做的，检查是否有存在的必要
	///// <summary>
	///// 管件对应的加工材料
	///// </summary>
	//public string ProcessingMaterial { get; set; }    // todo 需要进一步确认使用情况
	///// <summary>
	///// 管件截面的形状
	///// </summary>
	//public TShape CrossSectionShape { get => ShapeHandler.MakeCircle(OuterRadius * 0.8M); }   // 为了干涉计算的方便，截面尺寸比实际管径小 20%

	///// <summary>
	///// 管件的直线段
	///// </summary>
	//public OrderedDictionary WireDict { get; set; }   // todo 需要进一步确认使用情况,set里面设置了length为100

	///// <summary>
	///// 管件的中心线展开长度
	///// </summary>
	//public double FitLength { get; set; } //todo 应该是计算值，不能直接设置
	//public double InputFitLength { get; set; } //todo 从excel输入时会可能会附带管件的中心线展开长度，不定义在Pipe对象本身

	///// <summary>
	///// 管件的首段管接头
	///// </summary>
	//public string FirstFitting { get; set; } // todo 未使用
	///// <summary>
	///// 管件的末段管接头
	///// </summary>
	//public string LastFitting { get; set; } // todo 未使用
	#endregion

	#endregion

}

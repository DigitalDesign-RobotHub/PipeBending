using IMKernel.Utils;

using OCCTK.OCC.gp;

using PipeBendingBLL.Model.Analyzer;

namespace PipeBendingBLL.Model.Pipe;
public static class PipeHandler {

	/// <summary>
	/// 将 XYZR 坐标变为标准情况下的坐标
	/// </summary>
	public static List<XYZR> ToStandardPipe( List<XYZR> xyzrList ) {
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
	public static List<YBCR> XYZR2YBCR( List<XYZR> xyzrList ) {
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

	/// <summary>
	/// 根据 YBCR 构造 XYZR
	/// </summary>
	/// <param name="ybcr"></param>
	/// <returns></returns>
	public static List<XYZR> YBCR2XYZR( List<YBCR> ybcr ) {
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

	public static (double length, decimal diameter, List<XYZR> ybcrs) ParseSTEPFile( string step ) {
		PipeAnalyzer a=new ();
		a.ParseSTEP(step);
		return (2.0, a.Pipe.Diameter, a.Pipe.XYZRs);
	}
}

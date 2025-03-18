using OCCTK.OCC.Topo;

namespace PipeBendingBLL.Model.Pipe;
public class Pipe {
	/// <summary>
	/// 管件的类型
	/// </summary>
	private PipeType type;
	public PipeType Type {
		get {
			if( this.type == PipeType.Unknown ) {
				if( this.YBCRs.Count == 1 && this.YBCRs.First( ).B == 0 && this.YBCRs.First( ).C == 0 ) {
					return PipeType.Straight;
				} else {
					return PipeType.HollowCircle;
				}
			} else {
				return this.type;
			}
		}
		set { this.type = value; }
	}

	#region ReadOnly

	/// <summary>
	/// 管件的内半径
	/// </summary>
	public decimal InnerRadius {
		get {
			if( this.Type == PipeType.HollowCircle ) {
				return this.OuterRadius - this.Thickness;
			} else {
				return 0.0M;
			}
		}
	}

	/// <summary>
	/// 管径的外直径
	/// </summary>
	/// <remarks>管径 (Caliber)</remarks>
	public decimal Diameter => this.OuterRadius * 2;

	/// <summary>
	/// 折弯次数
	/// </summary>
	public int BendingNum { get => this.YBCRs.Count > 1 ? this.YBCRs.Count - 1 : 0; }

	public TShape GetPipeShape( ) {
		//! 最终管件的管件为实际管径的 80%
		return Utils.ShapeHandler.MakePipeFromYBCRList(this.YBCRs, Utils.ShapeHandler.MakeCircleFace(this.OuterRadius * 0.8M));
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
	public List<XYZR> XYZRs => PipeHandler.YBCR2XYZR(this.YBCRs);

	/// <summary>
	/// YBCR列表
	/// </summary>
	/// <remarks>管件的唯一描述</remarks>
	public List<YBCR> YBCRs { get; set; }

	public Pipe( PipeType pipeType ) {
		this.Type = pipeType;
		this.OuterRadius = 0.0M;
		this.Thickness = 0.0M;
		this.YBCRs = [ ];
	}
	// todo 对应原来的_repr_方法
	public override string ToString( ) { return ""; }


}

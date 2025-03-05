using IMKernel.Utils;

namespace PipeBendingBLL.Model.Pipe;
public readonly struct YBCR {
	/// <summary>
	/// 直段长度
	/// </summary>
	public double Y { get; }
	/// <summary>
	/// 旋转角度
	/// </summary>
	public RAD B { get; }
	/// <summary>
	/// 弯管角度
	/// </summary>
	public RAD C { get; }
	/// <summary>
	/// 弯管半径
	/// </summary>
	public decimal R { get; }

	public YBCR UpdateY( double newY ) { return new YBCR(newY, B, C, R); }
	public YBCR UpdateB( RAD newB ) { return new YBCR(Y, newB, C, R); }
	public YBCR UpdateC( RAD newC ) { return new YBCR(Y, B, newC, R); }
	public YBCR UpdateR( decimal newR ) { return new YBCR(Y, B, C, newR); }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="y">直段长度</param>
	/// <param name="b">旋转角度</param>
	/// <param name="c">弯管角度</param>
	/// <param name="r">弯管半径</param>
	public YBCR( double y, RAD b, RAD c, decimal r ) {
		Y = y;
		B = b;
		C = c;
		R = r;
	}
}

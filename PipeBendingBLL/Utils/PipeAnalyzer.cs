using PipeBendingBLL.Model.Pipe;

namespace PipeBendingBLL.Utils;
public class PipeAnalyzer {
	public PipeAnalyzer( ) {
		StartEndFace = null;
		EndEndFace = null;
	}


	public Pipe Pipe {
		get {
			Pipe p=new("1",PipeType.Circle,1,null);
			return p;
		}
	}


	/// <summary>
	/// 起始端面
	/// </summary>
	private PipeFace? StartEndFace { get; set; }
	/// <summary>
	/// 结束端面
	/// </summary>
	private PipeFace? EndEndFace { get; set; }
}

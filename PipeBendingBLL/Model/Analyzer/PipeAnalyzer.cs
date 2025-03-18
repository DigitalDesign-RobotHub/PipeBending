using log4net;

using OCCTK.IO;
using OCCTK.OCC.AIS;
using OCCTK.Extension;
using OCCTK.OCC.Topo;

using PipeBendingBLL.Model.Pipe;

using PipeModel = PipeBendingBLL.Model.Pipe.Pipe;

namespace PipeBendingBLL.Model.Analyzer;
public class PipeAnalyzer {
	private static readonly ILog log = LogManager.GetLogger(typeof(PipeAnalyzer));
#if DEBUG
	/// <summary>
	/// 测试用AIS上下文
	/// </summary>
	public InteractiveContext? DebugContext { get; set; }
#endif

	public PipeModel? Pipe { get; private set; }
	/// <summary>
	/// 文件路径
	/// </summary>
	/// <remarks>需要是包含文件后缀的完整路径</remarks>
	public string? FilePath { get; set; } = null;
	/// <summary>
	/// 文件后缀
	/// </summary><remarks>例如 "step"、"csv"、"excel"</remarks>
	public string? FileSuffix { get; set; } = null;

	private TShape? _originShape;
	/// <summary>
	/// 读取得到的原始STEP形状
	/// </summary>
	public TShape? OriginShape { get => this._originShape; private set { this._originShape = value; } }
	/// <summary>
	/// 解析到的原始XYZR列表
	/// </summary>
	public List<XYZR>? OriginXYZRList { get; private set; }

	#region private
	/// <summary>
	/// 管件对应的YBCR列表
	/// </summary>
	private List<YBCR> YCBRs { get; set; } = [ ];

	/// <summary>
	/// 起始端面
	/// </summary>
	private PipeFace? StartEndFace { get; set; }
	/// <summary>
	/// 结束端面
	/// </summary>
	private PipeFace? EndEndFace { get; set; }
	#endregion

	/// <summary>
	/// 管件分析器
	/// </summary>
	public PipeAnalyzer( ) {

	}
	#region 解析STEP
	private void GetShapeData( ILog methodLog ) {
		// 获取所有的面（面会自己获取边的信息）
		//Explorer explorer = new(this._originShape, ShapeEnum.FACE);
		//Explorer explorer = new();

		//// 并行获取边的信息
		//BlockingCollection<PipeFace> faceCollection = [];
		//Parallel.ForEach(explorer, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, item => {
		//	var f = item.AsFace();
		//	PipeFace face = new(f);
		//	faceCollection.Add(face);
		//});
		//// 转换为 HashSet
		//HashSet<PipeFace>  faces = new(faceCollection);
		//HashSet<PipeFace>  faces = [];
		//int num =0;
		//foreach( var shape in explorer ) {
		//	var f = shape.AsFace();
		//	PipeFace face = new(f);
		//	faces.Add(face);
		//	num++;
		//}

		//#if DEBUG
		//		foreach( var face in faces ) {
		//			AShape pipeAIS = new(face);
		//			this.DebugContext?.Display(pipeAIS, false);
		//			this.DebugContext?.SetColor(pipeAIS, ColorMap.Cyan, false);
		//		}
		//		this.DebugContext?.UpdateCurrentViewer( );
		//		methodLog.Debug($"总计: {num} 获取到{faces.Count}个面");
		//#endif
		//获取所有面的邻接关系


	}

	/// <summary>
	/// 解析STEP文件到解析器中
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	/// <exception cref="Exception"></exception>
	public PipeModel ParseSTEP( string filePath ) {
		ILog methodLog = LogManager.GetLogger($"FUNC: {nameof(PipeAnalyzer)}.{nameof(ParseSTEP)}");
		methodLog.Info("开始解析STEP数据...");


		this.FilePath = filePath;
		this.FileSuffix = Path.GetExtension(filePath).TrimStart('.');
		if( this.FileSuffix == "csv" || this.FileSuffix == "xlsx" || this.FileSuffix == "xls" ) {
			methodLog.Debug($"文件:{this.FilePath} 不是STEP文件");
			throw new FormatException("请调用 PraseExcel 解析Excel文件");
		}
		//- 不做是否为STEP文件的判断

		#region 读取STEP文件

		if( !Path.Exists(this.FilePath) ) {
			throw new FileNotFoundException("无法找到指定文件 ", this.FilePath);
		}
		try {
			//todo STEP读取器未完善
			this.OriginShape = new STEPExchanger(this.FilePath).Shape( ).TopoShape;
		} catch( Exception e ) {
			methodLog.Error($"{e}");
			throw;
		}

		#endregion

		//+++ 解析管件信息
		GetShapeData(methodLog);

		return this.Pipe ?? throw new Exception($"文件:{this.FilePath} 解析失败");
	}

	#endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Visualization;

using IMKernelUI.Message;

using OCCTK.OCC.AIS;
using OCCTK.OCC.OpenGL;
using OCCTK.OCC.V3d;
using OCCTK.Utils;

namespace PipeBendingUI.Singleton;

/// <summary>
/// Viewer管理器作为全局单例，负责绘制引擎和上下文的创建和管理
/// </summary>
public class ThreeDimensionContextManager {
	public ThreeDimensionContextManager( ) {
		Contexts = [ ];
		contextID = 0;
		IsShowViewCube = true;
		IsShowOriginTrihedron = false;
		IsShowViewTrihedron = false;
		IsShowGraduatedTrihedron = false;
	}

	public bool IsShowViewCube { get; set; }
	public bool IsShowOriginTrihedron { get; set; }
	public bool IsShowViewTrihedron { get; set; }
	public bool IsShowGraduatedTrihedron { get; set; }

	public List<ThreeDimensionContext> Contexts { get; set; }

	/// <summary>
	/// 默认的画布，列表第一项
	/// </summary>
	public ThreeDimensionContext? MainContext => Contexts.FirstOrDefault( );
	private int contextID;
	public ThreeDimensionContext CreateContext( ) {
		ThreeDimensionContext context;
		try {
			//创建图形引擎
			GraphicDriver graphicDriver = new();
			//创建视图对象
			Viewer viewer = new(graphicDriver);
			//创建AIS上下文管理器
			InteractiveContext anAISContext = new(viewer);
			//创建三维画布上下文对象
			context = new(anAISContext, contextID);
			Contexts.Add(context);
		} catch( Exception e ) {
			throw new Exception($"画布创建失败：{e}");
		}

		//初始化画布状态
		context.DisplayViewCube(IsShowViewCube);
		context.DisplayViewTrihedron(IsShowViewTrihedron);
		context.DisplayOriginTrihedron(IsShowOriginTrihedron);
		WeakReferenceMessenger.Default.Send(new CanvasCreatedMessage((contextID, IsShowViewCube, IsShowOriginTrihedron, IsShowViewTrihedron)));

		//完成后计数器+1
		contextID++;
		return context;
	}
}

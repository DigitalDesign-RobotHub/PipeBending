using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

using IMKernelUI.Singleton;

using log4net;
using log4net.Config;

using PipeBendingUI.Singleton;
using PipeBendingUI.Utils;
using PipeBendingUI.View;

using Application = System.Windows.Application;

namespace PipeBendingUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App:Application {
	private static readonly ILog log = LogManager.GetLogger(typeof(App));
	public static new App Current => (App)Application.Current;

	/// <summary>
	/// 程序缓存文件夹
	/// </summary>
	public readonly string AppCacheFolder;

	/// <summary>
	/// InsofRobot配置信息
	/// </summary>
	public readonly string InsofRobotConfigFolder;

	//todo 定期清理缓存

	public App( )
		: base( ) {
		//! 缓存的读写需要通过DAL层实现
		// 获取 LocalAppData 文件夹路径
		string appDataPath = Environment.GetFolderPath(
			Environment.SpecialFolder.LocalApplicationData
		);

		// 定义你的应用程序专属缓存目录
		this.AppCacheFolder = Path.Combine(appDataPath, "InsofRobot", "PipeBending", "Cache");
		this.InsofRobotConfigFolder = Path.Combine(appDataPath, "InsofRobot", "Config");
	}

	#region 全局单例
	/// <summary>
	/// 三维对象管理器
	/// </summary>
	public readonly ThreeDimensionContextManager ThreeDimensionContextManager =
		Singleton<ThreeDimensionContextManager>.Instance;

	/// <summary>
	/// 工作空间管理器
	/// </summary>
	public readonly WorkSpaceContextManager WorkSpaceContextManager =
		Singleton<WorkSpaceContextManager>.Instance;

	/// <summary>
	/// 命令管理器
	/// </summary>
	public readonly CommandManager CommandManager = Singleton<CommandManager>.Instance;

	/// <summary>
	/// 静态资源管理器
	/// </summary>
	public readonly StaticResourceManager StaticResourceManager =
		Singleton<StaticResourceManager>.Instance;

	public readonly AuthorizationManager AuthorizationManager =
		Singleton<AuthorizationManager>.Instance;

	#endregion

	[STAThread]
	protected override void OnStartup( StartupEventArgs e ) {
		base.OnStartup(e);

		#region TEST
		//测试权限(正常情况下应该通过启动页加载权限)
		var robot = new RobotPageGroupAuthority() { IsVisible = false }; //关闭机器人组权限
		var workspace = new WorkSpaceAuthority(robot);
		var resources = new ResourcesAuthority(new());
		this.AuthorizationManager.CurrentUser = new("TestUser", new(workspace, resources)) {
			TimeStamp = DateTime.Now
		};
		#endregion

		// 获取启动参数
		var commandLineArgs = new CommandLine(Environment.GetCommandLineArgs());
		#region 加载 log4net 配置文件

		var logRepository = LogManager.GetRepository(
			System.Reflection.Assembly.GetEntryAssembly() ?? throw new Exception("未找到进程exe")
		);
		XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

		#endregion

		#region 检查缓存目录

		// 确保目录存在
		if( !Directory.Exists(this.InsofRobotConfigFolder) ) {
			Directory.CreateDirectory(this.InsofRobotConfigFolder);
			log.Info($"创建 InsofRobot 配置文件目录：{this.InsofRobotConfigFolder}");
		}

		if( !Directory.Exists(this.AppCacheFolder) ) {
			Directory.CreateDirectory(this.AppCacheFolder);
			log.Info($"创建缓存目录：{this.AppCacheFolder}");
		}

		#endregion

		log.Info($"！！！！程序启动！！！！");
		AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);

		var culture = new CultureInfo("zh-Hans");
		Thread.CurrentThread.CurrentCulture = culture;
		Thread.CurrentThread.CurrentUICulture = culture;
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;

		CreateInstanceMutexes( );

		OpenMainWindow( );
		////不显示启动页直接启动
		//if (commandLineArgs.NoStartupPage)
		//{
		//    MainWindow = new MainWindow();
		//    MainWindow.Show();
		//}

		//// 默认显示启动页
		//ShowStartupView();

		this.ShutdownMode = ShutdownMode.OnMainWindowClose;
	}

	/// <summary>
	/// 显示启动页
	/// </summary>
	private void ShowStartupView( ) {
		var startupView = new StartupView();
		startupView.Show( );

		startupView.Closed += ( s, args ) => {
			// 获取用户在启动页选择的选项
			StartupResult startupResult = startupView.Result;

			// 启动页关闭后打开主窗口
			OpenMainWindow(startupResult);
		};
	}

	/// <summary>
	/// 显示主窗口
	/// </summary>
	private void OpenMainWindow( StartupResult? startupResult = null ) {
		//todo 启动的逻辑
		this.MainWindow = new MainWindow( );
		this.MainWindow.Show( );
	}

	/*
* Creates the two mutexes checked for by the installer/uninstaller to see if
* the program is still running.
* One of the mutexes is created in the global name space (which makes it
* possible to access the mutex across user sessions in Windows XP); the other
* is created in the session name space(because versions of Windows NT prior
* to 4.0 TSE don't have workspace global name space and don't support the 'Global\'
* prefix).
*/
	private void CreateInstanceMutexes( ) {
		//const string mutexName = "MacadInstanceRunning";
		///* By default on Windows NT, created mutexes are accessible only by the user
		// * running the process. We need our mutexes to be accessible to all users, so
		// * that the mutex detection can work across user sessions in Windows XP. To
		// * do this we use workspace security descriptor with workspace null DACL.
		// */
		//IntPtr ptrSecurityDescriptor = IntPtr.Zero;
		//try
		//{
		//    var securityDescriptor = new Win32Api.SECURITY_DESCRIPTOR();
		//    Win32Api.InitializeSecurityDescriptor(
		//        out securityDescriptor,
		//        Win32Api.SECURITY_DESCRIPTOR_REVISION
		//    );
		//    Win32Api.SetSecurityDescriptorDacl(ref securityDescriptor, true, IntPtr.Zero, false);
		//    ptrSecurityDescriptor = Marshal.AllocCoTaskMem(Marshal.SizeOf(securityDescriptor));
		//    Marshal.StructureToPtr(securityDescriptor, ptrSecurityDescriptor, false);

		//    var securityAttributes = new Win32Api.SECURITY_ATTRIBUTES();
		//    securityAttributes.nLength = Marshal.SizeOf(securityAttributes);
		//    securityAttributes.lpSecurityDescriptor = ptrSecurityDescriptor;
		//    securityAttributes.bInheritHandle = false;

		//    if (
		//        Win32Api.CreateMutex(ref securityAttributes, false, mutexName) == IntPtr.Zero
		//        || Win32Api.CreateMutex(ref securityAttributes, false, @"Global\" + mutexName)
		//            == IntPtr.Zero
		//    )
		//    {
		//        var lastError = Win32Error.GetLastError();
		//        // if we get the ERROR_ALREADY_EXISTS value, there is
		//        // already another instance of this application running.
		//        // That is ok and no error.
		//        if (lastError != Win32Error.ERROR_ALREADY_EXISTS)
		//        {
		//            Console.WriteLine(
		//                $"InstanceMutex creation failed: {Marshal.GetLastWin32Error()}"
		//            );
		//        }
		//    }
		//}
		//catch (Exception e)
		//{
		//    Console.WriteLine(e);
		//}

		//if (ptrSecurityDescriptor != IntPtr.Zero)
		//    Marshal.FreeCoTaskMem(ptrSecurityDescriptor);
	}
}

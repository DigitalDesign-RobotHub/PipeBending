using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeBendingUI.Utils;

public class CommandLine
{
    //public string PathToOpen { get; }
    //public bool HasPathToOpen => !PathToOpen.IsNullOrEmpty();
    //public bool EnableSandbox { get; }
    public bool NoStartupPage { get; }
    public bool Nothing { get; }

    //public string ScriptToRun { get; }
    //public bool HasScriptToRun => !ScriptToRun.IsNullOrEmpty();

    //--------------------------------------------------------------------------------------------------

    /// <summary>
    /// 初始化 CommandLine 对象，解析命令行参数。
    /// </summary>
    /// <param name="cmdArgs">命令行参数数组。</param>
    public CommandLine(string[] cmdArgs)
    {
        // 遍历传入的每一个命令行参数
        foreach (var cmdarg in cmdArgs)
        {
            // 判断参数是否以 '-' 或 '/' 开头，这通常表示选项参数
            if (cmdarg.StartsWith("-") | cmdarg.StartsWith("/"))
            {
                string option; // 用于存储选项名
                string? parameter; // 用于存储选项的值（如果存在）

                // 寻找 '=' 或 ':' 的位置，用于分隔选项名和参数值
                int splitPos = Math.Min(cmdarg.IndexOf('='), cmdarg.IndexOf(':'));

                // 如果找到分隔符，解析选项名和参数值
                if (splitPos > 1)
                {
                    // 提取选项名（去掉前缀字符 '-' 或 '/'）
                    option = cmdarg.Substring(1, splitPos - 1).ToLower();
                    // 提取选项的参数值
                    parameter = cmdarg.Substring(splitPos + 1);
                }
                else
                {
                    // 没有分隔符时，只有选项名，没有参数值
                    option = cmdarg.Substring(1).ToLower();
                }
                //将选项转化为小写字母
                option = option.ToLower();
                // 根据解析的选项名，执行不同的处理逻辑
                switch (option)
                {
                    case "nostartuppage":
                        NoStartupPage = true;
                        break;
                }
            }
            else
            {
                // 如果参数不是以 '-' 或 '/' 开头，将其标记为普通参数
                Nothing = true;
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
}

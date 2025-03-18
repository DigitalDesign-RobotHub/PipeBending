using System;
using System.Collections.Generic;

namespace PipeBendingUI.Singleton;

public class StaticResourceManager {
	public Dictionary<string, Object> Robots = [];
	public Dictionary<string, Object> Models = [];
	public Dictionary<string, Object> Pipes = [];
	public Dictionary<string, Object> Components = [];
	public Dictionary<string, Object> ProgramTemplates = [];
}

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PipeBendingUI.Singleton;

//权限控制只控制Ribbon选项卡和组的可见性

public class AuthorizationManager
{
    public UserAuthority CurrentUser { get; set; }
}

#region Base
/// <summary>
/// 权限基类
/// </summary>
public abstract class Authority
{
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// 选项卡权限
/// </summary>
public abstract class RibbonPageAuthority : Authority { }

/// <summary>
/// 选项卡组权限
/// </summary>
public abstract class RibbonPageGroupAuthority : Authority { }
#endregion

#region Ribbon权限

public class RibbonAuthority(
    WorkSpaceAuthority workSpaceAuthority,
    ResourcesAuthority resourcesAuthority
)
{
    public WorkSpaceAuthority WorkSpace { get; set; } = workSpaceAuthority;
    public ResourcesAuthority Resources { get; set; } = resourcesAuthority;
}

#region WorkSpace
public class WorkSpaceAuthority : RibbonPageAuthority
{
    public WorkSpaceAuthority(RobotPageGroupAuthority robot)
    {
        Robot = robot;
        robot.ParentPage = this;
    }

    public RobotPageGroupAuthority Robot { get; set; }
}

public class RobotPageGroupAuthority : RibbonPageGroupAuthority
{
    public RibbonPageAuthority? ParentPage { get; set; }
}
#endregion

#region Resources
public class ResourcesAuthority : RibbonPageAuthority
{
    public ResourcesAuthority(StaticResourceAuthority staticResource)
    {
        StaticResource = staticResource;
        staticResource.ParentPage = this;
    }

    public StaticResourceAuthority StaticResource { get; set; }
}

public class StaticResourceAuthority : RibbonPageGroupAuthority
{
    public RibbonPageAuthority? ParentPage { get; set; }
}
#endregion

#endregion
public class UserAuthority
{
    public UserAuthority(string name, RibbonAuthority ribbonAuthority)
    {
        UserName = name;
        RibbonAuthority = ribbonAuthority;
    }

    public string UserName { get; set; }
    public System.DateTime TimeStamp { get; set; }
    public RibbonAuthority RibbonAuthority { get; set; }
}

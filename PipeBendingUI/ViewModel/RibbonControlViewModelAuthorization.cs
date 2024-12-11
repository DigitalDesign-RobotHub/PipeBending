using CommunityToolkit.Mvvm.ComponentModel;

namespace PipeBendingUI.ViewModel;

partial class RibbonControlViewModel
{
    #region Authorization
    private void VisibilityControl()
    {
        //获取当前权限
    }

    public bool WorkSpace_RibbonPage_Visible =>
        App.Current.AuthorizationManager.CurrentUser.RibbonAuthority.WorkSpace.IsVisible;
    public bool WorkSpace_Robot_RibbonPageGroup_Visible =>
        App.Current.AuthorizationManager.CurrentUser.RibbonAuthority.WorkSpace.Robot.IsVisible;
    public bool Resources_RibbonPage_Visible =>
        App.Current.AuthorizationManager.CurrentUser.RibbonAuthority.Resources.IsVisible;
    #endregion
}

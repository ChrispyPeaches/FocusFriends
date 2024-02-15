using SimpleToolkit.SimpleShell.Transitions;

namespace FocusApp.Client;

public static class Transitions
{
    public static PlatformSimpleShellTransition CustomPlatformTransition { get; } = new PlatformSimpleShellTransition
    {
#if ANDROID
        SwitchingEnterAnimation = static (args) => Resource.Animation.simpleshell_enter_right,
        SwitchingLeaveAnimation = static (args) => Resource.Animation.simpleshell_exit_left,
        PushingEnterAnimation = static (args) => Resource.Animation.simpleshell_enter_right,
        PushingLeaveAnimation = static (args) => Resource.Animation.simpleshell_exit_left,
        PoppingEnterAnimation = static (args) => Resource.Animation.simpleshell_enter_left,
        PoppingLeaveAnimation = static (args) => Resource.Animation.simpleshell_exit_right
#endif
    };
}
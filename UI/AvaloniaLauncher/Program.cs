using Avalonia;
using Avalonia.ReactiveUI;

namespace KSCSharp.AvaloniaLauncher;

public static class Program
{
    // Basic Avalonia app bootstrap. Will host Bloxstrap-styled UI.
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace KSCSharp.AvaloniaLauncher;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Minimal window for initial scaffold; replace with Bloxstrap UI XAML
            desktop.MainWindow = new Avalonia.Controls.Window
            {
                Width = 800,
                Height = 600,
                Title = "KSC-Sharp (Avalonia UI scaffold)"
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

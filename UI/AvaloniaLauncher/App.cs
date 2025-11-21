using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;

namespace KSCSharp.AvaloniaLauncher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = Avalonia.Themes.Fluent.FluidThemeVariant();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Ensure Light theme; alternative is RequestedThemeVariant = ThemeVariant.Light
        RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

internal static class ThemeHelpers
{
    // Helper to avoid direct dependency issues; placeholder for future dynamic theme switching
    public static Avalonia.Styling.ThemeVariant FluidThemeVariant() => Avalonia.Styling.ThemeVariant.Light;
}

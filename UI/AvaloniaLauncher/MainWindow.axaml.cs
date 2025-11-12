using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSCSharp.Core;

namespace KSCSharp.AvaloniaLauncher;

public partial class MainWindow : Window
{
    private readonly FastFlagsManager _ffManager = new();
    private const string BootstrapperUrl = "https://setup.pekora.zip/PekoraPlayerLauncher.exe";
    private const string BootstrapperFile = "PekoraPlayerLauncher.exe";

    public MainWindow()
    {
        InitializeComponent();

        BtnLaunch2020.Click += BtnLaunch2020_Click;
        BtnLaunch2021.Click += BtnLaunch2021_Click;
        BtnFastFlags.Click += BtnFastFlags_Click;
        BtnDownloadBootstrapper.Click += BtnDownloadBootstrapper_Click;
    }

    private void AppendLog(string text)
    {
        Dispatcher.UIThread.Post(() =>
        {
            LogTextBox.Text += $"[{DateTime.Now:HH:mm:ss}] {text}\n";
            LogTextBox.CaretIndex = LogTextBox.Text.Length;
        });
    }

    private void BtnLaunch2020_Click(object? sender, RoutedEventArgs e)
    {
        AppendLog("Launching 2020 (attempt)...");
        var paths = LauncherHelper.GetExecutablePaths("2020L");
        var exe = paths.FirstOrDefault(System.IO.File.Exists);
        if (exe is not null)
        {
            AppendLog($"Found: {exe}");
            LauncherHelper.Launch(exe, new string[] { "--app" });
        }
        else
        {
            AppendLog("Executable not found. Check installation path.");
        }
    }

    private void BtnLaunch2021_Click(object? sender, RoutedEventArgs e)
    {
        AppendLog("Launching 2021 (attempt)...");
        var paths = LauncherHelper.GetExecutablePaths("2021M");
        var exe = paths.FirstOrDefault(System.IO.File.Exists);
        if (exe is not null)
        {
            AppendLog($"Found: {exe}");
            LauncherHelper.Launch(exe, new string[] { "--app" });
        }
        else
        {
            AppendLog("Executable not found. Check installation path.");
        }
    }

    private void BtnFastFlags_Click(object? sender, RoutedEventArgs e)
    {
        var flags = _ffManager.Load();
        AppendLog($"Loaded {flags.Count} FastFlag(s).");
        // Simple demonstration: show first 5 keys
        foreach (var kv in flags.Take(5))
        {
            AppendLog($"FFlag: {kv.Key} = {kv.Value}");
        }
    }

    private async void BtnDownloadBootstrapper_Click(object? sender, RoutedEventArgs e)
    {
        AppendLog("Starting bootstrapper download...");
        DownloadProgress.Value = 0;
        var dl = new BootstrapperDownloader(BootstrapperUrl, BootstrapperFile);
        var cts = new CancellationTokenSource();
        var progress = new Progress<(long downloaded, long? total)>(p =>
        {
            if (p.total.HasValue && p.total.Value > 0)
            {
                var percent = Math.Min(100, (int)(p.downloaded * 100 / p.total.Value));
                DownloadProgress.Value = percent;
            }
        });

        var ok = await Task.Run(() => dl.DownloadAsync(progress, cts.Token));
        AppendLog(ok ? "[*] Download finished." : "[!] Download failed.");
    }
}

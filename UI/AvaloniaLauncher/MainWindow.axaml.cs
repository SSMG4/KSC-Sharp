using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSCSharp.Core;

#if NET8_0_WINDOWS
using Microsoft.Win32;
#endif

namespace KSCSharp.AvaloniaLauncher;

public partial class MainWindow : Window
{
    private readonly FastFlagsManager _ffManager = new();
    private const string BootstrapperUrl = "https://setup.pekora.zip/PekoraPlayerLauncher.exe";
    private const string BootstrapperFile = "PekoraPlayerLauncher.exe";

    public MainWindow()
    {
        InitializeComponent();

        var btnLaunch2020 = this.FindControl<Button>("BtnLaunch2020");
        var btnLaunch2021 = this.FindControl<Button>("BtnLaunch2021");
        var btnFastFlags = this.FindControl<Button>("BtnFastFlags");
        var btnDownloadBootstrapper = this.FindControl<Button>("BtnDownloadBootstrapper");
        var btnRegisterUri = this.FindControl<Button>("BtnRegisterUri");
        var btnUnregisterUri = this.FindControl<Button>("BtnUnregisterUri");
        var btnClearLog = this.FindControl<Button>("BtnClearLog");
        var btnCopyLog = this.FindControl<Button>("BtnCopyLog");

        btnLaunch2020!.Click += (_, __) => LaunchVersion("2020L");
        btnLaunch2021!.Click += (_, __) => LaunchVersion("2021M");
        btnFastFlags!.Click += BtnFastFlags_Click;
        btnDownloadBootstrapper!.Click += BtnDownloadBootstrapper_Click;
        btnRegisterUri!.Click += (_, __) => RegisterUriScheme();
        btnUnregisterUri!.Click += (_, __) => UnregisterUriScheme();
        btnClearLog!.Click += (_, __) => ClearLog();
        btnCopyLog!.Click += (_, __) => CopyLogToClipboard();
    }

    private ProgressBar Progress => this.FindControl<ProgressBar>("DownloadProgress")!;
    private TextBlock ProgressText => this.FindControl<TextBlock>("ProgressText")!;
    private TextBox Log => this.FindControl<TextBox>("LogTextBox")!;

    private void AppendLog(string text)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Log.Text += $"[{DateTime.Now:HH:mm:ss}] {text}\n";
            Log.CaretIndex = Log.Text.Length;
        });
    }

    private void ClearLog()
    {
        Dispatcher.UIThread.Post(() => Log.Text = string.Empty);
    }

    private async void CopyLogToClipboard()
    {
        if (this.Clipboard is null) return;
        await this.Clipboard.SetTextAsync(Log.Text ?? string.Empty);
        AppendLog("[*] Log copied to clipboard.");
    }

    private void LaunchVersion(string folder)
    {
        try
        {
            var flags = _ffManager.Load();
            if (flags.Count > 0)
            {
                AppendLog($"[*] Applying {flags.Count} FastFlag(s)...");
                _ffManager.Save(flags);
            }

            AppendLog($"Launching {folder}...");
            var paths = LauncherHelper.GetExecutablePaths(folder);
            var exe = paths.FirstOrDefault(System.IO.File.Exists);
            if (exe is not null)
            {
                LauncherHelper.Launch(exe, new string[] { "--app" });
                AppendLog($"[+] Launched: {exe}");
            }
            else
            {
                AppendLog("[-] Executable not found. Searched:");
                foreach (var p in paths) AppendLog($" - {p}");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"[!] Launch failed: {ex.Message}");
        }
    }

    private async void BtnFastFlags_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new FastFlagsDialog(_ffManager.Load());
            var result = await dialog.ShowDialog<FastFlagsDialog.Result?>(this);
            if (result is { Saved: true })
            {
                _ffManager.Save(result.Flags);
                AppendLog($"[*] FastFlags saved ({result.Flags.Count}).");
            }
            else
            {
                AppendLog("[*] FastFlags dialog closed without saving.");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"[!] FastFlags error: {ex.Message}");
        }
    }

    private async void BtnDownloadBootstrapper_Click(object? sender, RoutedEventArgs e)
    {
        AppendLog("Starting bootstrapper download...");
        SetProgress(0);

        var dl = new BootstrapperDownloader(BootstrapperUrl, BootstrapperFile);
        var cts = new CancellationTokenSource();
        var progress = new Progress<(long downloaded, long? total)>(p =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (p.total.HasValue && p.total.Value > 0)
                {
                    var percent = Math.Min(100, (int)(p.downloaded * 100 / p.total.Value));
                    SetProgress(percent);
                }
            });
        });

        bool ok = false;
        try
        {
            ok = await Task.Run(() => dl.DownloadAsync(progress, cts.Token));
        }
        catch (Exception ex)
        {
            AppendLog($"[!] Download error: {ex.Message}");
        }

        AppendLog(ok ? "[*] Download finished." : "[!] Download failed.");
        SetProgress(0);
    }

    private void SetProgress(int percent)
    {
        Progress.Value = percent;
        ProgressText.Text = $"{percent}%";
    }

    private void RegisterUriScheme()
    {
#if NET8_0_WINDOWS
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "WindowsLauncher.exe";
            var commandValue = $"\"{exePath}\" --uri \"%1\"";

            using var baseKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\pekora-player");
            baseKey?.SetValue("URL Protocol", "", RegistryValueKind.String);

            using var shellKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\pekora-player\shell\open\command");
            shellKey?.SetValue("", commandValue, RegistryValueKind.String);

            AppendLog("[*] Registered URI scheme: pekora-player://");
        }
        catch (Exception ex)
        {
            AppendLog($"[!] Failed to register URI scheme: {ex.Message}");
        }
#else
        AppendLog("[!] URI registration is only supported on Windows.");
#endif
    }

    private void UnregisterUriScheme()
    {
#if NET8_0_WINDOWS
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\pekora-player", throwOnMissingSubKey: false);
            AppendLog("[*] Unregistered URI scheme: pekora-player://");
        }
        catch (Exception ex)
        {
            AppendLog($"[!] Failed to unregister URI scheme: {ex.Message}");
        }
#else
        AppendLog("[!] URI unregistration is only supported on Windows.");
#endif
    }
}

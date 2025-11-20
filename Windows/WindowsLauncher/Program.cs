using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSCSharp.Core;

namespace KSCSharp.WindowsLauncher;

class Program
{
    private const string BootstrapperUrl = "https://setup.pekora.zip/PekoraPlayerLauncher.exe";
    private const string BootstrapperFile = "PekoraPlayerLauncher.exe";

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("KSC-Sharp Windows Launcher (console)");
        Console.WriteLine($"Running on: {(KSCSharp.Core.Models.SystemInfo.IsWindows ? "Windows" : "Other")}\n");

        if (args.Length > 1 && args[0] == "--uri")
        {
            var raw = args[1];
            if (raw.StartsWith("pekora-player://", StringComparison.OrdinalIgnoreCase))
                raw = raw.Replace("pekora-player://", "");
            var parsed = UriParser.Parse(raw);
            Console.WriteLine($"Parsed client version: {parsed.Year}");
            Console.WriteLine($"Args: {parsed.ArgsString}");
            return 0;
        }

        if (args.Length > 0 && args[0].StartsWith("pekora-player://", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = UriParser.Parse(args[0].Replace("pekora-player://", ""));
            Console.WriteLine($"Parsed client version: {parsed.Year}");
            Console.WriteLine($"Args: {parsed.ArgsString}");
            return 0;
        }

        var manager = new FastFlagsManager();
        while (true)
        {
            Console.WriteLine("Select option:");
            Console.WriteLine("1 - Launch 2020");
            Console.WriteLine("2 - Launch 2021");
            Console.WriteLine("3 - Set FastFlags");
            Console.WriteLine("4 - Download/Update Bootstrapper");
            Console.WriteLine("5 - Register URI scheme (per-user)");
            Console.WriteLine("6 - Unregister URI scheme (per-user)");
            Console.WriteLine("0 - Exit");
            Console.Write("> ");
            var choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    LaunchVersion("2020L", manager);
                    break;
                case "2":
                    LaunchVersion("2021M", manager);
                    break;
                case "3":
                    ConfigureFastFlags(manager);
                    break;
                case "4":
                    await DownloadBootstrapper();
                    break;
                case "5":
                    RegisterUriScheme();
                    break;
                case "6":
                    UnregisterUriScheme();
                    break;
                case "0":
                    return 0;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }

    static void LaunchVersion(string folder, FastFlagsManager ffm)
    {
        var flags = ffm.Load();
        if (flags.Count > 0)
        {
            Console.WriteLine($"[*] Applying {flags.Count} FastFlag(s)...");
            ffm.Save(flags);
        }
        Console.WriteLine($"Launching {folder}...");
        var paths = LauncherHelper.GetExecutablePaths(folder);
        var exe = paths.FirstOrDefault(File.Exists);
        if (exe != null)
        {
            LauncherHelper.Launch(exe, new string[] { "--app" });
        }
        else
        {
            Console.WriteLine("Could not find executable. Searched:");
            foreach (var p in paths) Console.WriteLine($" - {p}");
        }
    }

    static void ConfigureFastFlags(FastFlagsManager ffm)
    {
        var flags = ffm.Load();
        Console.WriteLine("Current FastFlags:");
        if (flags.Count == 0) Console.WriteLine("  (none)");
        else foreach (var kv in flags) Console.WriteLine($" - {kv.Key} = {kv.Value}");

        Console.WriteLine("Add new flag? (y/N)");
        var yn = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (yn == "y")
        {
            Console.Write("Key: ");
            var key = Console.ReadLine()?.Trim();
            Console.Write("Value: ");
            var val = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(key))
            {
                flags[key!] = FastFlagsManager.AutoDetectValue(val ?? "");
                ffm.Save(flags);
            }
        }
    }

    static async Task DownloadBootstrapper()
    {
        var dl = new BootstrapperDownloader(BootstrapperUrl, BootstrapperFile);
        var cts = new CancellationTokenSource();
        var progress = new Progress<(long downloaded, long? total)>(p =>
        {
            if (p.total.HasValue && p.total.Value > 0)
            {
                var percent = Math.Min(100, (int)(p.downloaded * 100 / p.total.Value));
                Console.Write($"\rDownloading: {percent}% ({p.downloaded}/{p.total})");
            }
        });
        var ok = await dl.DownloadAsync(progress, cts.Token);
        Console.WriteLine();
        Console.WriteLine(ok ? "[*] Download finished." : "[!] Download failed.");
    }

    // Per-user URI scheme registration for pekora-player://
    static void RegisterUriScheme()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "WindowsLauncher.exe";
            var commandValue = $"\"{exePath}\" --uri \"%1\"";

            using var baseKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Classes\pekora-player");
            baseKey?.SetValue("URL Protocol", "", Microsoft.Win32.RegistryValueKind.String);

            using var shellKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Classes\pekora-player\shell\open\command");
            shellKey?.SetValue("", commandValue, Microsoft.Win32.RegistryValueKind.String);

            Console.WriteLine("[*] Registered URI scheme: pekora-player://");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Failed to register URI scheme: {ex.Message}");
        }
    }

    static void UnregisterUriScheme()
    {
        try
        {
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\pekora-player", throwOnMissingSubKey: false);
            Console.WriteLine("[*] Unregistered URI scheme: pekora-player://");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Failed to unregister URI scheme: {ex.Message}");
        }
    }
}

using System;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace KSCSharp.Core;

public static class LinuxIntegration
{
    private static readonly string Home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly string LocalShare = Path.Combine(Home, ".local", "share");
    private static readonly string DesktopAppsDir = Path.Combine(LocalShare, "applications");
    private static readonly string IconsBase = Path.Combine(LocalShare, "icons", "hicolor");
    public static readonly string EntryFile = Path.Combine(DesktopAppsDir, "pekora-player.desktop");
    public static readonly string UninstallEntryFile = Path.Combine(DesktopAppsDir, "uninstall-pekora-player.desktop");

    public static void CreateDesktopEntry(string scriptPath)
    {
        if (!Directory.Exists(DesktopAppsDir)) Directory.CreateDirectory(DesktopAppsDir);
        var desktopContent = $"[Desktop Entry]\nName=Pekora Player\nExec=dotnet {scriptPath} --uri %u\nType=Application\nTerminal=false\nMimeType=x-scheme-handler/pekora-player\nCategories=Game\nIcon=pekora-player\nNoDisplay=true\n";
        File.WriteAllText(EntryFile, desktopContent);
        var uninstallContent = $"[Desktop Entry]\nName=Uninstall Pekora Player\nExec=dotnet {scriptPath} --uninstall\nType=Application\nTerminal=true\nCategories=Game\nIcon=pekora-player\n";
        File.WriteAllText(UninstallEntryFile, uninstallContent);
        Console.WriteLine($"[*] Desktop entry created: {EntryFile}");
        Console.WriteLine($"[*] Uninstall entry created: {UninstallEntryFile}");
    }

    public static void RegisterMimeHandler()
    {
        try
        {
            // update desktop database
            Process.Start(new ProcessStartInfo
            {
                FileName = "update-desktop-database",
                Arguments = DesktopAppsDir,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit();
        }
        catch { /* best-effort */ }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "xdg-mime",
                Arguments = $"default {Path.GetFileName(EntryFile)} x-scheme-handler/pekora-player",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit();
            Console.WriteLine("[*] MIME type registered");
        }
        catch
        {
            Console.WriteLine("[!] Could not register MIME type (xdg-mime may be missing).");
        }
    }

    public static async Task DownloadIconAsync()
    {
        var iconDir = Path.Combine(IconsBase, "96x96", "apps");
        Directory.CreateDirectory(iconDir);
        var iconPath = Path.Combine(iconDir, "pekora-player.png");
        var iconUrl = "https://raw.githubusercontent.com/johnhamilcar/PekoraBootstrapperLinux/refs/heads/main/pekora-player-bootstrapper.png";

        try
        {
            using var http = new HttpClient();
            var data = await http.GetByteArrayAsync(iconUrl);
            await File.WriteAllBytesAsync(iconPath, data);
            Console.WriteLine($"[*] Icon installed: {iconPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Could not download icon: {ex.Message}");
        }
    }

    public static void UninstallIntegration()
    {
        TryDelete(EntryFile);
        TryDelete(UninstallEntryFile);
        var iconPath = Path.Combine(IconsBase, "96x96", "apps", "pekora-player.png");
        TryDelete(iconPath);

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "update-desktop-database",
                Arguments = DesktopAppsDir,
                UseShellExecute = false,
                CreateNoWindow = true,
            })?.WaitForExit();
        }
        catch { }
        Console.WriteLine("[*] Linux integration uninstalled!");
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Failed to remove {path}: {ex.Message}");
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace KSCSharp.Core;

public static class LauncherHelper
{
    public static string[] GetDefaultWindowsRoots()
    {
        var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return new[]
        {
            Path.Combine(localApp, "ProjectX", "Versions"),
            Path.Combine(localApp, "Pekora", "Versions")
        };
    }

    // Find possible executable paths for a given folder name (like "2020L")
    public static string[] GetExecutablePaths(string folder)
    {
        var roots = GetDefaultWindowsRoots();
        var list = roots.Select(r => Path.Combine(r, folder, "ProjectXPlayerBeta.exe")).ToArray();
        return list;
    }

    public static void Launch(string exePath, string[] args)
    {
        if (!File.Exists(exePath))
        {
            Console.Error.WriteLine($"[!] Executable not found: {exePath}");
            return;
        }

        var start = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = string.Join(' ', args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
            UseShellExecute = false
        };

        Process.Start(start);
        Console.WriteLine("[*] Client launched successfully!");
    }
}

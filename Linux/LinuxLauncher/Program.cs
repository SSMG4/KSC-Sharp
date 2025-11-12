using System;
using System.Threading.Tasks;
using KSCSharp.Core;

namespace KSCSharp.LinuxLauncher;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("KSC-Sharp Linux Launcher (early console)");
        if (args.Length > 0 && args[0] == "--install")
        {
            var scriptPath = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? "KSC-Sharp";
            LinuxIntegration.CreateDesktopEntry(scriptPath);
            await LinuxIntegration.DownloadIconAsync();
            LinuxIntegration.RegisterMimeHandler();
            Console.WriteLine("[*] Linux integration setup complete!");
            return 0;
        }
        if (args.Length > 0 && (args[0] == "--uninstall" || args[0] == "-u"))
        {
            Console.WriteLine("Uninstalling integration...");
            LinuxIntegration.UninstallIntegration();
            return 0;
        }

        Console.WriteLine("Run with --install to set up desktop integration, or use the UI when available.");
        return 0;
    }
}

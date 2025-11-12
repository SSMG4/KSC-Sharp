# Windows Installer Scaffolds

This folder contains three installer scaffolds for Windows:

- WiX Product.wxs (Windows/Installer/WiX/Product.wxs)
  - Use WiX Toolset (candle & light) to build an MSI package. Add your files to ComponentGroup and update UpgradeCode GUID.

- Inno Setup script (Windows/Installer/Inno/installer.iss)
  - Use Inno Setup to create an EXE installer. Update the Source path to include built binaries.

- PowerShell installer (Windows/Installer/install.ps1)
  - A simple script to copy files into Program Files and create a Start Menu shortcut. Useful for manual installs.

Build tips
1. Build your projects in Release:
   dotnet publish Windows/WindowsLauncher -c Release -r win-x64 --self-contained false

2. Place publish output into Windows/Installer/dist and run the chosen packaging tool:
   - Inno: use Inno Setup Compiler on installer.iss
   - WiX: create components in Product.wxs and run candle + light
   - PowerShell: run install.ps1 with elevated privileges

Remember: code signing should be added in CI/release pipeline. Store signing certificates and keystores securely in GitHub Secrets and use a protected manual release workflow to consume them.

<p align="center">
  <img width="96" height="96" alt="KoroneStrap Logo" src="KSCSL.png" />
</p>
<h1 align="center">KSC-Sharp</h1>

<p align="center">
  <i>A C#/.NET 8 port and expansion of the LittleBigDevs Team's [koroneStrap](https://github.com/LittleBigDevs/koroneStrap) bootstrapper for Korone baked with Bloxstrap's Avalonia GUI/UI and lots of improvements.</i>
</p>

<p align="center">
  
[![KSC-Sharp Windows CI](https://github.com/SSMG4/KSC-Sharp/actions/workflows/windows-ci.yml/badge.svg?branch=master)](https://github.com/SSMG4/KSC-Sharp/actions/workflows/windows-ci.yml)
[![KSC-Sharp Linux CI](https://github.com/SSMG4/KSC-Sharp/actions/workflows/linux-ci.yml/badge.svg?branch=master)](https://github.com/SSMG4/KSC-Sharp/actions/workflows/linux-ci.yml)
[![KSC-Sharp macOS CI](https://github.com/SSMG4/KSC-Sharp/actions/workflows/macos-ci.yml/badge.svg?branch=master)](https://github.com/SSMG4/KSC-Sharp/actions/workflows/macos-ci.yml)
[![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/SSMG4/KSC-Sharp/total)](https://github.com/SSMG4/KSC-Sharp/releases)
[![GitHub Downloads (latest)](https://img.shields.io/github/downloads/SSMG4/KSC-Sharp/latest)](https://github.com/SSMG4/KSC-Sharp/releases/latest)
[![GitHub Release](https://img.shields.io/github/v/release/SSMG4/KSC-Sharp)](https://github.com/SSMG4/KSC-Sharp/releases/latest)
[![GitHub Repo stars](https://img.shields.io/github/stars/SSMG4/KSC-Sharp)](https://github.com/SSMG4/KSC-Sharp/stargazers)
[![GitHub Pull Requests](https://img.shields.io/github/issues-pr/SSMG4/KSC-Sharp)](https://github.com/SSMG4/KSC-Sharp/pulls)
[![GitHub Issues](https://img.shields.io/github/issues/SSMG4/KSC-Sharp)](https://github.com/SSMG4/KSC-Sharp/issues)

</p>

---

## Goals / Roadmap
1. Finish Windows frontend with Avalonia UI (Bloxstrap assets integrated, credit preserved).
2. Port Linux and macOS platform integrations (desktop entry, MIME handler, icon).
3. Add mobile support (recommended: .NET MAUI) and native signing helpers (Kotlin for Android, Swift for iOS).
4. Create platform-specific installers (WiX & Inno for Windows; AppImage / deb / dmg for Linux/macOS).
5. Add tests (xUnit), more CI workflows with path filtering, and release workflows.

## Licenses
- [LICENSE.Bloxstrap](./LICENSE.Bloxstrap) - MIT (for using some Bloxstrap assets)
- [LICENSE.KoroneStrap](./LICENSE.KoroneStrap) - GPL-3.0 (for the original KoroneStrap project)
- [LICENSE](./LICENSE) - GPL-3.0 (for KSC-Sharp)

## Credits
- [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap) - UI assets
- [LittleBigDevs / koroneStrap](https://github.com/LittleBigDevs/koroneStrap) — custom Python bootstrapper
- [KoroneX / Korone-Bootstrapper](https://github.com/KoroneX/Korone-Bootstrapper) — original Windows bootstrapper
- SSMG4 — project owner / integrator

## How to build (local, Windows only)
- Requires .NET 8 SDK.
- Build Core:
  dotnet build KoroneStrap.Core/KoroneStrap.Core.csproj
- Build Windows launcher:
  dotnet build Windows/WindowsLauncher/WindowsLauncher.csproj
- Run Windows launcher (for quick test):
  dotnet run --project Windows/WindowsLauncher/WindowsLauncher.csproj

## Contributing
- Follow the folder-per-platform structure:
  - .github/ (Workflows, CIs, ...)
  - Windows/ (Main focus)
  - Linux/ (W.I.P)
  - macOS/ (W.I.P)
  - Android/ (Coming Soon)
  - iOS/ (Coming Soon)

## State Of The Project
Right now the project is in early development and will continue to be developed until a stable release is made for each platform.
If i'd estimate, we are around 10% to the way there

> By the way, this README is bare bone, its not the final version, please wait.

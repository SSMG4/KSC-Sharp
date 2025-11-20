using System;
using System.Collections.Generic;
using System.Net;

namespace KSCSharp.Core;

public class ParsedUri
{
    public List<string> Args { get; set; } = new();
    public string ArgsString { get; set; } = string.Empty;
    public string Year { get; set; } = "2017L";
}

public static class UriParser
{
    private static readonly Dictionary<string, string> UriKeyArgMap = new()
    {
        ["launchmode"] = "--",
        ["gameinfo"] = "-t",
        ["placelauncherurl"] = "-j",
        ["launchtime"] = "--launchtime=",
        ["task"] = "-task",
        ["placeId"] = "-placeId",
        ["universeId"] = "-universeId",
        ["userId"] = "-userId",
    };

    // Input expects something like "launchmode:Some+placeId:1234+clientversion:2018"
    public static ParsedUri Parse(string raw)
    {
        var parsed = new ParsedUri();
        var parts = raw.Split('+', StringSplitOptions.RemoveEmptyEntries);
        var argList = new List<string>();
        var argStrPieces = new List<string>();
        var year = "2017L";

        foreach (var part in parts)
        {
            if (!part.Contains(':')) continue;
            var idx = part.IndexOf(':');
            var key = part.Substring(0, idx);
            var val = part.Substring(idx + 1);

            if (key == "clientversion" && !string.IsNullOrEmpty(val))
            {
                year = val;
                continue;
            }

            if (!UriKeyArgMap.ContainsKey(key) || string.IsNullOrEmpty(val))
                continue;

            if (key == "placelauncherurl")
                val = WebUtility.UrlDecode(val);

            var prefix = UriKeyArgMap[key];
            if (key == "launchmode")
            {
                argList.Add($"{prefix}{val}");
                argList.Add("-a");
                argList.Add("https://www.pekora.zip/Login/Negotiate.ashx");

                argStrPieces.Add($"{prefix}{val}");
                argStrPieces.Add("-a https://www.pekora.zip/Login/Negotiate.ashx");
            }
            else
            {
                if (prefix.EndsWith("="))
                {
                    argList.Add($"{prefix}{val}");
                    argStrPieces.Add($"{prefix}{val}");
                }
                else
                {
                    argList.Add(prefix);
                    argList.Add(val);
                    argStrPieces.Add($"{prefix}{val}");
                }
            }
        }

        parsed.Args = argList;
        parsed.ArgsString = string.Join(" ", argStrPieces);
        parsed.Year = year;
        return parsed;
    }
}

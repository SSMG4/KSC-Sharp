using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KSCSharp.Core;

public class FastFlagsManager
{
    private readonly string _flagsFile;

    public FastFlagsManager(string flagsFile = "fastFlags.json")
    {
        _flagsFile = flagsFile;
    }

    public Dictionary<string, object> Load()
    {
        if (!File.Exists(_flagsFile))
        {
            File.WriteAllText(_flagsFile, "{}");
            return new();
        }

        try
        {
            var json = File.ReadAllText(_flagsFile);
            if (string.IsNullOrWhiteSpace(json)) return new();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
        }
        catch (JsonException)
        {
            Console.Error.WriteLine("[!] Error reading fastFlags.json - invalid JSON format");
            return new();
        }
    }

    public void Save(Dictionary<string, object> flags)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(flags, options);
            File.WriteAllText(_flagsFile, json);
            Console.WriteLine("[*] FastFlags saved successfully!");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[!] Failed to save FastFlags: {ex.Message}");
        }
    }

    // Helper to auto-convert string input to bool/int/float/string
    public static object AutoDetectValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        var trimmed = value.Trim();
        if (bool.TryParse(trimmed, out var b)) return b;
        if (int.TryParse(trimmed, out var i)) return i;
        if (double.TryParse(trimmed, out var d)) return d;
        return value;
    }
}

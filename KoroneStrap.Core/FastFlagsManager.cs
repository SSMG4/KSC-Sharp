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

            using var doc = JsonDocument.Parse(json);
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var v = prop.Value;
                object normalized = v.ValueKind switch
                {
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number when v.TryGetInt64(out var i) => i,
                    JsonValueKind.Number when v.TryGetDouble(out var d) => d,
                    JsonValueKind.String => v.GetString() ?? "",
                    _ => v.ToString()
                };
                result[prop.Name] = normalized;
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[!] Error reading fastFlags.json: {ex.Message}");
            return new();
        }
    }

    public void Save(Dictionary<string, object> flags)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(flags, options);
            var tmp = _flagsFile + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(_flagsFile))
                File.Copy(_flagsFile, _flagsFile + ".bak", overwrite: true);
            File.Move(tmp, _flagsFile, overwrite: true);
            Console.WriteLine("[*] FastFlags saved successfully!");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[!] Failed to save FastFlags: {ex.Message}");
        }
    }

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

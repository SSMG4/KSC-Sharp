using KSCSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class FastFlagsManagerTests : IDisposable
{
    private readonly string _tmpFile = Path.GetTempFileName();

    public FastFlagsManagerTests()
    {
    }

    [Fact]
    public void Save_And_Load_ReturnsSame()
    {
        var manager = new FastFlagsManager(_tmpFile);
        var data = new Dictionary<string, object>
        {
            ["FlagA"] = true,
            ["IntVal"] = 42,
            ["FloatVal"] = 3.14
        };
        manager.Save(data);

        var loaded = manager.Load();
        Assert.Equal("True", loaded["FlagA"].ToString(), ignoreCase: true);
        Assert.Equal("42", loaded["IntVal"].ToString(), ignoreCase: true);
    }

    public void Dispose()
    {
        try { File.Delete(_tmpFile); } catch { }
    }
}

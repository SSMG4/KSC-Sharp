using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KSCSharp.Core;

public class BootstrapperDownloader
{
    private readonly HttpClient _httpClient;
    public string Url { get; init; }
    public string OutputFile { get; init; }

    public BootstrapperDownloader(string url, string outputFile = "PekoraPlayerLauncher.exe")
    {
        Url = url;
        OutputFile = outputFile;
        _httpClient = new HttpClient();
    }

    public async Task<bool> DownloadAsync(IProgress<(long downloaded, long? total)>? progress = null, CancellationToken ct = default)
    {
        try
        {
            using var resp = await _httpClient.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            var total = resp.Content.Headers.ContentLength;
            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var fs = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

            var buffer = new byte[81920];
            long totalRead = 0;
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, ct)) != 0)
            {
                await fs.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                totalRead += bytesRead;
                progress?.Report((totalRead, total));
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[!] Download failed: {ex.Message}");
            return false;
        }
    }
}

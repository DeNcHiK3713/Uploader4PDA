using Luna.ConsoleProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uploader4PDA
{
    class Program
    {
        private static SettingsManager SettingsManager = new SettingsManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
        private static AppSettings Settings = SettingsManager.GetSection<AppSettings>();
        static async Task Main(string[] args)
        {
            if (args.Length < 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("File not found!");
                return;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var fname = Path.GetFileName(args[0]);

            var file = File.OpenRead(args[0]);

            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("member_id", Settings.MemberId, "/", "4pda.ru"));
            cookieContainer.Add(new Cookie("pass_hash", Settings.PassHash, "/", "4pda.ru"));

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                UseCookies = true,
                CookieContainer = cookieContainer
            };

            var httpClient = new HttpClient(handler).AddHeaders($"https://4pda.ru/forum/index.php?showtopic={Settings.TopicId}");
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var req = new HttpRequestMessage(HttpMethod.Post, "https://4pda.ru/forum/index.php?act=attach")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "allowExt", "" },
                    { "code", "check" },
                    { "forum-attach-files", "" },
                    { "index", "1" },
                    { "maxSize", "201326592" },
                    { "md5", GetMD5(file) },
                    { "name", fname },
                    { "relId", "0" },
                    { "size", file.Length.ToString() },
                    { "topic_id", Settings.TopicId }
                })
            };

            var res = await httpClient.SendAsync(req);

            var response = (await res.Content.ReadAsStringAsync())
                .Replace("", "")
                .Replace('', '');
            if (response != "0")
            {
                var result = response.Split('');
                if (result.Length < 6)
                {
                    Console.WriteLine($"UNACCEPTABLE FILE: {fname}");
                    return;
                }

                Console.WriteLine($"File {fname} exist on server");
                return;
            }
            else
            {
                Console.Write(fname);

                var progressBar = new ConsoleProgressBar
                {
                    ForegroundColor = ConsoleColor.Cyan,
                    NumberOfBlocks = 52,
                    IncompleteBlock = " ",
                    AnimationSequence = UniversalProgressAnimations.Explosion
                };


                file.Position = 0;
                var progressContent = new ProgressStreamContent(file, CancellationToken.None);

                progressContent.Progress = (bytes, totalBytes, totalBytesExpected) =>
                {
                    if (bytes == totalBytes)
                    {
                        progressBar.Report(1);
                        Console.WriteLine();
                    }
                    else
                    {
                        progressBar.Report((double)bytes / totalBytes);
                    }
                };

                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new StringContent(Settings.TopicId), "topic_id" },
                    { new StringContent("1"), "index" },
                    { new StringContent("0"), "relId" },
                    { new StringContent("201326592"), "maxSize" },
                    { new StringContent(""), "allowExt" },
                    { new StringContent(""), "forum-attach-files" },
                    { new StringContent("upload"), "code" },
                    { progressContent, "FILE_UPLOAD", fname }
                };
                res = await httpClient.PostAsync("https://4pda.ru/forum/index.php?act=attach", form);
                progressBar.Report(1);
                response = (await res.Content.ReadAsStringAsync())
                    .Replace("", "")
                    .Replace('', '');
                var result = response.Split('');
                if (response != "1" && result.Length < 6)
                {
                    Console.WriteLine($"UNACCEPTABLE FILE: {fname}");
                    return;
                }

                Console.WriteLine($"File {fname} uploaded");
                return;

            }
        }
        public static string GetMD5(Stream file)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(file);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

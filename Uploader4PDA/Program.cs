using Luna.ConsoleProgressBar;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            FileInfo fileInfo;
            if (args.Length < 1 || !(fileInfo = new FileInfo(args[0])).Exists)
            {
                Console.WriteLine("File not found!");
                return;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var fname = Path.GetFileName(args[0]);

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
                    { "md5", GetMD5(fileInfo) },
                    { "name", fname },
                    { "relId", "0" },
                    { "size", fileInfo.Length.ToString() },
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
            }
            else
            {
                Console.Write(fname);
                Console.Write(" ");

                var progressBar = new ConsoleProgressBar
                {
                    ForegroundColor = ConsoleColor.Cyan,
                    NumberOfBlocks = 52,
                    IncompleteBlock = " ",
                    AnimationSequence = UniversalProgressAnimations.Explosion
                };


                var client = new MyWebClient(cookieContainer, 30000).AddHeaders($"https://4pda.ru/forum/index.php?showtopic={Settings.TopicId}");

                var multipart = new MultipartFormBuilder();

                multipart.AddField("topic_id", Settings.TopicId);
                multipart.AddField("index", "1");
                multipart.AddField("relId", "0");
                multipart.AddField("maxSize", "201326592");
                multipart.AddField("allowExt", "");
                multipart.AddField("forum-attach-files", "");
                multipart.AddField("code", "upload");

                multipart.AddFile("FILE_UPLOAD", fileInfo);

                var totalBytes = multipart.GetStream().Length;

                client.UploadProgressChanged += (o, e) =>
                {
                    //progressBar.Report((double)e.BytesSent / e.TotalBytesToSend); // e.TotalBytesToSend is always -1
                    progressBar.Report((double)e.BytesSent / totalBytes);
                };
                var tcs = new TaskCompletionSource<byte[]>();
                client.UploadDataCompleted += (o, e) =>
                {
                    if (e.Cancelled)
                    {
                        progressBar.Report(0);
                    }
                    else
                    {
                        progressBar.Report(1);
                    }

                    response = Encoding.UTF8.GetString(e.Result)
                        .Replace("", "")
                        .Replace('', '');
                    var result = response.Split('');
                    if (response != "1" && result.Length < 6)
                    {
                        Console.WriteLine($"UNACCEPTABLE FILE: {fname}");
                        tcs.SetResult(e.Result);
                        return;
                    }

                    Console.WriteLine();
                    Console.WriteLine($"File {fname} uploaded");
                    tcs.SetResult(e.Result);
                };
                client.UploadMultipartAsync(new Uri("https://4pda.ru/forum/index.php?act=attach"), multipart);
                await tcs.Task;
            }
        }
        public static string GetMD5(FileInfo fileInfo)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(fileInfo.FullName));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

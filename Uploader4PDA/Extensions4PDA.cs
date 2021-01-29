using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Uploader4PDA
{
    public static class Extensions4PDA
    {
        public static HttpClient AddHeaders(this HttpClient httpClient, string referrer = null)
        {
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/plain, */*; q=0.01");
            httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,en-US;q=0.9,en;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://4pda.ru");
            if (referrer != null)
            {
                httpClient.DefaultRequestHeaders.Referrer = new Uri(referrer);
            }
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");

            return httpClient;
        }
        public static WebClient AddHeaders(this WebClient webClient, string referrer = null)
        {
            webClient.Headers.Add("Accept", "text/plain, */*; q=0.01");
            webClient.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            webClient.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
            webClient.Headers.Add("Origin", "https://4pda.ru");
            if (referrer != null)
            {
                webClient.Headers.Add("Referrer" ,referrer);
            }
            webClient.Headers.Add("Sec-Fetch-Site", "same-origin");
            webClient.Headers.Add("Sec-Fetch-Mode", "cors");
            webClient.Headers.Add("Sec-Fetch-Dest", "empty");
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");

            return webClient;
        }
    }
}

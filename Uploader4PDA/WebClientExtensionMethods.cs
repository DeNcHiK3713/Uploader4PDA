using System;
using System.Net;
using System.Threading.Tasks;

namespace Uploader4PDA
{
    public static class WebClientExtensionMethods
    {
        public static byte[] UploadMultipart(this WebClient client, string address, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return client.UploadData(address, stream.ToArray());
            }
        }

        public static byte[] UploadMultipart(this WebClient client, Uri address, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return client.UploadData(address, stream.ToArray());
            }
        }

        public static byte[] UploadMultipart(this WebClient client, string address, string method, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return client.UploadData(address, method, stream.ToArray());
            }
        }

        public static byte[] UploadMultipart(this WebClient client, Uri address, string method, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return client.UploadData(address, method, stream.ToArray());
            }
        }

        public static void UploadMultipartAsync(this WebClient client, Uri address, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                client.UploadDataAsync(address, stream.ToArray());
            }
        }

        public static void UploadMultipartAsync(this WebClient client, Uri address, string method, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                client.UploadDataAsync(address, method, stream.ToArray());
            }
        }

        public static void UploadMultipartAsync(this WebClient client, Uri address, string method, MultipartFormBuilder multipart, object userToken)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                client.UploadDataAsync(address, method, stream.ToArray(), userToken);
            }
        }

        public static async Task<byte[]> UploadMultipartTaskAsync(this WebClient client, string address, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return await client.UploadDataTaskAsync(address, stream.ToArray());
            }
        }

        public static async Task<byte[]> UploadMultipartTaskAsync(this WebClient client, Uri address, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return await client.UploadDataTaskAsync(address, stream.ToArray());
            }
        }

        public static async Task<byte[]> UploadMultipartTaskAsync(this WebClient client, string address, string method, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return await client.UploadDataTaskAsync(address, method, stream.ToArray());
            }
        }

        public static async Task<byte[]> UploadMultipartTaskAsync(this WebClient client, Uri address, string method, MultipartFormBuilder multipart)
        {
            client.Headers.Add(HttpRequestHeader.ContentType, multipart.ContentType);

            using (var stream = multipart.GetStream())
            {
                return await client.UploadDataTaskAsync(address, method, stream.ToArray());
            }
        }
    }
}
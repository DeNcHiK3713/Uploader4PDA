using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uploader4PDA
{
    public class MultipartFormBuilder
    {
        static readonly string MultipartContentType = "multipart/form-data; boundary=";
        static readonly string FileHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
        static readonly string FormDataTemplate = "\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\";\r\n\r\n{2}";

        public string ContentType { get; private set; }

        string Boundary { get; set; }

        Dictionary<string, FileInfo> FilesToSend { get; set; } = new Dictionary<string, FileInfo>();
        Dictionary<string, string> FieldsToSend { get; set; } = new Dictionary<string, string>();

        public MultipartFormBuilder()
        {
            Boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            ContentType = MultipartContentType + Boundary;
        }

        public void AddField(string key, string value)
        {
            FieldsToSend.Add(key, value);
        }

        public void AddFile(FileInfo file)
        {
            string key = file.Extension.Substring(1);
            FilesToSend.Add(key, file);
        }

        public void AddFile(string key, FileInfo file)
        {
            FilesToSend.Add(key, file);
        }

        public MemoryStream GetStream()
        {
            var memStream = new MemoryStream();

            WriteFields(memStream);
            WriteStreams(memStream);
            WriteTrailer(memStream);

            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        void WriteFields(Stream stream)
        {
            if (FieldsToSend.Count == 0)
                return;

            foreach (var fieldEntry in FieldsToSend)
            {
                string content = string.Format(FormDataTemplate, Boundary, fieldEntry.Key, fieldEntry.Value);

                using (var fieldData = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    fieldData.CopyTo(stream);
                }
            }
        }

        void WriteStreams(Stream stream)
        {
            if (FilesToSend.Count == 0)
                return;

            foreach (var fileEntry in FilesToSend)
            {
                WriteBoundary(stream);

                string header = string.Format(FileHeaderTemplate, fileEntry.Key, fileEntry.Value.Name);
                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileData = File.OpenRead(fileEntry.Value.FullName))
                {
                    fileData.CopyTo(stream);
                }
            }
        }

        void WriteBoundary(Stream stream)
        {
            byte[] boundarybytes = Encoding.UTF8.GetBytes("\r\n--" + Boundary + "\r\n");
            stream.Write(boundarybytes, 0, boundarybytes.Length);
        }

        void WriteTrailer(Stream stream)
        {
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + Boundary + "--\r\n");
            stream.Write(trailer, 0, trailer.Length);
        }
    }
}
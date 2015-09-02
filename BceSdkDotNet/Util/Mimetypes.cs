using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using log4net;

namespace BaiduBce.Util
{
    public static class MimeTypes
    {
        public const string MimeTypeOctetStream = "application/octet-stream";

        private static readonly ILog log = LogManager.GetLogger(typeof(MimeTypes));

        private static IDictionary<string, string> extensionToMimetypeMap = new Dictionary<string, string>();

        static MimeTypes()
        {
            Stream stream = typeof(MimeTypes).Assembly.GetManifestResourceStream("BaiduBce.mime.types");
            if (stream != null)
            {
                using (stream)
                {
                    LoadAndReplaceMimetypes(stream);
                }
            }
            else
            {
                log.Warn("Unable to find 'mime.types'");
            }
        }

        private static void LoadAndReplaceMimetypes(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            string line = null;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.Length == 0)
                {
                    continue;
                }
                string[] lines = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    string mimetype = lines[0];
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string extension = lines[i];
                        extensionToMimetypeMap[extension.ToLower()] = mimetype;
                    }
                }

            }
        }

        public static string GetMimetype(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return MimeTypeOctetStream;
            }
            string mimetype;
            if (extensionToMimetypeMap.TryGetValue(extension, out mimetype))
            {
                return mimetype;
            }
            else
            {
                return MimeTypeOctetStream;
            }
        }

        public static string GetMimetype(FileInfo fileInfo)
        {
            return GetMimetype(fileInfo.Extension);
        }
    }
}

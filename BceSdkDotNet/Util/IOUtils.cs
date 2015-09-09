// Copyright (c) 2014 Baidu.com, Inc. All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaiduBce.Util
{
    public static class IOUtils
    {
        public const int DefaultBufferSize = 8192;

        public static Stream GetMemoryStream(Stream sourceStream)
        {
            return GetMemoryStream(sourceStream, DefaultBufferSize);
        }

        public static Stream GetMemoryStream(Stream sourceStream, int bufferSize)
        {
            Stream resultStream = new MemoryStream();
            var buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                resultStream.Write(buffer, 0, bytesRead);
            }
            return resultStream;
        }

        public static byte[] StreamToBytes(Stream sourceStream)
        {
            return StreamToBytes(sourceStream, sourceStream.Length, DefaultBufferSize);
        }

        public static byte[] StreamToBytes(Stream sourceStream, long streamLength, int bufferSize)
        {
            var result = new byte[streamLength];
            var buffer = new byte[bufferSize];
            int totalBytesRead = 0;
            int bytesRead = 0;
            while ((bytesRead = sourceStream.Read(buffer, 0, bufferSize)) > 0)
            {
                Array.Copy(buffer, 0, result, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;
            }
            return result;
        }

        public static void StreamToFile(Stream sourceStream, FileInfo destinationFileInfo, int bufferSize)
        {
            // attempt to create the parent if it doesn't exist
            DirectoryInfo parentDirectory = destinationFileInfo.Directory;
            if (parentDirectory != null && !parentDirectory.Exists)
            {
                parentDirectory.Create();
            }
            using (FileStream fileStream = destinationFileInfo.Create())
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
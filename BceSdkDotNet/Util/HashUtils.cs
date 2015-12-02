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
using System.Security.Cryptography;

namespace BaiduBce.Util
{
    public static class HashUtils
    {
        public static string ComputeSHA256Hash(FileInfo fileInfo)
        {
            using (Stream stream = fileInfo.OpenRead())
            {
                byte[] bytes = IOUtils.StreamToBytes(stream);
                SHA256 shaM = new SHA256Managed();
                byte[] result = shaM.ComputeHash(bytes);
                return Convert.ToBase64String(result);
            }
        }

        public static string ComputeMD5Hash(FileInfo fileInfo)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fileInfo.OpenRead())
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public static string ComputeMD5HashWithBase64(FileInfo fileInfo)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fileInfo.OpenRead())
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }

        public static string ComputeMD5Hash(Stream stream, long contentLength)
        {
            using (var md5 = MD5.Create())
            {
                long position = stream.Position;

                byte[] temp = new byte[4096];
                long offset = 0;
                int size = 0;

                while (offset < contentLength)
                {
                    size = (contentLength - offset) > 4096 ? 4096 : (int)(contentLength - offset);
                    size = stream.Read(temp, 0, size);
                    if (size <= 0)
                    {
                        break;
                    }
                    offset += size;
                    md5.TransformBlock(temp, 0, size, temp, 0);
                }

                md5.TransformFinalBlock(new byte[0], 0, 0);
                stream.Position = position;
                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
    }
}
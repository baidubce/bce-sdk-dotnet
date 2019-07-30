﻿// Copyright 2014 Baidu, Inc.
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

namespace BaiduBce.Services.Bos
{

    public static class BosConstants
    {
        public static class Permission
        {
            public const string FullControl = "FULL_CONTROL";
            public const string Read = "READ";
            public const string Write = "WRITE";
        }

        public static class CannedAcl
        {
            public const string Private = "private";
            public const string PublicRead = "public-read";
            public const string PublicReadWrite = "public-read-write";
        }

        /// <summary>
        /// storage class of object
        /// </summary>
        public static class StorageClass
        {
            public const string Standard = "STANDARD";
            public const string StandardInfrequentAccess = "STANDARD_IA";
        }
    }
}
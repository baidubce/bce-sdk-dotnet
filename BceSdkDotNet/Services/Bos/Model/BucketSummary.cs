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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Represents an Baidu Bos bucket.
    /// 
    /// <para>
    /// Every object stored in Baidu Bos is contained within a bucket. Buckets partition the namespace of objects stored
    /// in Baidu Bos at the top level. Within a bucket, any name can be used for objects. However, bucket names must be
    /// unique across all of Baidu Bos.
    /// 
    /// </para>
    /// <para>
    /// There are no limits to the number of objects that can be stored in a bucket. Performance does not vary based on
    /// the number of buckets used. Store all objects within a single bucket or organize them across several buckets.
    /// </para>
    /// </summary>
    public class BucketSummary
    {
        /// <summary>
        /// The name of this Baidu Bos bucket.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date this bucket was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The location of this Baidu Bos bucket.
        /// </summary>
        public string Location { get; set; }
    }
}
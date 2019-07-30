// Copyright 2014 Baidu, Inc.
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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Provides options for downloading an Baidu Bos object.
    /// 
    /// <para>
    /// All <code>GetObjectRequests</code> must specify a bucket name and key.
    /// </para>
    /// Beyond that, requests can also specify: * <para>
    /// </para>
    ///  <para>
    /// <ul>
    ///   <li>The range of bytes within the object to download,
    /// </ul>
    /// </para>
    /// </summary>
    public class GetObjectRequest : ObjectRequestBase
    {
        /// <summary>
        /// Optional member indicating the byte range of data to retrieve
        /// </summary>
        public long[] Range { get; set; }

        /// <summary>
        /// Sets the optional inclusive byte range within the desired object that will be downloaded by this request.
        /// 
        /// <para>
        /// The first byte in an object has  position 0; as an example, the first ten bytes of an object can be
        /// downloaded by specifying a range of 0 to 9.
        /// 
        /// </para>
        /// <para>
        /// If no byte range is specified, this request downloads the entire object from Baidu Bos.
        /// 
        /// </para>
        /// </summary>
        /// <param name="start"> The start of the inclusive byte range to download. </param>
        /// <param name="end"> The end of the inclusive byte range to download. </param>
        public void SetRange(long start, long end)
        {
            if (start < 0)
            {
                throw new ArgumentException("start should be non-negative");
            }
            if (start > end)
            {
                throw new ArgumentException("start should not be greater than end");
            }
            Range = new long[] {start, end};
        }
    }
}
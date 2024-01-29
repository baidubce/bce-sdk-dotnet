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
using BaiduBce.Model;

namespace BaiduBce.Services.Bos.Model
{
    public class ObjectRequestBase : BucketRequestBase
    {
        private const int MaxObjectKeyLength = 1024;

        private string _key;

        /// <summary>
        /// Limit the read and write rate of object
        /// </summary>
        private long _trafficLimit;
        
        public string Key
        {
            get { return _key; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("object key should not be null or empty");
                }
                if (value.Length > MaxObjectKeyLength)
                {
                    throw new ArgumentException("objectKey should not be greater than " + MaxObjectKeyLength + ".");
                }
                _key = value;
            }
        }

        public long TrafficLimit
        {
            get { return _trafficLimit; }
            set
            {
                if (value == 0 || (value >= 819200 && value <= 838860800))
                {
                    _trafficLimit = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Traffic limit must be between 819200(bit/s) and 838860800(bit/s).");
                }
            }
        }
    }
}
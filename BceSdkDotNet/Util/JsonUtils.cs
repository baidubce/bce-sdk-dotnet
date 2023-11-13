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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BaiduBce.Util
{
    public static class JsonUtils
    {
        private static JsonSerializer serializer = new JsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static T ToObject<T>(StreamReader input)
        {
            using (JsonReader jsonReader = new JsonTextReader(input))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }
    }
}
//// Copyright (c) 2014 Baidu.com, Inc. All Rights Reserved
////
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//// the License. You may obtain a copy of the License at
////
//// http://www.apache.org/licenses/LICENSE-2.0
////
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//// specific language governing permissions and limitations under the License.

//using System;
//using System.Collections.Generic;

//namespace BaiduBce.Util.Json.Mapper
//{
//    public delegate object Deserializer(JsonLexer lexer);

//    public static class ObjectMapper<T>
//    {
//        public static IDictionary<string, Deserializer> deserializerMap = new Dictionary<string, Deserializer>();

//        public static ObjectMapper()
//        {
//            foreach (var property in typeof(T).GetProperties())
//            {
//                if (property.CanRead && property.CanWrite)
//                {
//                    string name = Char.ToLower(property.Name[0]) + property.Name.Substring(1);
//                    var type = property.PropertyType;
//                    if (type == typeof(int))
//                    {
//                        deserializerMap.Add(name, DeserializeInt);
//                    }
//                }
//            }
//        }

//        public static object DeserializeInt(JsonLexer lexer)
//        {
//            lexer.ReadNextToken();
//            if (lexer.TokenType != JsonTokenType.Long)
//            {
//                throw new JsonParseException(
//                    "Line " + lexer.TokenLine + " column " + lexer.TokenColumn + ": int expected");
//            }
//            return lexer.TokenValue;
//        }

//        public static object Deserialize(JsonLexer lexer)
//        {
//            return default(T);
//        }
//    }
//}


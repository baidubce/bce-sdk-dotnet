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
using System.IO;
using System.Text;

namespace BaiduBce.Util.Json
{
    internal class JsonLexer
    {
        private StreamReader input;

        private int nextCharBuffer = -1;
        private int currentLine = 1;
        private int currentColumn = 0;

        public JsonTokenType TokenType { get; set; }

        public object TokenValue { get; set; }

        public int TokenLine { get; set; }

        public int TokenColumn { get; set; }

        public JsonLexer(StreamReader input)
        {
            this.input = input;
        }

        public void ReadNextToken()
        {
            int nextChar;
            do
            {
                nextChar = this.ReadNextChar();
            } while (nextChar == ' ' || nextChar >= '\t' && nextChar <= '\r');

            this.TokenLine = this.currentLine;
            this.TokenColumn = this.currentColumn;

            switch (nextChar)
            {
                case -1:
                    this.TokenType = JsonTokenType.End;
                    return;
                case '{':
                    this.TokenType = JsonTokenType.OpeningBrace;
                    return;
                case '}':
                    this.TokenType = JsonTokenType.ClosingBrace;
                    return;
                case '[':
                    this.TokenType = JsonTokenType.OpeningBracket;
                    return;
                case ']':
                    this.TokenType = JsonTokenType.ClosingBracket;
                    return;
                case ':':
                    this.TokenType = JsonTokenType.Colon;
                    return;
                case ',':
                    this.TokenType = JsonTokenType.Comma;
                    return;
                case '"':
                    this.TokenType = JsonTokenType.String;
                    this.TokenValue = ReadJsonString();
                    return;
                case 't':
                    this.ReadTrue();
                    this.TokenType = JsonTokenType.Bool;
                    this.TokenValue = true;
                    return;
                case 'f':
                    this.ReadFalse();
                    this.TokenType = JsonTokenType.Bool;
                    this.TokenValue = false;
                    return;
                case 'n':
                    this.ReadNull();
                    this.TokenType = JsonTokenType.Null;
                    return;
            }

            if (nextChar == '-' || nextChar >= '0' && nextChar <= '9')
            {
                StringBuilder builder = new StringBuilder(128);
                builder.Append(nextChar);
                this.ReadNumber(builder);
            }
        }

        public string ReadJsonString()
        {
            StringBuilder builder = new StringBuilder(128);
            builder.Append('"');
            for (;;)
            {
                int nextChar = this.ReadNextChar();
                builder.Append(nextChar);
                if (nextChar < 0 || nextChar == '\n')
                {
                    throw new JsonParseException("Malformed Json: Unclosed string");
                }
                if (nextChar == '"')
                {
                    return builder.ToString();
                }
                if (nextChar == '\\')
                {
                    builder.Append(this.ReadEscapedChar());
                }
            }
        }

        public char ReadEscapedChar()
        {
            int nextChar = this.ReadNextChar();
            switch (nextChar)
            {
                case '"':
                    return '"';
                case '\\':
                    return '\\';
                case '/':
                    return '/';
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'u':
                    return this.ReadEspacedUnicodeChar();
                default:
                    throw new JsonParseException("Malformed Json: Invalid escaped sequence.");
            }
        }

        public char ReadEspacedUnicodeChar()
        {
            int value = 0;
            for (int i = 0; i < 4; ++i)
            {
                int nextChar = this.ReadNextChar();
                value *= 16;
                if (nextChar >= '0' && nextChar <= '9')
                {
                    value += nextChar - '0';
                }
                else if (nextChar >= 'A' && nextChar <= 'F')
                {
                    value += nextChar - 'A' + 10;
                }
                else if (nextChar >= 'a' && nextChar <= 'f')
                {
                    value += nextChar - 'a' + 10;
                }
                else
                {
                    throw new JsonParseException("Malformed Json: Invalid escpaed unicode sequence.");
                }
            }
            return (char) value;
        }

        public void ReadNumber(StringBuilder builder)
        {
            for (;;)
            {
                int nextChar = this.ReadNextChar();
                if (nextChar == '.' || nextChar == 'e' || nextChar == 'E')
                {
                    builder.Append(nextChar);
                    this.ReadDouble(builder);
                }
                if (nextChar < '0' || nextChar > '9')
                {
                    this.PutBackChar(nextChar);
                    this.TokenType = JsonTokenType.Long;
                    try
                    {
                        this.TokenValue = long.Parse(builder.ToString());
                    }
                    catch (FormatException e)
                    {
                        throw new JsonParseException("Malformed Json: Fail to parse number.", e);
                    }
                    catch (OverflowException e)
                    {
                        throw new JsonParseException("Malformed Json: Number too large.", e);
                    }
                }
                builder.Append(nextChar);
            }
        }

        public void ReadDouble(StringBuilder builder)
        {
            int nextChar = this.ReadNextChar();
            if (nextChar == '+' || nextChar == '-' || nextChar >= '0' && nextChar <= '9')
            {
                builder.Append(nextChar);
                for (;;)
                {
                    nextChar = this.ReadNextChar();
                    if (nextChar < '0' || nextChar > '9')
                    {
                        break;
                    }
                }
            }
            this.TokenType = JsonTokenType.Double;
            try
            {
                this.TokenValue = double.Parse(builder.ToString());
            }
            catch (FormatException e)
            {
                throw new JsonParseException("Malformed Json: Fail to parse number.", e);
            }
            catch (OverflowException e)
            {
                throw new JsonParseException("Malformed Json: Number too large.", e);
            }
            this.PutBackChar(nextChar);
        }

        public void ReadTrue()
        {
            if (this.ReadNextChar() != 'r' || this.ReadNextChar() != 'u' || this.ReadNextChar() != 'e')
            {
                throw new JsonParseException("Malformed Json: Unrecognized token, 'true' expected.");
            }
        }

        public void ReadFalse()
        {
            if (this.ReadNextChar() != 'a'
                || this.ReadNextChar() != 'l'
                || this.ReadNextChar() != 's'
                || this.ReadNextChar() != 'e')
            {
                throw new JsonParseException("Malformed Json: Unrecognized token, 'false' expected.");
            }
        }

        public void ReadNull()
        {
            if (this.ReadNextChar() != 'u'
                || this.ReadNextChar() != 'l'
                || this.ReadNextChar() != 'l')
            {
                throw new JsonParseException("Malformed Json: Unrecognized token, 'null' expected.");
            }
        }


        private int ReadNextChar()
        {
            int ret;
            if (this.nextCharBuffer >= 0)
            {
                ret = this.nextCharBuffer;
                this.nextCharBuffer = -1;
            }
            else
            {
                for (;;)
                {
                    ret = this.input.Read();
                    if (ret != '\n' && ret != '\r')
                    {
                        break;
                    }
                    ++this.currentLine;
                    this.currentColumn = 0;
                }
            }
            ++this.currentColumn;
            return ret;
        }

        private void PutBackChar(int ch)
        {
            this.nextCharBuffer = ch;
            --this.currentColumn;
        }
    }
}
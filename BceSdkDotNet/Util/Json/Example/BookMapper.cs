//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BaiduBce.Util.Json.Example
//{
//    public class BookMapper
//    {
//        public static Book Deserialize(JsonLexer lexer)
//        {
//            lexer.ReadNextToken();
//            if (lexer.TokenType != JsonTokenType.OpeningBrace)
//            {
//                throw new JsonParseException("Fail to deserialize to Book: '{' expected.");
//            }
//            Book ret = new Book();
//            for (; ; )
//            {
//                lexer.ReadNextToken();
//                if (lexer.TokenType == JsonTokenType.ClosingBrace)
//                {
//                    break;
//                }
//                if (lexer.TokenType != JsonTokenType.String)
//                {
//                    throw new JsonParseException("Malformed Json: Invalid object format, key expected.");
//                }
//                string key = lexer.TokenValue as string;
//                lexer.ReadNextToken();
//                if (lexer.TokenType != JsonTokenType.Colon)
//                {
//                    throw new JsonParseException("Malformed Json: Invalid object format, ':' expected.");
//                }
//                if (key == "author")
//                {
//                    ret.Author = ObjectMapper<Author>.Deserialize(lexer);
//                }
//                else if (key == "date")
//                {
//                    if (lexer.TokenType != JsonTokenType.String)
//                    {
//                        throw new JsonParseException(
//                            "Fail to deserialize to Book: Field '" + key + "' should be of type string.");
//                    }
//                    ret.PublishDate = DateTimeMapper.Deserialize(lexer);
//                }
//                else if (key == "title")
//                {
//                    ret.Title = StringMapper.Deserialize(lexer);
//                }
//                else if (key == "price")
//                {
//                    ret.Price = DoubleMapper.Deserialize(lexer);
//                }
//                else
//                {
//                    new JsonParseException("Fail to deserialize to Book: Unexpected key '" + key + "'");
//                }
//            }

//            return null;
//        }
//    }
//}

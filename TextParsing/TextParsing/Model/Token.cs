using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextParsing.Enums;

namespace TextParsing.Model
{
    public class Token
    {
        public TokenType TokenType;
        public Type Type { get; set; }

        public string Text { get; set; }

        public Token(TokenType tokenType, string text, Type type = null)
        {
            TokenType = tokenType;
            Text = text;
            Type = type;
        }
        public override string ToString()
        {
            return $"`{Text}`";
        }
    }
}

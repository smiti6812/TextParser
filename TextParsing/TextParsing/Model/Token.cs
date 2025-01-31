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

        public string Text { get; set; }

        public Token(TokenType tokenType, string text)
        {
            TokenType = tokenType;
            Text = text;
        }
        public override string ToString()
        {
            return $"`{Text}`";
        }
    }
}

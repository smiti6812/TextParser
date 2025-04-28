using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextParsing.Model;

namespace TextParsing.Interfaces
{
    public interface ITextSplitter
    {
        Queue<Token> Tokens { get; set; }
        IList<Token> Lex(string input);
        CultureInfo CultureInfo { get; set; }
        Type GetPropertyType(string field);
    }
}

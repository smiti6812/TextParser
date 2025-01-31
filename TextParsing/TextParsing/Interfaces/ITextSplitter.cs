using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextParsing.Model;

namespace TextParsing.Interfaces
{
    public interface ITextSplitter
    {
        Queue<Token> Tokens { get; set; }
    }
}

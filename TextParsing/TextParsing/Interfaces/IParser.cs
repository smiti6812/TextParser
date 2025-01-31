using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextParsing.Model;

namespace TextParsing.Interfaces
{
    public interface IParser
    {
        bool Evaluate<T>(T ch);        
    }
}

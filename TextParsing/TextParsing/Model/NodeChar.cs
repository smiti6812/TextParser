using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeChar : ExpressionNode
    {       
        public char ConstantValue { get; set; }
        public NodeChar(char constantValue) => ConstantValue = constantValue;
       
    }
}

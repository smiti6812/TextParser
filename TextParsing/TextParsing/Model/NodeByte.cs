using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeByte : ExpressionNode
    {       
        public byte ConstantValue { get; set; }
        public NodeByte(byte constantValue) => ConstantValue = constantValue;
        
    }
}

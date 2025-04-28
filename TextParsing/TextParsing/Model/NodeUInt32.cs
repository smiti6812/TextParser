using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeUInt32 : ExpressionNode
    {        
        public UInt32 ConstantValue { get; set; }
        public NodeUInt32(UInt32 constantValue) => ConstantValue = constantValue;       
    }
}

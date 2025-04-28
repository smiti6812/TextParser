using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeUInt64 : ExpressionNode
    {
        public UInt64 ConstantValue { get; set; }
        public NodeUInt64(UInt64 constantValue) => ConstantValue = constantValue;

    }
}

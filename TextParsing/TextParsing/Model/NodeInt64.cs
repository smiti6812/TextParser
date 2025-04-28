using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeInt64 : ExpressionNode
    {       
        public Int64 ConstantValue { get; set; }
        public NodeInt64(long constantValue) => ConstantValue = constantValue;       
    }
}

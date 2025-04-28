using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeInt32 : ExpressionNode
    {        
        public Int32 ConstantValue { get; set; }
        public NodeInt32(int constantValue) => ConstantValue = constantValue;       
    }
}

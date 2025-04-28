using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeSingle : ExpressionNode
    {        
        public float ConstantValue { get; set; }
        public NodeSingle(float constantValue) => ConstantValue = constantValue;        
    }
}

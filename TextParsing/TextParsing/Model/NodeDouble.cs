using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeDouble : ExpressionNode
    {        
        public double ConstantValue { get; set; }
        public NodeDouble(double constantValue) => ConstantValue = constantValue;
        
    }
}

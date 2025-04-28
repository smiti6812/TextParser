using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeDecimal : ExpressionNode
    {      
        public decimal ConstantValue { get; set; }
        public NodeDecimal(decimal constantValue) => ConstantValue = constantValue;        
    }
}

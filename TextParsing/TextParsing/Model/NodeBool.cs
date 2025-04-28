using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeBool : ExpressionNode
    {        
        public bool ConstantValue { get; set; }
        public NodeBool(bool constantValue) => ConstantValue = constantValue;       
    }
}

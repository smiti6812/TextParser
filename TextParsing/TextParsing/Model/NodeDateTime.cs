using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeDateTime : ExpressionNode
    {       
        public DateTime ConstantValue { get; set; }
        public NodeDateTime(DateTime constantValue) => ConstantValue = constantValue;        
       
    }
}

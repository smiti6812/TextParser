using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeString : ExpressionNode
    {        
        public string ConstantValue { get; set; }
        public NodeString(string constantValue) => ConstantValue = constantValue;       
    }
}

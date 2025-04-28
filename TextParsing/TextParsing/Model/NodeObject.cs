using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class NodeObject : ExpressionNode
    {
        public object ConstantValue { get; set; }
        public NodeObject(object constantValue) => ConstantValue = constantValue;
    }
}

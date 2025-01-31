using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class ExpressionNode
    {
        public ExpressionNode Parent { get; set; }
        public int Level { get; set; }
        public Token Token { get; set; }
        public string Text { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public IList<ExpressionNode> Children { get; set; }
    }
}

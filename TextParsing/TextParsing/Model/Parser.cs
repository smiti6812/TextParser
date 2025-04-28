using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using TextParsing.Enums;
using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public class Parser : IParser
    {
        private bool result;
        protected Queue<Token> Tokens;
        private Queue<Token> brackets;
        private readonly int numberOfVariables;
        private ITextSplitter textSplitter;
        private readonly string pattern = @"(<=)|(<)|(>=)|(>)|(==)|(!=)|(StartWith)|(EndWith)|(Contains)";
        private readonly string datePattern = @"(-)|(:)|(\s)|(\.)";
        private IList<Token> processedTokens;
        protected CultureInfo CultureInfo;

        public ExpressionNode Root { get; set; }

        public Parser(ITextSplitter _textSplitter)
        {
            textSplitter = _textSplitter;
            CultureInfo = _textSplitter.CultureInfo;
            Tokens = textSplitter.Tokens;
            numberOfVariables = Tokens.Where(f => f.TokenType == TokenType.Variable).Count();
            processedTokens = new List<Token>();
            brackets = new Queue<Token>();
            Parse();
        }

        protected virtual void Parse()
        {
            Root = ParseExpression(null);
            while (Root.Parent != null && Root.Parent.Children != null)
            {
                Root = Root.Parent;
            }
        }

        private ExpressionNode ParseExpression(ExpressionNode root)
        {
            var expr = new ExpressionNode();
            if (root != null && brackets.Count == 0)
            {
                expr.Parent = root;
                if (root.Left != null && root.Right == null)
                {
                    root.Right = expr;
                    if (root.Children == null)
                    {
                        root.Children = new List<ExpressionNode>();
                        root.Children.Add(expr);
                    }
                    else
                    {
                        root.Children.Add(expr);
                    }
                }
            }
            var sb = new StringBuilder();
            bool leftHandeled = false;
            while (Tokens.Count > 0)
            {
                var token = Tokens.Dequeue();
                switch (token.TokenType)
                {
                    case TokenType.LessThanEqual:
                    case TokenType.LessThan:
                    case TokenType.GreaterThanEqual:
                    case TokenType.Constant:
                    case TokenType.NotEqual:
                    case TokenType.GreaterThan:
                    case TokenType.Equal:
                    case TokenType.CallOpeningBracket:
                    case TokenType.CallClosingBracket:
                    case TokenType.Call:
                    case TokenType.Variable:
                        if (token.TokenType == TokenType.Variable)
                        {
                            sb = new StringBuilder();
                        }
                        sb.Append(token.Text);
                        if (token.TokenType == TokenType.Constant)
                        {
                            string[] cmd = Regex.Split(sb.ToString(), pattern);
                            var node = ConstVariableTypeValueProvider.ReturnExpressionNode(token.Type.Name, cmd[2].Replace("(", "").Replace(")", ""), CultureInfo);
                            node.Text = sb.ToString();
                            node.ValueType = token.Type;
                            node.Token = processedTokens[processedTokens.Count - 1].Text == "(" ? new Token(TokenType.Call, processedTokens[processedTokens.Count - 2].Text) : processedTokens[processedTokens.Count - 1];

                            if (Tokens.Count > 0 && Tokens.Peek().TokenType == TokenType.CallClosingBracket)
                            {
                                node.Text += ")";
                            }
                            node.Parent = expr;
                            if (numberOfVariables == 1)
                            {
                                expr = node;
                                node.Parent = null;
                                break;
                            }

                            if (!leftHandeled)
                            {
                                expr.Children = new List<ExpressionNode>();
                                expr.Children.Add(node);
                                expr.Left = node;
                                leftHandeled = true;
                            }
                            else
                            {
                                expr.Right = node;
                                expr.Children.Add(node);
                                _ = ParseExpression(expr);
                            }
                        }
                        processedTokens.Add(token);
                        break;
                    case TokenType.OpenParenthesis:
                        brackets.Enqueue(token);
                        break;
                    case TokenType.CloseParenthesis:
                        _ = brackets.Dequeue();
                        break;
                    case TokenType.And:
                    case TokenType.Or:
                        expr.Token = token;
                        expr.Text = token.Text;
                        if (expr.Left == null)
                        {
                            if (expr.Children == null)
                            {
                                expr.Children = new List<ExpressionNode>();
                                expr.Children.Add(root);
                            }
                            else
                            {
                                expr.Children.Add(root);
                            }

                            expr.Left = root;
                            expr.Left.Text = root.Text;
                            root.Parent = expr;
                            leftHandeled = true;
                            if (root != null && brackets.Count == 0)
                            {
                                _ = ParseExpression(expr);
                            }
                        }

                        if (Tokens.Count < 8 || expr.Right == null)
                        {
                            break;
                        }

                        break;
                }
            }

            return expr;
        }


        public bool Evaluate<T>(T ch) => EvaluateExpression(Root, ch);

        private bool EvaluateExpression<T>(ExpressionNode root, T ch)
        {
            if (root == null)
            {
                return result;
            }

            bool resultLeft = EvaluateExpression(root.Left, ch);
            bool resultRight = EvaluateExpression(root.Right, ch);
            if (!string.IsNullOrEmpty(root.Text))
            {
                string[] cmd = Regex.Split(root.Text, pattern);
                string operandus = cmd.Length == 1 && (cmd[0] == "Or" || cmd[0] == "And") ? cmd[0] : cmd[1];
                if (operandus != "Or" && operandus != "And")
                {
                    var variableValue = ConstVariableTypeValueProvider.GetPropertyValue<T>(cmd[0].Replace(".", ""), ch);
                    var constantValue = ConstVariableTypeValueProvider.GetExpressionNodeDescendant(root).ConstantValue;
                    result = operandus switch
                    {
                        "==" => variableValue == constantValue,
                        "!=" => variableValue != constantValue,
                        ">=" => variableValue >= constantValue,
                        ">" => variableValue > constantValue,
                        "<=" => variableValue <= constantValue,
                        "<" => variableValue < constantValue,
                        "StartWith" => variableValue.StartsWith(constantValue),
                        "EndWith" => variableValue.EndsWith(constantValue),
                        "Contains" => variableValue.Contains(constantValue),
                        _ => throw new InvalidOperationException($"Invalid operation: {cmd[1]}")
                    };
                }
                else
                {
                    result = operandus switch
                    {
                        "And" => resultLeft && resultRight,
                        "Or" => resultLeft || resultRight,
                    };
                }
            }

            return result;
        }
    }
}


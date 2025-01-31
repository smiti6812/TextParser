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
        private readonly Queue<Token> tokens;
        private Queue<Token> brackets;
        private readonly int numberOfVariables;
        private ITextSplitter textSplitter;
        private readonly string pattern = @"(<=)|(<)|(>=)|(>)|(==)|(!=)|(StartWith)|(EndWith)|(Contains)";
        private readonly string datePattern = @"(-)|(:)|(\s)|(\.)";
        private IList<Token> processedTokens;
        private readonly CultureInfo cultureInfo;

        public ExpressionNode Root { get; set; }

        public Parser(ITextSplitter _textSplitter, CultureInfo culture)
        {
            textSplitter = _textSplitter;
            cultureInfo = culture;
            tokens = textSplitter.Tokens;
            numberOfVariables = tokens.Where(f => f.TokenType == TokenType.Variable).Count();
            processedTokens = new List<Token>();
            brackets = new Queue<Token>();
            Parse();
        }

        private DateTime ReturnDateTime(string dateStr)
        {
            string[] dateStrArr = Regex.Split(dateStr, datePattern);
            var dateStrList = dateStrArr.Where(c => c != " " && c != "-" && c != ":" && c != "." && c != "").ToList();

            var dateIntList = new List<int>();
            dateStrList.ForEach(d =>
            {
                if (d.Length == 2 && d.StartsWith("0"))
                {
                    dateIntList.Add(int.Parse(d.Substring(1, 1)));
                }
                else
                {
                    dateIntList.Add(int.Parse(d));
                }
            });

            return cultureInfo.Name switch
            {
                "hu-HU" => new DateTime(dateIntList[0], dateIntList[1], dateIntList[2], dateIntList[3], dateIntList[4], dateIntList[5]),
                "de-DE" => new DateTime(dateIntList[0], dateIntList[1], dateIntList[2], dateIntList[3], dateIntList[4], dateIntList[5]),
                "en-UK" => new DateTime(dateIntList[2], dateIntList[1], dateIntList[0], dateIntList[3], dateIntList[4], dateIntList[5]),
                "en-US" => new DateTime(dateIntList[2], dateIntList[0], dateIntList[1], dateIntList[3], dateIntList[4], dateIntList[5]),
            };
        }

        private void Parse()
        {
            Root = ParseExpression(null);
            while (Root.Parent != null)
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
            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
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
                            var node = new ExpressionNode()
                            {
                                Text = sb.ToString()
                            };

                            node.Token = processedTokens[processedTokens.Count - 1].Text == "(" ? new Token(TokenType.Call, processedTokens[processedTokens.Count - 2].Text) : processedTokens[processedTokens.Count - 1];
                            node.Parent = expr;
                            if (tokens.Peek().TokenType == TokenType.CallClosingBracket)
                            {
                                node.Text += ")";
                            }

                            if (numberOfVariables == 1)
                            {
                                expr = node;
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

                        if (tokens.Count < 8 || expr.Right == null)
                        {
                            break;
                        }
                        /*
                        if (brackets.Count == 0)
                        {
                            _ = ParseExpression(expr);
                        }
                        */
                        break;
                }
            }

            return expr;
        }

        private Int32 ReturnInt32Value(string value) => Convert.ToInt32(value, CultureInfo.InvariantCulture);
        private UInt32 ReturnUInt32Value(string value) => Convert.ToUInt32(value, CultureInfo.InvariantCulture);
        private UInt64 ReturnUInt64Value(string value) => Convert.ToUInt64(value, CultureInfo.InvariantCulture);
        private Int64 ReturnInt64Value(string value) => Convert.ToInt64(value, CultureInfo.InvariantCulture);        private decimal ReturnDecimalValue(string value) => Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        private Double ReturnDoubleValue(string value) => Convert.ToDouble(value, CultureInfo.InvariantCulture);
        private float ReturnFloatValue(string value) => Convert.ToSingle(value, CultureInfo.InvariantCulture);
        private bool ReturnBoolValue(string value) => Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        private char ReturnCharValue(string value) => Convert.ToChar(value, CultureInfo.InvariantCulture);
        private byte ReturnByteValue(string value) => Convert.ToByte(value, CultureInfo.InvariantCulture);
        private DateTime ReturnDateTimeValue(string value) => ReturnDateTime(value);
        private bool EvaluateValueAndConstantNotEqual(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "String":
                    return fieldValue.ToString() != constant;
                case "Int32":
                    return Convert.ToInt32(fieldValue) != ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) != ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) != ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) != ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) != ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) != ReturnDoubleValue(constant);
                case "Bool":
                    return Convert.ToBoolean(fieldValue) != ReturnBoolValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) != ReturnFloatValue(constant);
                case "Char":
                    return Convert.ToChar(fieldValue) != ReturnCharValue(constant);
                case "Byte":
                    return Convert.ToByte(fieldValue) != ReturnByteValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) != ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
        private bool EvaluateValueAndConstantEqual(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "String":
                    return fieldValue.ToString() == constant;
                case "Int32":
                    return Convert.ToInt32(fieldValue) == ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) == ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) == ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) == ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) == ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) == ReturnDoubleValue(constant);
                case "Bool":
                    return Convert.ToBoolean(fieldValue) == ReturnBoolValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) == ReturnFloatValue(constant);
                case "Char":
                    return Convert.ToChar(fieldValue) == ReturnCharValue(constant);
                case "Byte":
                    return Convert.ToByte(fieldValue) == ReturnByteValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) == ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
        private bool EvaluateValueAndConstantGreaterThan(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue) > ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) > ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) > ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) > ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) > ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) > ReturnDoubleValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) > ReturnFloatValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) > ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
        private bool EvaluateValueAndConstantGreaterThanEqual(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue) >= ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) >= ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) >= ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) >= ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) >= ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) >= ReturnDoubleValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) >= ReturnFloatValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) >= ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
        private bool EvaluateValueAndConstantLessThan(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue) < ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) < ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) < ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) < ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) < ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) < ReturnDoubleValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) < ReturnFloatValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) < ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        private bool EvaluateValueAndConstantLessThanEqual(object fieldValue, string constant, string propertyType)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue) <= ReturnDecimalValue(constant);
                case "Int64":
                    return Convert.ToInt64(fieldValue) <= ReturnInt64Value(constant);
                case "UInt64":
                    return Convert.ToUInt64(fieldValue) <= ReturnUInt64Value(constant);
                case "UInt32":
                    return Convert.ToUInt32(fieldValue) <= ReturnUInt32Value(constant);
                case "Decimal":
                    return Convert.ToDecimal(fieldValue) <= ReturnDecimalValue(constant);
                case "Double":
                    return Convert.ToDouble(fieldValue) <= ReturnDoubleValue(constant);
                case "Single":
                    return Convert.ToSingle(fieldValue) <= ReturnFloatValue(constant);
                case "DateTime":
                    return Convert.ToDateTime(fieldValue) <= ReturnDateTimeValue(constant);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        private bool EvaluateCall(object fieldValue, string constant, string callFunction)
        {
            return callFunction switch
            {
                "StartWith" => fieldValue.ToString().StartsWith(constant.Substring(1, constant.Length - 2)),
                "EndWith" => fieldValue.ToString().EndsWith(constant.Substring(1, constant.Length - 2)),
                "Contains" => fieldValue.ToString().Contains(constant.Substring(1, constant.Length - 2))
            };
        }

        private (object value, string type) GetValue<T>(string field, T item)
        {
            Type t = item.GetType();
            PropertyInfo propertyInfo = t.GetProperty(field);
            return (propertyInfo.GetValue(item, null), propertyInfo.PropertyType.Name);
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
                    (object value, string propertyType) = GetValue<T>(cmd[0].Replace(".", ""), ch);
                    result = operandus switch
                    {
                        "==" => EvaluateValueAndConstantEqual(value, cmd[2], propertyType),
                        "!=" => EvaluateValueAndConstantNotEqual(value, cmd[2], propertyType),
                        ">=" => EvaluateValueAndConstantGreaterThanEqual(value, cmd[2], propertyType),
                        ">" => EvaluateValueAndConstantGreaterThan(value, cmd[2], propertyType),
                        "<=" => EvaluateValueAndConstantLessThanEqual(value, cmd[2], propertyType),
                        "<" => EvaluateValueAndConstantLessThan(value, cmd[2], propertyType),
                        "StartWith" => EvaluateCall(value, cmd[2], operandus),
                        "EndWith" => EvaluateCall(value, cmd[2], operandus),
                        "Contains" => EvaluateCall(value, cmd[2], operandus),
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


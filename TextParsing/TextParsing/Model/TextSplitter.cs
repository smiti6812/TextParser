using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.VisualBasic;

using TextParsing.Enums;
using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public class TextSplitter<T> : ITextSplitter where T : new()
    {
        private const string pattern = @"(EndWith)|(StartWith)|(Contains)|([A-Z]+[1-9]+)|(==)|(\()|(\))|(!=)|(>=)|(>)|(<=)|(<)";
        private const string splitPattern = @"([a-zA-Z]+_+[a-z-A-Z0-9]\s+)|([a-zA-Z]+\s+[a-z-A-Z0-9]+)|(EndWith)|(StartWith)|(Contains)|([A-Z]+[0-9]+)|(==)|(\()|(\))|(!=)|(>=)|(>)|(<=)|(<)";
        private readonly Dictionary<string, string> dateFormats = new Dictionary<string, string>() {
            {"en-UK", @"(0[1-9]|1[0-9]|2[0-9]|3[0-1])\.(0[1-9]|1[0-2])\.([1-9][0-9][0-9][0-9]) \d{2}:\d{2}:\d{2}"},
            {"en-US",@"(0[1-9]|1[0-2])\.(0[1-9]|1[0-9]|2[0-9]|3[0-1])\.([1-9][0-9][0-9][0-9]) \d{2}:\d{2}:\d{2}"},
            {"hu-HU", @"([1-9][0-9][0-9][0-9])\.(0[1-9]|1[0-2])\.(0[1-9]|1[0-9]|2[0-9]|3[0-1]) \d{2}:\d{2}:\d{2}"},
            {"de-DE", @"([1-9][0-9][0-9][0-9])\.(0[1-9]|1[0-2])\.(0[1-9]|1[0-9]|2[0-9]|3[0-1]) \d{2}:\d{2}:\d{2}"}
        };

        private readonly Dictionary<string, string> validDateFormats = new Dictionary<string, string>()
        {
            {"en-UK", "dd.MM.yyyy hh:mm:ss" },
            {"en-US", "MM.dd.yyyy hh:mm:ss" },
            {"hu-HU", "yyyy.MM.dd hh:mm:ss" },
            {"de-DE", "yyyy.MM.dd hh:mm:ss" }
        };

        private string datePattern = @"\d{2}\.|-\d{2}\.|-\d{4} \d{2}:\d{2}:\d{2}";

        private Stack<string> Brackets { get; set; }
        public Queue<Token> Tokens { get; set; }

        public CultureInfo CultureInfo { get; set; }
        public TextSplitter(string input) => Tokens = new Queue<Token>(Lex(input));

        public TextSplitter(string input, CultureInfo _cultureInfo = null)
        {
            Brackets = new Stack<string>();
            CultureInfo = _cultureInfo ?? CultureInfo.CurrentCulture;
            Tokens = new Queue<Token>(Lex(input));
            CheckBrackets();
        }

        private void CheckBrackets()
        {
            if (Brackets.Count > 0)
            {
                throw new InvalidDataException("Check if the number of opening and closing parenthesis are matching! There seems to be more opening parenthesis than closing one.");
            }
        }

        public virtual IList<Token> Lex(string input)
        {
            string[] operands = Regex.Split(input, pattern, RegexOptions.IgnoreCase);
            operands = operands.Where(c => c != "" && c != " " && c != ".").ToArray();

            List<Token> tokens = new List<Token>();
            bool variable = false;
            for (int i = 0; i < operands.Length; i++)
            {
                string propertyName = "";
                switch (operands[i].Trim())
                {
                    case "<=":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.LessThanEqual, "<="));
                        break;
                    case "<":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.LessThan, "<"));
                        break;
                    case ">=":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.GreaterThanEqual, ">="));
                        break;
                    case ">":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.GreaterThan, ">"));
                        break;
                    case "!=":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.NotEqual, "!="));
                        break;
                    case "==":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Equal, "=="));
                        break;
                    case "(":
                        if (tokens.Count > 0 && tokens[tokens.Count - 1].TokenType == TokenType.Call)
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.CallOpeningBracket, "("));
                        }
                        else
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.OpenParenthesis, "("));
                        }
                        break;
                    case ")":
                        if (tokens.Count > 3 && tokens[tokens.Count - 3].TokenType == TokenType.Call)
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.CallClosingBracket, ")"));
                        }
                        else
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.CloseParenthesis, ")"));
                        }
                        break;
                    case "And":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.And, "And"));
                        break;
                    case "Or":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Or, "Or"));
                        break;
                    case "StartWith":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Call, "StartWith"));
                        break;
                    case "EndWith":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Call, "EndWith"));
                        break;
                    case "Contains":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Call, "Contains"));
                        break;
                    default:
                        if (!variable && tokens.Count > 1 && tokens[tokens.Count - 2].TokenType != TokenType.Call)
                        {
                            propertyName = ValidateVariable(operands[i].Trim().Replace(".", ""));
                            tokens.Add(new Token(TokenType.Variable, propertyName, GetPropertyType(propertyName)));
                            variable = true;
                        }
                        else if (!variable)
                        {
                            propertyName = operands[i].Trim().Replace(".", "");
                            tokens.Add(new Token(TokenType.Variable, propertyName, GetPropertyType(propertyName)));
                            variable = true;
                        }
                        else
                        {
                            var constant = operands[i].Trim();
                            Type type = tokens[tokens.Count - 2].TokenType == TokenType.Call ? ConstantTypeValidation(tokens[tokens.Count - 3].Type, constant) : ConstantTypeValidation(tokens[tokens.Count - 2].Type, constant);
                            tokens.Add(new Token(TokenType.Constant, operands[i].Trim(), type));
                            variable = false;
                        }
                        break;
                }
            }

            return tokens;
        }
        protected virtual Type ConstantTypeValidation(Type propertyType, string constant)
        {
            Type type = null;
            if(constant == "x")
            {
                type = new string("x").GetType();
            }
            else if (propertyType.FullName == "System.DateTime")
            {
                var datePattern = dateFormats.FirstOrDefault(d => d.Key == CultureInfo.Name);
                if (datePattern.Value != null && Regex.Match(constant, datePattern.Value, RegexOptions.IgnoreCase).Success)
                {
                    type = propertyType;
                }
                else
                {
                    var validDateFormat = validDateFormats.FirstOrDefault(vdf => vdf.Key == CultureInfo.Name);
                    throw new InvalidDataException($"{constant} is not in a valid date format! Valid date format for {validDateFormat.Key} is: {validDateFormat.Value}!");
                }
            }
            else if (propertyType.FullName == "System.String")
            {
                type = Regex.Match(constant, @"^[a-zA-Z0-9\s""+_-]+$", RegexOptions.IgnoreCase).Success
                    ? propertyType
                    : throw new InvalidDataException($"{constant} contains character(s) which is/are not valid!");
            }
            else if (propertyType.FullName == "System.Int32")
            {
                type = int.TryParse(constant, out int result)
                    ? propertyType
                    : throw new InvalidDataException($"{constant} is not a valid nummerical value!");
            }
            else if (propertyType.FullName == "System.Int64")
            {
                type = long.TryParse(constant, out long result)
                    ? propertyType
                    : throw new InvalidDataException($"{constant} is not a valid nummerical value!");
            }
            else if (propertyType.FullName == "System.Double")
            {
                type = double.TryParse(constant, out double result)
                    ? propertyType
                    : throw new InvalidDataException($"{constant} is not a valid double value!");
            }
            else if (propertyType.FullName == "System.Single")
            {
                type = float.TryParse(constant, out float result)
                    ? propertyType
                    : throw new InvalidDataException($"{constant} is not a valid double value!");
            }
            else if (propertyType.FullName == "System.Decimal")
            {
                type = Decimal.TryParse(constant, out decimal result)
                    ? propertyType
                    : throw new InvalidDataException($"{constant} is not a valid double value!");
            }

            return type;
        }

        public Type GetPropertyType(string field)
        {
            T t = new T();
            Type type = t.GetType();
            PropertyInfo propertyInfo = type.GetProperty(field);
            return propertyInfo == null ? throw new InvalidDataException($"{field} property does not exist!") : propertyInfo.PropertyType;
        }

        private string ValidateVariable(string variable)
        {
            return !Regex.Match(variable, @"^[a-zA-Z0-9_-]+$", RegexOptions.IgnoreCase).Success
                ? throw new InvalidOperationException($"{variable} contains character('s) that is/are not valid. Only word, digital underscore or hyphen charaters are allowed")
                : variable;
        }
    }
}
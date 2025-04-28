using System.Globalization;
using System.Text.RegularExpressions;

namespace TextParsing.Model
{
    public class AritmeticTextSplitter<T> : TextSplitter<T> where T : struct
    {
        private readonly string aritmeticSplitPattern = @"([0-9]+)|(\+)|(\*)|(\/)|(-)|(\()|(\))|(==)";

        public AritmeticTextSplitter(string input, CultureInfo _cultureInfo) : base(input, _cultureInfo)
        {
            CultureInfo = _cultureInfo ?? CultureInfo.CurrentCulture;
            Tokens = new Queue<Token>(Lex(input));
        }

        public override IList<Token> Lex(string input)
        {
            string[] operands = Regex.Split(input, aritmeticSplitPattern, RegexOptions.IgnoreCase);
            operands = operands.Where(c => c is not "" and not " " and not ".").ToArray();

            IList<Token> tokens = [];
            for (int i = 0; i < operands.Length; i++)
            {
                switch (operands[i].Trim())
                {
                    case "(":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.OpenParenthesis, "("));
                        break;
                    case ")":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.CloseParenthesis, ")"));
                        break;
                    case "+":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Addition, "+"));
                        break;
                    case "-":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Substruction, "-"));
                        break;
                    case "*":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Multiplication, "*"));
                        break;
                    case "/":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Division, "/"));
                        break;
                    case "==":
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Equal, "=="));
                        break;
                    default:
                        string constant = operands[i].Trim();
                        T t = new();
                        Type type = ConstantTypeValidation(t.GetType(), constant);
                        tokens.Add(new Token(TextParsing.Enums.TokenType.Constant, constant, type));
                        break;
                }
            }

            return tokens;
        }
    }
}

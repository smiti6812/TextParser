using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TextParsing.Enums;
using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public class TextSplitter : ITextSplitter
    {
        private const string pattern = @"(EndWith)|(StartWith)|(Contains)|([A-Z]+[1-9]+)|(==)|(\()|(\))|(!=)|(>=)|(>)|(<=)|(<)";
        public Queue<Token> Tokens { get; set; }
        public TextSplitter(string input) => Tokens = new Queue<Token>(Lex(input));
        
        private IList<Token> Lex(string input)
        {
            string[] operands = Regex.Split(input, pattern, RegexOptions.IgnoreCase);
            operands = operands.Where(c => c != "" && c != " " && c != ".").ToArray();

            List<Token> tokens = new List<Token>();
            bool variable = false;
            for (int i = 0; i < operands.Length; i++)
            {
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
                        if (!variable && tokens.Count > 1 && tokens[tokens.Count - 2].TokenType != TextParsing.Enums.TokenType.Call)
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.Variable, operands[i].Trim()));
                            variable = true;
                        }
                        else if (!variable)
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.Variable, operands[i].Trim()));
                            variable = true;
                        }
                        else
                        {
                            tokens.Add(new Token(TextParsing.Enums.TokenType.Constant, operands[i].Trim()));
                            variable = false;
                        }
                        break;
                }
            }

            return tokens;
        }
    }
}
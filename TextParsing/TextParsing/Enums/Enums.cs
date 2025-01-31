using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Enums
{
    public enum OperationType
    {
        And,
        Or,
        Equal,
        NotEqual,
        Contains,
        EndWith,
        StartWith
    }

    public enum TokenType
    {
        Bool,
        String,
        Equal,
        And,
        Or,
        NotEqual,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        OpenParenthesis,
        CloseParenthesis,
        Variable,
        Constant,
        Lbracket,
        Rbracket,
        Expression,
        Call,
        CallOpeningBracket,
        CallClosingBracket


    }

}

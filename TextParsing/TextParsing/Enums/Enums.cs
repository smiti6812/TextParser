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
        Addition,
        Substruction,
        Division,
        Multiplication,
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
        CallClosingBracket,
        NoAction

    }

}

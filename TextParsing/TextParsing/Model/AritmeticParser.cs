using System.Text;

using TextParsing.Enums;
using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public class AritmeticParser<T> : Parser where T : struct
    {
        private T aritmeticResult;
        private bool closingBrackets = false;
        private bool openingBrackets = false;
        public AritmeticParser(ITextSplitter _textSplitter) : base(_textSplitter) => aritmeticResult = default;

        protected override void Parse()
        {
            ExpressionNode root = new();
            Root = ParseExpression1(root);
            Root = GoToRoot(Root);
        }

        private void ProcessingExpressionNode(ExpressionNode root, ExpressionNode node)
        {
            ExpressionNode newRoot = new();
            if (root.Left == null)
            {
                node.Parent = root;
                root.Left = node;
            }
            else
            {
                if (Tokens.Any() && Tokens.Peek().TokenType != TokenType.CloseParenthesis)
                {
                    if (Tokens.Peek().TokenType is TokenType.Addition or TokenType.Substruction)
                    {
                        newRoot.Token = Tokens.Peek();
                        newRoot.ValueType = Tokens.Peek().Type;
                        root.Right = node;
                        node.Parent = root;
                        root = GoToRoot(root);
                        root.Parent = newRoot;
                        newRoot.Left = root;
                        _ = ParseExpression(newRoot);
                    }
                    else if (Tokens.Peek().TokenType is TokenType.Multiplication or TokenType.Division)
                    {
                        newRoot.Token = Tokens.Peek();
                        newRoot.ValueType = Tokens.Peek().Type;
                        newRoot.Left = node;
                        node.Parent = newRoot;
                        root.Right = newRoot;
                        newRoot.Parent = root;
                        _ = ParseExpression(newRoot);
                    }
                }
                else
                {
                    node.Parent = root;
                    root.Right = node;
                }
            }
        }

        private ExpressionNode GoToRoot(ExpressionNode root)
        {
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }

        private ExpressionNode GoUpToCurrentRoot(ExpressionNode root)
        {
            while (root.Token.TokenType != TokenType.Multiplication || root.Token.TokenType != TokenType.Division || root.Parent != null)
            {
                if (root.Parent == null)
                {
                    return root;
                }

                root = root.Parent;
            }

            return root;
        }

        private ExpressionNode ParseExpression1(ExpressionNode root)
        {
            while (Tokens.Count > 0)
            {
                StringBuilder sb = new();
                Token token = Tokens.Dequeue();
                switch (token.TokenType)
                {
                    case TokenType.CloseParenthesis:
                        closingBrackets = true;
                        openingBrackets = false;
                        return root;
                    case TokenType.OpenParenthesis:
                        //"5+4*(2-4+2*3)";
                        openingBrackets = true;
                        ExpressionNode newNode = new()
                        {
                            OpenBracket = true
                        };
                        if (root.Left == null)
                        {
                            root.Left = newNode;
                            newNode.Parent = root;
                        }
                        else
                        {
                            root.Right = newNode;
                            newNode.Parent = root;
                        }
                        //closingBrackets = true;
                        _ = ParseExpression1(newNode);

                        break;
                    case TokenType.Equal:
                        ExpressionNode equalRoot = new()
                        {
                            Token = token,
                            Text = sb.ToString(),
                            Left = root
                        };
                        root.Parent = equalRoot;
                        _ = ParseExpression1(equalRoot);
                        break;
                    case TokenType.Constant:
                        _ = sb.Append(token.Text);
                        ExpressionNode node = ConstVariableTypeValueProvider.ReturnExpressionNode(token.Type.Name, sb.ToString(), CultureInfo);
                        node.Token = token;
                        node.ValueType = token.Type;
                        //5+4*(2-4)/2       
                        //"5+4*(2-4+2*3)/2";
                        if (root.Left == null)
                        {
                            node.Parent = root;
                            root.Left = node;
                        }
                        else
                        {
                            node.Parent = root;
                            root.Right = node;
                        }

                        break;
                    case TokenType.Addition:
                    case TokenType.Substruction:
                    case TokenType.Multiplication:
                    case TokenType.Division:
                        _ = sb.Append(token.Text);
                        //5+4*(2-4)/2  
                        //"5+4*(2-4+2-4*3)/2";
                        //"5+4*(2-4+2*3)";
                        if (root.Right != null)
                        {
                            ExpressionNode newRoot = new()
                            {
                                Text = sb.ToString(),
                                Token = token
                            };
                            if (root.Parent == null || !closingBrackets)
                            {
                                if ((token.TokenType == TokenType.Addition || token.TokenType == TokenType.Substruction) && root.Token.TokenType != TokenType.Equal)
                                {
                                    if (root.Token.TokenType is TokenType.Division or TokenType.Multiplication)
                                    {
                                        root = GoUpToCurrentRoot(root);
                                    }

                                    ExpressionNode? parent = root.Parent;

                                    newRoot.Parent = root.Parent;
                                    root.Parent = newRoot;
                                    newRoot.Left = root;
                                    if (parent != null)
                                    {
                                        parent.Right = newRoot;
                                    }
                                }
                                else
                                {
                                    ExpressionNode temp = root.Right;
                                    temp.Parent = newRoot;
                                    newRoot.Left = temp;
                                    root.Right = newRoot;
                                    newRoot.Parent = root;
                                    //root = GoToRoot(root);
                                }
                            }
                            else
                            {
                                /*
                                while (!root.OpenBracket)
                                {
                                    root = root.Parent;
                                }
                                */
                                ExpressionNode parent = root.Parent;
                                ExpressionNode temp = parent.Right;
                                temp.Parent = newRoot;
                                newRoot.Left = temp;
                                parent.Right = newRoot;
                                newRoot.Parent = parent;
                            }

                            closingBrackets = false;
                            _ = ParseExpression1(newRoot);
                            //root = root.Parent != null ? root.Parent : root;
                        }
                        else
                        {
                            root.Text = sb.ToString();
                            root.Token = token;
                        }

                        break;
                }
            }

            return root;
        }
        private ExpressionNode ParseExpression(ExpressionNode root, ExpressionNode? subRoot = null)
        {
            while (Tokens.Count > 0)
            {
                StringBuilder sb = new();
                Token token = Tokens.Dequeue();
                switch (token.TokenType)
                {
                    case TokenType.CloseParenthesis:
                        return root;
                    case TokenType.OpenParenthesis:
                        ExpressionNode newNode = new();
                        _ = ParseExpression(root, newNode);
                        ExpressionNode newRoot = new()
                        {
                            Token = Tokens.Peek(),
                            ValueType = Tokens.Peek().Type,
                            Left = newNode
                        };
                        newNode.Parent = newRoot;
                        root.Right = newRoot;
                        newRoot.Parent = root;
                        _ = ParseExpression(newRoot);
                        subRoot = null;
                        break;
                    case TokenType.Constant:
                        _ = sb.Append(token.Text);
                        ExpressionNode node = ConstVariableTypeValueProvider.ReturnExpressionNode(token.Type.Name, sb.ToString(), CultureInfo);
                        node.Token = token;
                        node.ValueType = token.Type;
                        //5+4*(2-4)/2   
                        if (subRoot != null)
                        {
                            ProcessingExpressionNode(subRoot, node);
                        }
                        else
                        {
                            ProcessingExpressionNode(root, node);
                        }
                        /*
                        if (root.Left == null)
                        {
                            node.Parent = root;
                            root.Left = node;
                        }
                        else
                        {                         
                            if (Tokens.Any())
                            {
                                var t = Tokens.Peek();
                                ProcessingExpressionNode(root, node);                              
                            }
                            else
                            {
                                node.Parent = root;
                                root.Right = node;
                            }
                        }
                        */
                        break;
                    case TokenType.Addition:
                    case TokenType.Substruction:
                    case TokenType.Multiplication:
                    case TokenType.Division:
                        _ = sb.Append(token.Text);
                        if (subRoot != null)
                        {
                            subRoot.Text = sb.ToString();
                            subRoot.Token = token;
                            subRoot.ValueType = token.Type;
                        }
                        else
                        {
                            root.Text = sb.ToString();
                            root.Token = token;
                            root.ValueType = token.Type;
                        }

                        break;
                }
            }

            return root;
        }

        private bool CheckEqualExists(ExpressionNode root) => root.Token.TokenType == TokenType.Equal;

        private ExpressionNode? GetRootWithoutXNode(ExpressionNode root)
        {
            if (root == null)
            {
                return null;
            }

            if (root.ValueType != null && root.ValueType.FullName == "System.String" && ((NodeString)root).ConstantValue == "x")
            {
                if (root.Parent.Token.TokenType is TokenType.Multiplication or TokenType.Division)
                {
                    //x*4+6-2+8 or 4*x+6-2+8
                    ExpressionNode? parent = root.Parent.Parent;
                    if (parent != null)
                    {
                        ExpressionNode? xNode = null;
                        NodeInt32 newRightLeft = parent.Token.TokenType is TokenType.Multiplication or TokenType.Division ? (NodeInt32)ConstVariableTypeValueProvider.ReturnExpressionNode("Int32", "1", CultureInfo) :
                            (NodeInt32)ConstVariableTypeValueProvider.ReturnExpressionNode("Int32", "0", CultureInfo);
                        if (parent.Left.Token.TokenType is TokenType.Multiplication or TokenType.Division)
                        {
                            xNode = parent.Left;
                        }
                        else if (parent.Right.Token.TokenType is TokenType.Multiplication or TokenType.Division)
                        {
                            xNode = parent.Right;
                        }

                        newRightLeft.Token = parent.Token.TokenType is TokenType.Multiplication or TokenType.Division ? new Token(TokenType.Constant, "1", new int().GetType()) :
                            new Token(TokenType.Constant, "0", new int().GetType());
                        newRightLeft.ValueType = newRightLeft.Token.Type;
                        xNode.Left = newRightLeft;
                        xNode.Left.Parent = xNode;
                        xNode.Right = newRightLeft;
                        xNode.Right.Parent = xNode;
                    }
                }
                else
                {
                    _ = root.Parent;
                }

                return root;
            }

            return GetRootWithoutXNode(root.Left) ?? GetRootWithoutXNode(root.Right);

        }
        private ExpressionNode? GetXNodeParent(ExpressionNode root)
        {
            if (root == null)
            {
                return null;
            }

            if (root.ValueType != null && root.ValueType.FullName == "System.String" && ((NodeString)root).ConstantValue == "x")
            {
                _ = root.Parent.Token.TokenType is TokenType.Multiplication or TokenType.Division
                    ? root.Parent.Parent
                    : root.Parent;

                return root;
            }

            return GetXNodeParent(root.Left) ?? GetXNodeParent(root.Right);
        }

        private ExpressionNode CreateNodeWithChildren(ExpressionNode temp)
        {
            ExpressionNode xNodeParent = new();
            ExpressionNode leftNode = ConstVariableTypeValueProvider.ReturnExpressionNode(temp.Left.Token.Type.Name, temp.Left.Token.Text, CultureInfo);
            ExpressionNode rightNode = ConstVariableTypeValueProvider.ReturnExpressionNode(temp.Right.Token.Type.Name, temp.Right.Token.Text, CultureInfo);
            leftNode.ValueType = temp.Left.ValueType;
            leftNode.Token = temp.Token;
            rightNode.ValueType = temp.Right.ValueType;
            rightNode.Token = temp.Token;

            xNodeParent.Token = temp.Token;
            xNodeParent.ValueType = temp.ValueType;
            xNodeParent.Left = leftNode;
            xNodeParent.Right = rightNode;
            xNodeParent.Parent = temp.Parent;
            return xNodeParent;
        }

        private ExpressionNode GetConstantNode(ExpressionNode root)
        {
            if (root == null)
            {
                return root;
            }

            if (root.ValueType != null && root.ValueType.FullName != "System.String")
            {
                return root;
            }

            return GetConstantNode(root.Left) ?? GetConstantNode(root.Right);
        }

        private List<ExpressionNode> GetConstantXNodeList(ExpressionNode root, List<ExpressionNode> xNodeList)
        {

            if (root == null)
            {
                return xNodeList;
            }

            if (root.ValueType != null && root.ValueType.FullName == "System.String")
            {
                xNodeList.Add(root);
            }

            GetConstantXNodeList(root.Left, xNodeList);
            GetConstantXNodeList(root.Right, xNodeList);

            return xNodeList;
        }

        private ExpressionNode CreateXNodeBranch(ExpressionNode root)
        {
            var xNodeList = GetConstantXNodeList(root, new List<ExpressionNode>());
            ExpressionNode newRoot = new ExpressionNode();
            ExpressionNode newXnode = null;
            foreach (var xNode in xNodeList)
            {
                if (newRoot.Left != null && newRoot.Right != null)
                {
                    var node = new ExpressionNode();
                    node.Left = newRoot;
                    newRoot.Parent = node;
                    newRoot = node;
                }

                if (xNode.Parent == null || xNode.Parent.Token.TokenType == TokenType.Addition)
                {
                    newRoot.Token = new Token(TokenType.Addition, "+");
                    newXnode = xNode;
                }
                else if (xNode.Parent.Token.TokenType == TokenType.Substruction)
                {
                    newRoot.Token = new Token(TokenType.Substruction, "-");
                    newXnode = xNode;
                }
                else
                {
                    if (xNode.Parent.Token.TokenType == TokenType.Multiplication || xNode.Parent.Token.TokenType == TokenType.Division)
                    {
                        if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Addition)
                        {
                            newRoot.Token = new Token(TokenType.Substruction, "-");
                            newXnode = xNode.Parent;
                        }
                        else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Substruction)
                        {
                            newRoot.Token = new Token(TokenType.Addition, "+");
                            newXnode = xNode.Parent;
                        }
                        else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Multiplication)
                        {
                            newRoot.Token = new Token(TokenType.Division, "*");
                            newXnode = xNode.Parent;
                        }
                        else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Division)
                        {
                            newRoot.Token = new Token(TokenType.Multiplication, "/");
                            newXnode = xNode.Parent;
                        }
                    }
                }

                newXnode.Parent = newRoot;
                if (newRoot.Left == null)
                {
                    newRoot.Left = newXnode;
                }
                else
                {
                    newRoot.Right = newXnode;
                }
            }

            return newRoot;
        }

        private ExpressionNode GetConstantXNode(ExpressionNode root)
        {
            if (root == null)
            {
                return root;
            }
            if (root.ValueType != null && root.ValueType.FullName == "System.String")
            {
                return root;
            }

            return GetConstantXNode(root.Left) ?? GetConstantXNode(root.Right);
        }
        private ExpressionNode GetNoConstantNode(ExpressionNode root) => root.Left.Token.TokenType != TokenType.Constant ? root.Left : root.Right;

        private (ExpressionNode node, bool xnNodeExists) CreateNewBranch(int value, ExpressionNode xNode, ExpressionNode root, bool left)
        {
            bool xNodeExists = false;
            if (GetXNodeParent(xNode) is ExpressionNode _xNode)
            {
                xNodeExists = true;
                ExpressionNode newNode = ConstVariableTypeValueProvider.ReturnExpressionNode(value.GetType().Name, value.ToString(), CultureInfo);
                ExpressionNode parent = null;
                if ((_xNode.Parent.Token.TokenType == TokenType.Multiplication || _xNode.Parent.Token.TokenType == TokenType.Multiplication) && _xNode.Parent.Parent != null)
                {
                    parent = _xNode.Parent.Parent;
                }
                else
                {
                    parent = _xNode.Parent;
                }
                parent.Parent = root;
                newNode.Parent = parent;
                newNode.Token = new Token(TokenType.Constant, value.ToString(), value.GetType());
                newNode.ValueType = value.GetType();
                if (parent.Left.ValueType.Name != "String")
                {
                    parent.Left = newNode;
                }
                else
                {
                    parent.Right = newNode;
                }

                if (left)
                {
                    root.Left = parent;
                }
                else
                {
                    root.Right = parent;
                }
            }
            else
            {
                ExpressionNode newNode = ConstVariableTypeValueProvider.ReturnExpressionNode(value.GetType().Name, value.ToString(), CultureInfo);
                newNode.Token = new Token(TokenType.Constant, value.ToString(), value.GetType());
                newNode.ValueType = value.GetType();
                newNode.Parent = root;
                if (left)
                {
                    root.Left = newNode;
                }
                else
                {
                    root.Right = newNode;
                }
            }

            return (root, xNodeExists);
        }

        private (TokenType tokenType, ExpressionNode node) ReturnTokenTypeForXNode(ExpressionNode xNode)
        {
            if (xNode.Parent == null || xNode.Parent.Token.TokenType == TokenType.Addition)
            {
                return (TokenType.Substruction, xNode);
            }
            else if (xNode.Parent.Token.TokenType == TokenType.Substruction)
            {
                return (TokenType.Addition, xNode);
            }
            else
            {
                if (xNode.Parent.Token.TokenType == TokenType.Multiplication || xNode.Parent.Token.TokenType == TokenType.Division)
                {
                    if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Addition)
                    {
                        return (TokenType.Substruction, xNode.Parent);
                    }
                    else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Substruction)
                    {
                        return (TokenType.Addition, xNode.Parent);
                    }
                    else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Multiplication)
                    {
                        return (TokenType.Division, xNode.Parent);
                    }
                    else if (xNode.Parent.Parent != null && xNode.Parent.Parent.Token.TokenType == TokenType.Division)
                    {
                        return (TokenType.Multiplication, xNode.Parent);
                    }
                    else
                    {
                        var constantNode = GetConstantNode(xNode.Parent);
                        return xNode.Parent.Parent == null && xNode.Parent.Token.TokenType == TokenType.Multiplication ? (TokenType.Division, constantNode) : (TokenType.Multiplication, constantNode);
                    }
                }
            }

            return (TokenType.NoAction, null);
        }

        private ExpressionNode SolvingEquation(ExpressionNode root, int leftNumberOfLevels, int rightNumberOfLevels)
        {
            int leftBranchCalc = EvaluateExpression<int>(root.Left);
            int rightBranchCalc = EvaluateExpression<int>(root.Right);
            ExpressionNode _newRoot = null;
            while (GetXNodeParent(root.Right) != null)
            {
                rightBranchCalc = EvaluateExpression<int>(root.Right);
                (_newRoot, bool _xNodeRightExists) = CreateNewBranch(rightBranchCalc, root.Right, root, false);
                ExpressionNode _xNodeRight = GetConstantXNode(_newRoot.Right);
                ExpressionNode constantNodeRight = GetConstantNode(_newRoot.Right);
                constantNodeRight.Parent = _newRoot;
                _newRoot.Right = constantNodeRight;
                (TokenType tokenType, ExpressionNode xNode) = ReturnTokenTypeForXNode(_xNodeRight);
                var newNode = new ExpressionNode();
                var temp = _newRoot.Left;
                newNode.Right = xNode;
                temp.Parent = newNode;
                newNode.Left = temp;
                newNode.Parent = _newRoot;
                xNode.Parent = newNode;
                _newRoot.Left = newNode;
                switch (tokenType)
                {
                    case TokenType.Addition:
                        newNode.Token = new Token(tokenType, "+", xNode.Token.Type);
                        break;
                    case TokenType.Substruction:
                        newNode.Token = new Token(tokenType, "-", xNode.Token.Type);
                        break;
                    case TokenType.Multiplication:
                        newNode.Token = new Token(tokenType, "*", xNode.Token.Type);
                        break;
                    case TokenType.Division:
                        newNode.Token = new Token(tokenType, "*", xNode.Token.Type);
                        break;
                }
            }
            var xNodeList = GetConstantXNodeList(root.Left, new List<ExpressionNode>());
            leftBranchCalc = Int32.MaxValue;
            while (((dynamic)GetConstantNode(_newRoot.Left)).ConstantValue != leftBranchCalc)
            {
                leftBranchCalc = EvaluateExpression<int>(_newRoot.Left);
                var leftBranch = CreateXNodeBranch(_newRoot.Left);
                (_newRoot, bool _xNodeLeftExists) = CreateNewBranch(leftBranchCalc, root.Left, root, true);
                ExpressionNode constantNodeLeft = GetConstantNode(_newRoot.Left);
                (TokenType tokenType, ExpressionNode constantNode) = ReturnTokenTypeForXNode(constantNodeLeft);
                var newNode = new ExpressionNode();
                newNode.Left = _newRoot.Right;
                newNode.Right = constantNode;
                newNode.Parent = _newRoot;
                _newRoot.Right = newNode;
                switch (tokenType)
                {
                    case TokenType.Addition:
                        newNode.Token = new Token(tokenType, "+", constantNode.Token.Type);
                        break;
                    case TokenType.Substruction:
                        newNode.Token = new Token(tokenType, "-", constantNode.Token.Type);
                        break;
                    case TokenType.Multiplication:
                        newNode.Token = new Token(tokenType, "*", constantNode.Token.Type);
                        break;
                    case TokenType.Division:
                        newNode.Token = new Token(tokenType, "*", constantNode.Token.Type);
                        break;
                }
                rightBranchCalc = EvaluateExpression<int>(_newRoot.Right);
                (_newRoot, bool _xNodeRightExists) = CreateNewBranch(rightBranchCalc, _newRoot.Right, _newRoot, false);
            }


            (ExpressionNode newRoot, bool xNodeLeftExists) = CreateNewBranch(leftBranchCalc, root.Left, root, true);
            (newRoot, bool xNodeRightExists) = CreateNewBranch(rightBranchCalc, root.Right, root, false);
            dynamic xNodeRight = null;
            if (xNodeRightExists)
            {
                ExpressionNode constantNodeRight = GetConstantNode(newRoot.Right);
                xNodeRight = GetConstantXNode(newRoot.Right);
                xNodeRight.Parent = newRoot;
                newRoot.Right = xNodeRight;
                if (constantNodeRight.Parent.Token.TokenType == TokenType.Addition)
                {
                    ExpressionNode leftConstant = GetConstantNode(newRoot.Left);
                    ((NodeInt32)leftConstant).ConstantValue -= rightBranchCalc;
                    leftConstant.Token.Text = ((NodeInt32)leftConstant).ConstantValue.ToString();
                }
                else if (constantNodeRight.Parent.Token.TokenType == TokenType.Substruction)
                {
                    ExpressionNode leftConstant = GetConstantNode(newRoot.Left);
                    ((NodeInt32)leftConstant).ConstantValue += rightBranchCalc;
                    leftConstant.Token.Text = ((NodeInt32)leftConstant).ConstantValue.ToString();
                }
            }
            if (xNodeLeftExists)
            {
                ExpressionNode xNodeLeft = GetConstantXNode(newRoot.Left).Parent;
                ExpressionNode constantNodeLeft = GetConstantNode(newRoot.Left);
                constantNodeLeft.Parent = newRoot;
                newRoot.Left = constantNodeLeft;
                if (xNodeRightExists)
                {
                    if (xNodeLeft.Token.TokenType == TokenType.Addition)
                    {

                    }
                    else if (xNodeLeft.Token.TokenType == TokenType.Substruction)
                    {

                    }
                    else if (xNodeLeft.Token.TokenType == TokenType.Multiplication)
                    {
                        if (xNodeLeft.Parent != null && xNodeLeft.Parent.Token.TokenType == TokenType.Addition)
                        {
                            if (xNodeRight.Parent.Token.TokenType == TokenType.Equal)
                            {
                                dynamic constant = GetConstantNode(xNodeLeft);
                                dynamic xValue = 1 - constant.ConstantValue;
                                xNodeRight.ConstantValue = xValue.ToString() + xNodeRight.ConstantValue;
                                xNodeRight.Token.Text = xNodeRight.ConstantValue;
                                ;
                            }
                        }
                        else if (xNodeLeft.Parent != null && xNodeLeft.Parent.Token.TokenType == TokenType.Substruction)
                        {
                            if (xNodeRight.Parent.Token.TokenType == TokenType.Equal)
                            {
                                dynamic constant = GetConstantNode(xNodeLeft);
                                dynamic xValue = 1 + constant.ConstantValue;
                                xNodeRight.ConstantValue = xValue.ToString() + xNodeRight.ConstantValue;
                                xNodeRight.Token.Text = xNodeRight.ConstantValue;
                                ;
                            }

                        }
                    }
                    else if (xNodeLeft.Token.TokenType == TokenType.Division)
                    {

                    }
                }
                xNodeLeft.Parent = newRoot;
                newRoot.Left = xNodeLeft;
            }

            return SolvingEquation(root, leftNumberOfLevels, rightNumberOfLevels);
        }

        private int GetNumberOfLevels(ExpressionNode root, bool left, int level)
        {

            if (root == null)
            {
                return level;
            }

            if (left)
            {
                if (root.Left != null)
                {
                    level++;
                }

                return GetNumberOfLevels(root.Left, left, level);
            }
            else
            {
                if (root.Right != null)
                {
                    level++;
                }

                return GetNumberOfLevels(root.Right, left, level);
            }
        }

        public (ExpressionNode root, T result) Evaluate()
        {
            int leftLevels = GetNumberOfLevels(Root, true, 0);
            int rightLevels = GetNumberOfLevels(Root, false, 0);

            _ = SolvingEquation(Root, leftLevels, rightLevels);

            _ = GetXNodeParent(Root.Left);

            _ = GetXNodeParent(Root.Right);
            if (CheckEqualExists(Root))
            {
                ExpressionNode temp = GetXNodeParent(Root.Left);
                if (temp.Parent != null)
                {
                    temp = temp.Parent;
                    ExpressionNode xNodeParent = CreateNodeWithChildren(temp);

                    ExpressionNode left = Root.Left;
                    ExpressionNode right = Root.Right;
                    left = GetRootWithoutXNode(left) ?? left;
                    left = GoToRoot(left);

                    _ = EvaluateExpression<T>(left.Left);
                    right = GetRootWithoutXNode(right) ?? right;
                    right = GoToRoot(right);

                    _ = EvaluateExpression<T>(right);
                    if (xNodeParent != null)
                    {
                        if (xNodeParent.Left.ValueType.FullName == "System.String" && ((NodeString)xNodeParent.Left).ConstantValue == "x")
                        {
                            xNodeParent.Left = null;
                        }
                        else if (xNodeParent.Right.ValueType.FullName == "System.String" && ((NodeString)xNodeParent.Right).ConstantValue == "x")
                        {
                            xNodeParent.Right = null;
                        }

                        ExpressionNode root = xNodeParent.Parent;
                        while (root.Parent != null)
                        {
                            root = root.Parent;
                        }

                        ExpressionNode leftTree = root.Left;
                        leftTree.Parent = null;
                        ExpressionNode rightTree = root.Right;
                        rightTree.Parent = null;

                        _ = EvaluateExpression<T>(leftTree);
                    }
                }

                return (Root, aritmeticResult);
            }
            else
            {
                return (Root, aritmeticResult);
            }
        }

        public T Evaluate<T>() => EvaluateExpression<T>(Root);

        private dynamic EvaluateExpression<T>(ExpressionNode root)
        {
            //"5+4*(2-4+2-4*3)/2";
            if (root == null)
            {
                return aritmeticResult;
            }

            T resultLeft = EvaluateExpression<T?>(root.Left);
            T resultRight = EvaluateExpression<T?>(root.Right);

            aritmeticResult = root.Token.TokenType switch
            {
                TokenType.Constant => root.Token.Text == "x" ? 0 : ConstVariableTypeValueProvider.GetExpressionNodeDescendant(root).ConstantValue,
                TokenType.Addition => ConstVariableTypeValueProvider.GetValue(resultLeft) + ConstVariableTypeValueProvider.GetValue(resultRight),
                TokenType.Substruction => ConstVariableTypeValueProvider.GetValue(resultLeft) - ConstVariableTypeValueProvider.GetValue(resultRight),
                TokenType.Multiplication => ConstVariableTypeValueProvider.GetValue(resultLeft) * ConstVariableTypeValueProvider.GetValue(resultRight),
                TokenType.Division => root.Right.Token.Text == "x" ? 0 : ConstVariableTypeValueProvider.GetValue(resultLeft) / ConstVariableTypeValueProvider.GetValue(resultRight),
                TokenType.Equal => aritmeticResult,
                _ => throw new InvalidOperationException($"Invalid operation")
            };

            return aritmeticResult;
        }
    }
}

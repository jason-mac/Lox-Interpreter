namespace LoxInterpreter
{
    public class Interpreter : Expr.IVisitor<object>
    {
        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public void interpret(Expr expression)
        {
            try
            {
                object value = evaluate(expression);
                Console.WriteLine(stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);

            }

        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            object right = evaluate(expr.right);

            switch (expr.oper.type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expr.oper, right);
                    return -(double)right;
            }
            return null;
        }

        private bool isTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        private object evaluate(Expr expr)
        {
            return expr.accept(this);

        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.oper.type)
            {
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                case TokenType.GREATER:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.oper, "Operands must be two numbers or two strings")
                case TokenType.SLASH:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left * (double)right;

            }

            return null;
        }

        private void checkNumberOperands(Token oper, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operands must be numbers");

        }

        private void checkNumberOperand(Token oper, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand must be a number");

        }

        private bool isEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        private string stringify(object obj)
        {
            if (obj == null) return "nil";
            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();

        }
    }

}

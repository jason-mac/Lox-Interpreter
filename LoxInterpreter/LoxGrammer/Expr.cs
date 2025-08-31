namespace LoxInterpreter
{
    public abstract class Expr
    {
        public interface IVisitor<R>
        {
            R visitAssignExpr(Assign expr);
            R visitBinaryExpr(Binary expr);
            R visitCallExpr(Call expr);
            R visitGetExpr(Get expr);
            R visitGroupingExpr(Grouping expr);
            R visitLiteralExpr(Literal expr);
            R visitLogicalExpr(Logical expr);
            R visitSetExpr(Set expr);
            R visitSuperExpr(Super expr);
            R visitThisExpr(This expr);
            R visitUnaryExpr(Unary expr);
            R visitVariableExpr(Variable expr);

        }

        public abstract R accept<R>(IVisitor<R> visitor);

        public class Assign : Expr
        {
            public readonly Token name;
            public readonly Expr value;

            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitAssignExpr(this);
            }
        }

        public class Binary : Expr
        {
            public readonly Expr left;
            public readonly Token oper;
            public readonly Expr right;

            public Binary(Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
        }

        public class Call : Expr
        {
            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguments;

            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitCallExpr(this);
            }
        }

        public class Get : Expr
        {
            public readonly Expr obj;
            public readonly Token name;

            public Get(Expr obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitGetExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public readonly Expr expression;

            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public readonly Object value;

            public Literal(Object value)
            {
                this.value = value;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
        }

        public class Logical : Expr
        {
            public readonly Expr left;
            public readonly Token oper;
            public readonly Expr right;

            public Logical(Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitLogicalExpr(this);
            }
        }

        public class Set : Expr
        {
            public readonly Expr obj;
            public readonly Token name;
            public readonly Expr value;

            public Set(Expr obj, Token name, Expr value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitSetExpr(this);
            }
        }

        public class Super : Expr
        {
            public readonly Token keyword;
            public readonly Token method;

            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitSuperExpr(this);
            }
        }

        public class This : Expr
        {
            public readonly Token keyword;

            public This(Token keyword)
            {
                this.keyword = keyword;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitThisExpr(this);
            }
        }

        public class Unary : Expr
        {
            public readonly Token oper;
            public readonly Expr right;

            public Unary(Token oper, Expr right)
            {
                this.oper = oper;
                this.right = right;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
        }

        public class Variable : Expr
        {
            public readonly Token name;

            public Variable(Token name)
            {
                this.name = name;
            }

            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitVariableExpr(this);
            }
        }

    }
}

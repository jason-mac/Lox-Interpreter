namespace LoxInterpreter
{
		public abstract class Expr
		{
				public interface IVisitor<R>
				{
						R visitBinaryExpr(Binary expr);
						R visitGroupingExpr(Grouping expr);
						R visitLiteralExpr(Literal expr);
						R visitUnaryExpr(Unary expr);

				}

				public abstract R Accept<R>(IVisitor<R> visitor);

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

						public override R Accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitBinaryExpr(this);
						}
				}

				public class Grouping : Expr
				{
						public readonly Expr expression;

						public Grouping(Expr expression)
						{
								this.expression = expression;
						}

						public override R Accept<R>(IVisitor<R> visitor)
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

						public override R Accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitLiteralExpr(this);
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

						public override R Accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitUnaryExpr(this);
						}
				}

		}
}

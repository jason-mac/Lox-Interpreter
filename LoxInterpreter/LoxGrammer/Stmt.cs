namespace LoxInterpreter
{
		public abstract class Stmt
		{
				public interface IVisitor<R>
				{
						R visitBlockStmt(Block stmt);
						R visitClassStmt(Class stmt);
						R visitExpressionStmt(Expression stmt);
						R visitFunctionStmt(Function stmt);
						R visitIfStmt(If stmt);
						R visitPrintStmt(Print stmt);
						R visitReturnStmt(Return stmt);
						R visitVarStmt(Var stmt);
						R visitWhileStmt(While stmt);

				}

				public abstract R accept<R>(IVisitor<R> visitor);

				public class Block : Stmt
				{
						public readonly List<Stmt> statements;

						public Block(List<Stmt> statements)
						{
								this.statements = statements;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitBlockStmt(this);
						}
				}

				public class Class : Stmt
				{
						public readonly Token name;
						public readonly Expr.Variable superclass;
						public readonly List<Stmt.Function> methods;

						public Class(Token name, Expr.Variable superclass, List<Stmt.Function> methods)
						{
								this.name = name;
								this.superclass = superclass;
								this.methods = methods;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitClassStmt(this);
						}
				}

				public class Expression : Stmt
				{
						public readonly Expr expression;

						public Expression(Expr expression)
						{
								this.expression = expression;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitExpressionStmt(this);
						}
				}

				public class Function : Stmt
				{
						public readonly Token name;
						public readonly List<Token> parameters;
						public readonly List<Stmt> body;

						public Function(Token name, List<Token> parameters, List<Stmt> body)
						{
								this.name = name;
								this.parameters = parameters;
								this.body = body;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitFunctionStmt(this);
						}
				}

				public class If : Stmt
				{
						public readonly Expr condition;
						public readonly Stmt thenBranch;
						public readonly Stmt elseBranch;

						public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
						{
								this.condition = condition;
								this.thenBranch = thenBranch;
								this.elseBranch = elseBranch;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitIfStmt(this);
						}
				}

				public class Print : Stmt
				{
						public readonly Expr expression;

						public Print(Expr expression)
						{
								this.expression = expression;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitPrintStmt(this);
						}
				}

				public class Return : Stmt
				{
						public readonly Token keyword;
						public readonly Expr value;

						public Return(Token keyword, Expr value)
						{
								this.keyword = keyword;
								this.value = value;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitReturnStmt(this);
						}
				}

				public class Var : Stmt
				{
						public readonly Token name;
						public readonly Expr initializer;

						public Var(Token name, Expr initializer)
						{
								this.name = name;
								this.initializer = initializer;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitVarStmt(this);
						}
				}

				public class While : Stmt
				{
						public readonly Expr condition;
						public readonly Stmt body;

						public While(Expr condition, Stmt body)
						{
								this.condition = condition;
								this.body = body;
						}

						public override R accept<R>(IVisitor<R> visitor)
						{
								return visitor.visitWhileStmt(this);
						}
				}

		}
}

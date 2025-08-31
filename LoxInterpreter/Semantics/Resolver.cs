namespace LoxInterpreter
{
    public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }


        private enum FunctionType
        {
            NONE,
            FUNCTION,
            METHOD,
            INITIALIZER
        }

        public readonly Interpreter interpreter;

        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;

        private readonly List<Dictionary<string, bool>> scopes = new();

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }

        public object visitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;
            Declare(stmt.name);
            Define(stmt.name);
            if (stmt.superclass != null && stmt.name.lexeme.Equals(stmt.superclass.name.lexeme))
            {
                Lox.error(stmt.superclass.name, "A class can't inherit from itself");
            }

            if (stmt.superclass != null)
            {
                currentClass = ClassType.SUBCLASS;
                Resolve(stmt.superclass);
            }

            if (stmt.superclass != null)
            {
                BeginScope();
                scopes[scopes.Count - 1]["super"] = true;
            }

            BeginScope();
            scopes[scopes.Count - 1]["this"] = true;

            foreach (Stmt.Function method in stmt.methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (method.name.lexeme.Equals("init"))
                {
                    declaration = FunctionType.INITIALIZER;
                }
                ResolveFunction(method, declaration);
            }


            EndScope();

            if (stmt.superclass != null) EndScope();

            currentClass = enclosingClass;
            return null;
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.accept(this);
        }

        private void BeginScope()
        {
            scopes.Add(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            if (scopes.Count != 0)
            {
                scopes.RemoveAt(scopes.Count - 1);
            }
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null)
            {
                Resolve(stmt.initializer);
            }
            Define(stmt.name);
            return null;
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;
            Dictionary<string, bool> scope = scopes[scopes.Count - 1];
            if (scope.ContainsKey((name.lexeme)))
            {
                Lox.error(name, "Already a variable with this name in scope");
            }
            scope[name.lexeme] = false;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes[scopes.Count - 1][name.lexeme] = true;
        }

        public object visitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count > 0 && scopes[scopes.Count - 1].TryGetValue(expr.name.lexeme, out bool defined) && defined == false)

            {
                Lox.error(expr.name, "Can't read local variable in its own initializer.");
            }
            ResolveLocal(expr, expr.name);
            return null;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes[i].ContainsKey(name.lexeme))
                {
                    interpreter.resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }

        public object visitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);
            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();
            foreach (Token parameter in function.parameters)
            {
                Declare(parameter);
                Define(parameter);
            }
            Resolve(function.body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object visitReturnStmt(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE) Lox.error(stmt.keyword, "Can't return from top-level code");
            if (stmt.value != null)
            {
                if (currentFunction == FunctionType.INITIALIZER) Lox.error(stmt.keyword, "Can't return value from an initializer.");
                Resolve(stmt.value);
            }
            return null;
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object visitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);
            foreach (Expr argument in expr.arguments)
            {
                Resolve(argument);
            }
            return null;
        }


        public object visitGetExpr(Expr.Get expr)
        {
            Resolve(expr.obj);
            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object visitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object visitSetExpr(Expr.Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.obj);
            return null;
        }

        public object visitSuperExpr(Expr.Super expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.error(expr.keyword, "Can't use 'super' outside of a class.");
            }
            else if (currentClass != ClassType.SUBCLASS)
            {
                Lox.error(expr.keyword, "Can't use 'super' in a class with no superclass.");
            }
            ResolveLocal(expr, expr.keyword);
            return null;
        }

        public object visitThisExpr(Expr.This expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.error(expr.keyword, "Can't use 'this' outside of a class.");
            }
            ResolveLocal(expr, expr.keyword);
            return null;
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.right);
            return null;
        }
    }
}

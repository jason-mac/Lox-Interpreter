namespace LoxInterpreter
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {

        public readonly Environment globals = new Environment();
        private Environment? env;
        private readonly Dictionary<Expr, int> locals = new();

        public Interpreter()
        {
            globals.define("clock", new NativeFunction(0, (interpreter, arguments) =>
                                    (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0));
            this.env = globals;
        }


        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public object visitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, env, false);
            env.define(stmt.name.lexeme, function);
            return null;
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            if (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                execute(stmt.elseBranch);
            }
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            object value = evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        public object visitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value != null) value = evaluate(stmt.value);
            throw new Return(value);
        }

        public object visitLogicalExpr(Expr.Logical expr)
        {
            object left = evaluate(expr.left);
            if (expr.oper.type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else
            {
                if (!isTruthy(left)) return left;
            }
            return evaluate(expr.right);
        }

        public object visitSetExpr(Expr.Set expr)
        {
            object obj = evaluate(expr.obj);
            if (obj is not LoxInstance)
            {
                throw new RuntimeError(expr.name, "Only instances have fields.");
            }

            object value = evaluate(expr.value);
            ((LoxInstance)obj).set(expr.name, value);
            return value;
        }

        public object visitSuperExpr(Expr.Super expr)
        {
            int distance = locals[expr];
            LoxClass superclass = (LoxClass)env.getAt(distance, "super");
            LoxInstance obj = (LoxInstance)env.getAt(distance - 1, "this");
            LoxFunction method = superclass.findMethod(expr.method.lexeme);
            if (method == null)
            {
                throw new RuntimeError(expr.method, "Undefined property '" + expr.method.lexeme + "'.");
            }
            return method.bind(obj);
        }

        public object visitThisExpr(Expr.This expr)
        {
            return lookUpVariable(expr.keyword, expr);
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }
            env.define(stmt.name.lexeme, value);
            return null;
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            while (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.body);
            }
            return null;
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            object value = evaluate(expr.value);
            int dist = locals.ContainsKey((expr)) ? locals[expr] : -1;
            if (dist != -1)
            {
                env.assignAt(dist, expr.name, value);
            }
            else
            {
                globals.assign(expr.name, value);
            }
            return value;
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public void interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);

            }
        }

        private void execute(Stmt stmt)
        {
            stmt.accept(this);
        }

        public void resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            executeBlock(stmt.statements, new Environment(env));
            return null;
        }
        public object visitClassStmt(Stmt.Class stmt)
        {

            object superclass = null;
            if (stmt.superclass != null)
            {
                superclass = evaluate(stmt.superclass);
                if (superclass is not LoxClass)
                {
                    throw new RuntimeError(stmt.superclass.name, "Superclass must be a class");
                }
            }
            env.define(stmt.name.lexeme, null);

            if (stmt.superclass != null)
            {
                env = new Environment(env);
                env.define("super", superclass);
            }

            Dictionary<string, LoxFunction> methods = new();
            foreach (Stmt.Function method in stmt.methods)
            {
                LoxFunction function = new LoxFunction(method, env, method.name.lexeme.Equals("this"));
                methods[method.name.lexeme] = function;
            }

            LoxClass loxClass = new LoxClass(stmt.name.lexeme, (LoxClass)superclass, methods);

            if (superclass != null) env = env.enclosing;

            env.assign(stmt.name, loxClass);
            return null;
        }

        public void executeBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.env;
            try
            {
                env = environment;

                foreach (Stmt statement in statements)
                {
                    execute(statement);
                }
            }
            finally
            {
                env = previous;
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

        public object visitVariableExpr(Expr.Variable expr)
        {
            return lookUpVariable(expr.name, expr);
        }

        private object lookUpVariable(Token name, Expr expr)
        {
            int distance = locals.ContainsKey((expr)) ? locals[expr] : -1;
            if (distance != -1)
            {
                return env.getAt(distance, name.lexeme);
            }
            else
            {
                return globals.get(name);
            }
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
                    checkNumberOperand(expr.oper, right);
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    checkNumberOperand(expr.oper, right);
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
                    if (left is double && right is double) return (double)left + (double)right;
                    if (left is string && right is string) return (string)left + (string)right;
                    throw new RuntimeError(expr.oper, "Operands must be two numbers or two strings");
                case TokenType.SLASH:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expr.oper, left, right);
                    return (double)left * (double)right;
            }

            return null;
        }

        public object visitCallExpr(Expr.Call expr)
        {
            object callee = evaluate(expr.callee);

            List<object> arguments = new List<object>();
            foreach (Expr argument in expr.arguments)
            {
                arguments.Add(evaluate((argument)));
            }

            if (callee is not ILoxCallable)
            {
                Console.WriteLine(callee.ToString());
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            ILoxCallable function = (ILoxCallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.paren, "Expected " + function.Arity() + " arguments but got " + arguments.Count + ".");
            }
            return function.Call(this, arguments);
        }

        public object visitGetExpr(Expr.Get expr)
        {
            object obj = evaluate(expr.obj);
            if (obj is LoxInstance)
            {
                return ((LoxInstance)obj).get(expr.name);
            }

            throw new RuntimeError(expr.name, "Only instances have properties.");
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
                    text = text[0..(text.Length - 2)];
                }
                return text;
            }

            return obj.ToString();
        }
    }
}

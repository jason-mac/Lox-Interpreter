namespace LoxInterpreter
{
    public class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;

        readonly Environment closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            this.closure = closure;
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.parameters.Count;
        }

        public override String ToString()
        {
            return "<fn " + declaration.name.lexeme + ">";

        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(closure);
            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                environment.define(declaration.parameters[i].lexeme, arguments[i]);
            }
            try
            {
                interpreter.executeBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.value;
            }
            return null;
        }
    }

}

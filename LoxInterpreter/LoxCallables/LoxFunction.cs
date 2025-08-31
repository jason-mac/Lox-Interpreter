namespace LoxInterpreter
{
    public class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly bool isInitializer;

        readonly Environment closure;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            this.closure = closure;
            this.declaration = declaration;
            this.isInitializer = isInitializer;
        }

        public LoxFunction bind(LoxInstance instance)
        {
            Environment environment = new Environment(closure);
            environment.define("this", instance);
            return new LoxFunction(declaration, environment, isInitializer);

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
            for (int i = 0; i < declaration.parameters.Count; i++) environment.define(declaration.parameters[i].lexeme, arguments[i]);
            try
            {
                interpreter.executeBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                return isInitializer ? closure.getAt(0, "this") : returnValue.value;
            }
            return isInitializer ? closure.getAt(0, "this") : null;
        }
    }

}

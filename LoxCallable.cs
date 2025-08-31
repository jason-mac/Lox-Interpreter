namespace LoxInterpreter
{
    public interface ILoxCallable
    {
        public int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }

    public class NativeFunction : ILoxCallable
    {
        private readonly int arity;
        private readonly Func<Interpreter, List<object>, object> func;

        public NativeFunction(int arity, Func<Interpreter, List<object>, object> func)
        {
            this.arity = arity;
            this.func = func;
        }

        public int Arity() => arity;
        public object Call(Interpreter interpreter, List<object> arguments) => func(interpreter, arguments);
        public override string ToString() => "<native fn>";
    }

}

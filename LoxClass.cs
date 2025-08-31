namespace LoxInterpreter
{
    public class LoxClass : ILoxCallable
    {
        readonly public string name;

        public LoxClass(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }
    }

}

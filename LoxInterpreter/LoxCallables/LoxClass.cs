namespace LoxInterpreter
{
    public class LoxClass : ILoxCallable
    {
        readonly public string name;
        readonly public LoxClass superclass;
        readonly private Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            this.methods = methods;
            this.name = name;
            this.superclass = superclass;
        }

        public LoxFunction findMethod(string name)
        {
            if (methods.ContainsKey(name)) return methods[name];
            if (superclass != null) return superclass.findMethod(name);
            return null;
        }

        public override string ToString()
        {
            return name;
        }

        public int Arity()
        {
            LoxFunction initializer = findMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction initializer = findMethod("init");
            if (initializer != null)
            {
                initializer.bind(instance).Call(interpreter, arguments);
            }
            return instance;
        }
    }

}

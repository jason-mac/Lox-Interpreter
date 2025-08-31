namespace LoxInterpreter
{
    public class LoxInstance
    {

        private readonly Dictionary<string, object> fields = new();
        private LoxClass klass;

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        public override string ToString()
        {
            return klass.name + " instance.";
        }

        public object get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
            {
                return fields[name.lexeme];
            }

            LoxFunction method = klass.findMethod(name.lexeme);
            if (method != null) return method.bind(this);

            throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'.");
        }

        public void set(Token name, object value)
        {
            fields[name.lexeme] = value;
        }
    }
}

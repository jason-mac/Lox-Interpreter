namespace LoxInterpreter
{
    public class RuntimeError : System.Exception
    {
        public Token Token { get; }

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}

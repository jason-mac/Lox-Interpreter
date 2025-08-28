namespace LoxInterpreter
{
    public class Token
    {
        readonly public TokenType type;
        readonly public String lexeme;
        readonly public Object? literal;
        readonly public int line;

        public Token(TokenType type, String lexeme, Object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public String toString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }

}

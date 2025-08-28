namespace LoxInterpreter
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Expr expression()
        {
            return equality();
        }

        private Expr equality()
        {
            Expr expr = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQAUL_EQUAL))
            {
                Token oper = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        private bool match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        private bool check(TokenType type)
        {
            if (isAtEnd()) return false;
            return peek().type == type;
        }

        private bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens.ElementAt(current);
        }

        private Token previous()
        {
            return tokens.ElementAt(current - 1);
        }
    }
}

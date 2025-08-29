namespace LoxInterpreter
{
    public class Parser
    {
        private class ParseError : System.Exception { }
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        bool match(List<TokenType> types)
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

        public Expr parse()
        {
            try
            {
                return expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Token consume(TokenType type, String message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }

        private ParseError error(Token token, String message)
        {
            Lox.error(token, message);
            return new ParseError();
        }

        private void synchronize()
        {
            advance();

            while (!done())
            {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
            }
            advance();
        }

        bool check(TokenType type)
        {
            if (done()) return false;
            return peek().type == type;

        }

        Token previous()
        {
            return tokens.ElementAt(current - 1);
        }

        bool done()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens.ElementAt(current);
        }

        private Token advance()
        {
            if (!done()) current++;
            return previous();

        }

        private Expr expression()
        {
            return equality();
        }

        private Expr equality()
        {
            Expr expr = comparison();

            List<TokenType> types = new List<TokenType>
            {
              TokenType.BANG_EQUAL,
              TokenType.EQUAL_EQUAL,
            };

            while (match(types))
            {
                Token oper = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr comparison()
        {
            Expr expr = term();

            List<TokenType> types = new List<TokenType>
            {
              TokenType.GREATER,
              TokenType.GREATER_EQUAL,
              TokenType.LESS,
              TokenType.LESS_EQUAL
            };

            while (match(types))
            {
                Token oper = previous();
                Expr right = term();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }


        private Expr term()
        {
            Expr expr = factor();

            List<TokenType> types = new List<TokenType>
            {
              TokenType.MINUS,
              TokenType.PLUS
            };

            while (match(types))
            {
                Token oper = previous();
                Expr right = factor();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        private Expr factor()
        {
            Expr expr = unary();

            List<TokenType> types = new List<TokenType>
            {
              TokenType.SLASH,
              TokenType.STAR,
            };

            while (match(types))
            {
                Token oper = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        private Expr unary()
        {

            List<TokenType> types = new List<TokenType>
            {
              TokenType.BANG,
              TokenType.MINUS,
            };

            if (match(types))
            {
                Token oper = previous();
                Expr right = unary();
                return new Expr.Unary(oper, right);
            }
            return primary();
        }

        private Expr primary()
        {
            List<TokenType> ttFalse = new List<TokenType> { TokenType.FALSE };
            List<TokenType> ttTrue = new List<TokenType> { TokenType.TRUE };
            List<TokenType> ttNil = new List<TokenType> { TokenType.NIL };
            List<TokenType> ttNumStr = new List<TokenType> { TokenType.NUMBER, TokenType.STRING }
            List<TokenType> ttLeftP = new List<TokenType> { TokenType.LEFT_PAREN }
            List<TokenType> ttRightP = new List<TokenType> { TokenType.RIGHT_PAREN }
            if (match(ttFalse)) return new Expr.Literal(false);
            if (match(ttTrue)) return new Expr.Literal(true);
            if (match(ttNil)) return new Expr.Literal(null);
            if (match(ttNumStr)) return new Expr.Literal(previous().literal);
            if (match(ttLeftP))
            {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
            throw error(peek(), "Expect Expression");
        }
    }
}

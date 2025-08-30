using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;


namespace LoxInterpreter
{
    public class Scanner
    {
        private readonly Dictionary<String, TokenType> keywords = new Dictionary<string, TokenType>()
        {
          {"and", TokenType.AND},
          {"class", TokenType.CLASS},
          {"else", TokenType.ELSE},
          {"false", TokenType.FALSE},
          {"for", TokenType.FOR},
          {"fun", TokenType.FUN},
          {"nil", TokenType.NIL},
          {"or", TokenType.OR},
          {"print", TokenType.PRINT},
          {"return", TokenType.RETURN},
          {"super", TokenType.SUPER},
          {"this", TokenType.THIS},
          {"true", TokenType.TRUE},
          {"var", TokenType.VAR},
          {"while", TokenType.WHILE},
        };
        private readonly String source;
        readonly List<Token> tokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(String source)
        {
            this.source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '!': addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else
                    {
                        addToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\t':
                case '\r':
                    break;
                case '\n':
                    line++;
                    break;
                case '"': addString(); break;
                default:
                    if (isDigit(c))
                    {
                        addNumber();
                    }
                    else if (isAlpha(c))
                    {
                        addIdentifier();
                    }
                    else
                    {
                        Lox.error(line, "Unexpexted Error");
                    }
                    break;
            }
        }

        private void addIdentifier()
        {
            while (isAlphaNumeric(peek())) advance();
            String text = source[start..current];
            try
            {
                TokenType type = keywords[text];
                addToken(type);
            }
            catch (KeyNotFoundException)
            {
                addToken(TokenType.IDENTIFIER);

            }
        }

        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
             (c >= 'A' && c <= 'Z') ||
             c == '_';
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        private void addNumber()
        {
            while (isDigit(peek())) advance();

            if (peek() == '.' && isDigit(peekNext()))
            {
                advance();
                while (isDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER, Double.Parse(source[start..current]));
        }



        private void addString()
        {
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                Lox.error(line, "Undetermined string.");
                return;
            }

            advance();

            String value = source[(start + 1)..(current - 1)];
            addToken(TokenType.STRING, value);
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source[current];
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source.ElementAt(current + 1);
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }
        private char advance()
        {
            return source.ElementAt(current++);
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, Object? literal)
        {
            String text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}

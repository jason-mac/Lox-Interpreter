using LoxInterpreter;
namespace LoxInterpreterTests
{
    public class ScannerTests
    {
        [Fact]
        public void TokenValuesTestNumber()
        {
            var source = "var x = 5;";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();

            Assert.NotEmpty(tokens); // ensure tokens are generated
            Assert.Equal(TokenType.VAR, tokens[0].type);
            Assert.Equal(TokenType.IDENTIFIER, tokens[1].type);
            Assert.Equal(TokenType.EQUAL, tokens[2].type);
            Assert.Equal(TokenType.NUMBER, tokens[3].type);
            Assert.Equal(TokenType.SEMICOLON, tokens[4].type);
            Assert.Equal(TokenType.EOF, tokens[5].type);
            Assert.Equal("var", tokens[0].lexeme);
            Assert.Equal("x", tokens[1].lexeme);
            Assert.Equal("=", tokens[2].lexeme);
            Assert.Equal("5", tokens[3].lexeme);
            Assert.Equal(";", tokens[4].lexeme);
            Assert.Equal(";", tokens[4].lexeme);
            Assert.Equal("", tokens[5].lexeme);
            Assert.Equal(5, (double)tokens[3].literal);
        }


        [Fact]
        public void TokenValuesTestBool()
        {
            var source1 = "var value = true;";
            var source2 = "var value2 = false;";
            var scanner = new Scanner(source1 + source2);
            var tokens = scanner.scanTokens();
            Assert.Equal("true", tokens[3].lexeme);
            Assert.Equal("false", tokens[8].lexeme);
        }

        [Fact]
        public void TokenValuesTestString()
        {
            var source = "var value = \"hello\";";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            Assert.Equal("hello", (string)tokens[3].literal);
            Assert.Equal(TokenType.STRING, tokens[3].type);
        }

        [Fact]
        public void TokenValuesTestTokenTypesOne()
        {
            var source = "( ) { } , . - + ; / *";
            string[] split = source.Split(" ");
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            for (int i = 0; i < 11; i++)
            {
                Assert.Equal(types[i], tokens[i].type);
                Assert.Equal(split[i], tokens[i].lexeme);
            }
        }

        [Fact]
        public void TokenValuesTestTokenTypesTwo()
        {
            var source = "! != = == > >= < <=";
            string[] split = source.Split(" ");
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            for (int i = 11; i < 19; i++)
            {
                Assert.Equal(types[i], tokens[i - 11].type);
                Assert.Equal(split[i - 11], tokens[i - 11].lexeme);
            }
        }

        [Fact]
        public void TokenValuesTestTokenTypesThree()
        {
            var source = "and class else false fun for if nil or print return super this true var while";
            string[] split = source.Split(" ");
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            for (int i = 22; i < 38; i++)
            {
                Assert.Equal(types[i], tokens[i - 22].type);
                Assert.Equal(split[i - 22], tokens[i - 22].lexeme);
            }
        }

        [Fact]
        public void TokenValuesTestDouble()
        {
            var source = "var x = 5.5;";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();

            Assert.NotEmpty(tokens); // ensure tokens are generated
            Assert.Equal(TokenType.VAR, tokens[0].type);
            Assert.Equal(TokenType.IDENTIFIER, tokens[1].type);
        }

        [Theory]
        [InlineData("1+1", 4)]   // expects 3 tokens: 1, +, 1, EOF
        [InlineData("()", 3)]     // expects 2 tokens: (, )
        public void TokenCountTheory(string source, int expectedCount)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            Assert.Equal(expectedCount, tokens.Count);
        }

        [Fact]
        public void ComplexExpressionTokens()
        {
            var source = "var total = (5 + 3.2) * 10 / 2 - 1;";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();

            Assert.Equal(TokenType.VAR, tokens[0].type);
            Assert.Equal(TokenType.IDENTIFIER, tokens[1].type);
            Assert.Equal(TokenType.EQUAL, tokens[2].type);
            Assert.Equal(TokenType.LEFT_PAREN, tokens[3].type);
            Assert.Equal(TokenType.NUMBER, tokens[4].type);
            Assert.Equal(TokenType.PLUS, tokens[5].type);
            Assert.Equal(TokenType.NUMBER, tokens[6].type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[7].type);
            Assert.Equal(TokenType.STAR, tokens[8].type);
            Assert.Equal(TokenType.NUMBER, tokens[9].type);
            Assert.Equal(TokenType.SLASH, tokens[10].type);
            Assert.Equal(TokenType.NUMBER, tokens[11].type);
            Assert.Equal(TokenType.MINUS, tokens[12].type);
            Assert.Equal(TokenType.NUMBER, tokens[13].type);
            Assert.Equal(TokenType.SEMICOLON, tokens[14].type);
        }

        [Fact]
        public void CommentsAreIgnored()
        {
            var source = "var x = 1; // this is a comment\nvar y = 2;";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();

            // Ensure the comment does not produce tokens
            Assert.Equal(TokenType.VAR, tokens[0].type);
            Assert.Equal(TokenType.IDENTIFIER, tokens[1].type);
            Assert.Equal(TokenType.NUMBER, tokens[3].type);
            Assert.Equal(TokenType.VAR, tokens[5].type); // y variable after newline
        }
    }
}

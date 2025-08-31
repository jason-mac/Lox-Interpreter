using LoxInterpreter;
namespace LoxInterpreterExpr
{
    public class ParserTests
    {

        [Fact]
        public void TestPrintNumber()
        {
            var source = "print 42;";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();

            Assert.Equal("42", output);
        }

        [Fact]
        public void TestPrintString()
        {
            var source = "print \"hi there\";";
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();

            Assert.Equal("hi there", output);
        }

        [Theory]
        [InlineData("print 1 + 2;", "3")]
        [InlineData("print 5 - 3;", "2")]
        [InlineData("print 2 * 3;", "6")]
        [InlineData("print 10 / 2;", "5")]
        [InlineData("print (1 + 2) * 3;", "9")]
        public void TestPrintNumberOperators(string source, string expected)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();

            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData("var x = 42; print x;", "42")]
        [InlineData("var x = \"hello\"; print x;", "hello")]
        [InlineData("var flag = true; print flag;", "True")]
        [InlineData("var flag = false; print flag;", "False")]
        public void VariableTests(string source, string expected)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();
            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData("print 5 == 5;", "True")]
        [InlineData("print 5 == 4;", "False")]
        [InlineData("print 5 >= 5;", "True")]
        [InlineData("print 5 > 5;", "False")]
        [InlineData("print 5 < 10;", "True")]
        [InlineData("print 5 > 10;", "False")]
        public void BooleanAndComparisonTests(string source, string expected)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();
            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData("if (true) print 123;", "123")]
        [InlineData("if (false) print 123; else print 456;", "456")]
        public void IfStatementTests(string source, string expected)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();
            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData("var i = 0; while (i < 3) { print i; i = i + 1; }", "0\n1\n2")]
        public void WhileLoopTests(string source, string expected)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var interpreter = new Interpreter();
            interpreter.interpret(statements);

            var output = stringWriter.ToString().Trim();
            Assert.Equal(expected, output);
        }
    }
}

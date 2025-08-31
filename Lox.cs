using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;

namespace LoxInterpreter
{

    public class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        public static void Main(String[] args)
        {
            if (args.Length > 1)
            {
                Console.Write("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runFile(String path)
        {
            StreamReader sr = new StreamReader(path);
            run(sr.ReadToEnd());
            //run(new String(Encoding.Default.GetChars(File.ReadAllBytes(path))));
            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);
        }


        private static void runPrompt()
        {
            Stream inputStream = Console.OpenStandardInput();
            for (; ; )
            {
                Console.Write("> ");
                String? line = Console.ReadLine();
                if (line == null) break;
                run(line);
                hadError = false;
            }
        }

        private static void run(String source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();

            //foreach (Token token in tokens)
            //{
            //    Console.WriteLine(token.toString());
            //}

            if (hadError) return;

            Resolver resolver = new Resolver(interpreter);
            resolver.Resolve(statements);

            if (hadError) return;
            interpreter.interpret(statements);

        }

        public static void error(int line, String message)
        {
            report(line, "", message);
        }

        public static void error(Token token, String message)
        {
            if (token.type == TokenType.EOF)
            {
                report(token.line, " at end", message);
            }
            else
            {
                report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.line}]");
            hadRuntimeError = true;
        }

        private static void report(int line, String where, String message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

        public static void Exit(int code)
        {
            System.Environment.Exit(code);
        }
    }
}

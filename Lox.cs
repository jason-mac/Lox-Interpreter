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
        static bool hadError = false;
        public static void Main(String[] args)
        {
            if (args.Length > 1)
            {
                Console.Write("Usage: cslox [script]");
                Environment.Exit(64);
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
            run(new String(Encoding.Default.GetChars(File.ReadAllBytes(path))));
            if (hadError) Environment.Exit(65);
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

            foreach (Token token in tokens)
            {
                Console.WriteLine(token.toString());
            }

        }

        public static void error(int line, String message)
        {
            report(line, "", message);
        }

        private static void report(int line, String where, String message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}

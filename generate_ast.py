#!/usr/bin/env python3
"""
Script to Generate Ast Code for Lox Interpreter
"""

import os
import sys


def tab(times):
    """return c# tab"""
    to_return = ""
    for _ in range(times):
        to_return += "\t\t"
    return to_return


def write_ast_file(output_dir, base_name, types):
    """define ast"""
    path = os.path.join(output_dir, f"{base_name}.cs")
    with open(path, "w", encoding="utf-8") as open_file:
        open_file.write("namespace LoxInterpreter" + "\n")
        open_file.write("{" + "\n")
        open_file.write(tab(1) + "public abstract class " + base_name + "\n")
        open_file.write(tab(1) + "{" + "\n")
        define_visitor(open_file, base_name, types)
        open_file.write(tab(2) + "public abstract R accept<R>(IVisitor<R> visitor);\n")
        open_file.write("\n")
        for type_ in types:
            split_ = type_.split(":")
            class_name = split_[0].strip()
            fields = split_[1].strip()
            define_type(open_file, base_name, class_name, fields)

        open_file.write(tab(1) + "}" + "\n")
        open_file.write("}" + "\n")


def define_type(open_file, base_name, class_name, fields_list):
    """define type"""
    open_file.write(tab(2) + "public class " + class_name + " : " + base_name + "\n")
    open_file.write(tab(2) + "{" + "\n")
    fields = fields_list.split(", ")
    for field in fields:
        open_file.write(tab(3) + "public readonly " + field + ";\n")
    open_file.write("\n")
    open_file.write(tab(3) + "public " + class_name + "(" + fields_list + ")" + "\n")
    open_file.write(tab(3) + "{\n")
    for field in fields:
        variable_name = field.split(" ")[1]
        open_file.write(
            tab(4) + "this." + variable_name + " = " + variable_name + ";\n"
        )
    open_file.write(tab(3) + "}\n")
    open_file.write("\n")
    open_file.write(f"{tab(3)}public override R accept<R>(IVisitor<R> visitor)\n")
    open_file.write(f"{tab(3)}{{\n")
    open_file.write(f"{tab(4)}return visitor.visit{class_name}{base_name}(this);\n")
    open_file.write(f"{tab(3)}}}\n")

    open_file.write(tab(2) + "}\n")
    open_file.write("\n")


def define_visitor(open_file, base_name, types):
    """define visitor"""
    open_file.write(tab(2) + "public interface IVisitor<R>" + "\n")
    open_file.write(tab(2) + "{" + "\n")
    for type_ in types:
        prod: str = ""
        prod = type_.split(":")[0].strip()
        open_file.write(
            f"{tab(3)}R visit{prod}{base_name}({prod} {base_name.lower()});\n"
        )

    open_file.write("\n")
    open_file.write(tab(2) + "}" + "\n")
    open_file.write("\n")


def main():
    """main"""
    if len(sys.argv) != 2:
        print("Usage: python script.py <output filename>")
        sys.exit(64)

    output_dir = sys.argv[1]
    expr_content = [
        "Assign   : Token name, Expr value",
        "Binary   : Expr left, Token oper, Expr right",
        "Call     : Expr callee, Token paren, List<Expr> arguments",
        "Get      : Expr obj, Token name",
        "Grouping : Expr expression",
        "Literal  : Object value",
        "Logical  : Expr left, Token oper, Expr right",
        "Unary    : Token oper, Expr right",
        "Variable : Token name",
    ]

    stmt_content = [
        "Block      : List<Stmt> statements",
        "Class      : Token name, List<Stmt.Function> methods",
        "Expression : Expr expression",
        "Function   : Token name, List<Token> parameters," + " List<Stmt> body",
        "If         : Expr condition, Stmt thenBranch," + " Stmt elseBranch",
        "Print      : Expr expression",
        "Return     : Token keyword, Expr value",
        "Var        : Token name, Expr initializer",
        "While      : Expr condition, Stmt body",
    ]
    write_ast_file(output_dir, "Expr", expr_content)
    write_ast_file(output_dir, "Stmt", stmt_content)


if __name__ == "__main__":
    main()

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Compiler
    {
        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText("decafFile.txt");

            int parameters = args.Length;
            switch(parameters){
                case 0:
                    Console.WriteLine("Texto ayuda");
                    break;

                case 3:
                    Console.WriteLine("filename: " + args[0].ToString());
                    switch(args[1]){
                        case "-o":
                            Console.WriteLine("Output en file: " + args[2].ToString());
                            break;
                        case "-target":
                            stage(args[1],args[2]);
                            break;

                        case "-opt":
                            switch(args[2]){
                                case "constant":
                                    Console.WriteLine("constant optimizing");
                                    break;
                                case "algebraic":
                                    Console.WriteLine("algebraic optimization");
                                    break;
                                default:
                                    Console.WriteLine("Cursed optimization >:)  F");
                                    break;
                            }
                            break;
                            case "-debug":
                                stage(args[1],args[2]);
                                break;                        
                        default:
                            Console.WriteLine("Cursed flag");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Jaja mucho o poco texto");
                    break;
            }
            void stage(String flag, String myArg){
                String message;
                if(flag=="-target"){
                    message="stage";
                }else{
                    message="debugging";
                }
                switch(myArg){
                    case "scan":
                        Console.WriteLine(message + ": scanning");
                        Scanner scan = new Scanner(text, message);
                        break;
                    case "parse":
                        Console.WriteLine(message + ": scanning");
                        Console.WriteLine(message + ": parsing");
                        var gramatica = "<program> : <class> <variable> <open_braces> <field_decl> <method_decl> <close_braces>\n"
                                        +"<field_decl> : (<type> <variable>) | (<variable> <open_brackets> <number> <close_brackets>)\n"
                                        +"<method_decl> : (<type> | <void>) <variable> <open_parents> <type> <variable> <block>\n"
                                        +"<block> : <open_braces> <var_decl>* <statement>* <close_braces>\n"
                                        +"<var_decl> : <type> <variable>\n"
                                        +"<statement> : (<location> <assign_op> <expr>) | <method_call> | (<if_stmt> <open_parents> <expr> <close_parents> <block> [<else_stmt> <block>]) | <return> <expr> | <break> | <continue> | <block>\n"
                                        +"<method_call> : (<variable> <open_parents> <expr>+ <close_parents>) | (<callout> <open_parents> <string_literal> <open_brackets> <callout_arg> <close_brackets> <close_parents>)\n"
                                        +"<location> : <variable> | (<variable> <open_brackets> <expr> <close_brackets>)\n"
                                        +"<expr> : <location> | <method_call> | <literal> | <expr> <bin_op> <expr> | !<expr> | (<open_parents> <expr> <close_parents>)\n"
                                        +"<callout_arg> : <expr> | <string_literal>\n"
                                        +"<char_literal> : <char_op> <variable> <char_op>\n"
                                        +"<string_literal> : <string_op> <variable> <string_op>\n"
                                        +"<bin_op> : <arith_op> | <rel_op> | <eq_op> | <cond_op>\n"
                                        +"<literal> : <number> | <variable> | <bool_literal>";
                        // separa producciones
                        var gramarG = Tools.GetProductions(gramatica);
                        // cambia estados
                        var setC = Parser.Items(gramarG).ToList();    
                        // forma la tabla
                        var tableAction = Parser.LRTable(setC, gramarG);
                        
                        var terminals = Tools.GetTerminals(gramarG);
                        var noTerminals = Tools.GetNoTerminals (gramarG);
                        var tokens = terminals.Union (noTerminals).ToArray();

                        System.Console.Write ("\t");
                        foreach(var token in tokens){
                            System.Console.Write (token + "\t");
                        }
                        System.Console.Write("\n");
                        for(int i = 0; i < tableAction.GetLength(0); i++){
                            System.Console.Write (i + "\t");
                            for(int j = 0; j < tableAction.GetLength(1); j++){
                                System.Console.Write (tableAction[i,j] + "\t");
                            }
                            System.Console.Write ("\n");
                        }
                        break;
                    case" ast":
                        Console.WriteLine(message + ": scanning");
                        Console.WriteLine(message + ": parsing");
                        Console.WriteLine(message + ": ast");
                        break;
                    case" semantic":
                        Console.WriteLine(message + ": scanning");
                        Console.WriteLine(message + ": parsing");
                        Console.WriteLine(message + ": ast");
                        Console.WriteLine(message + ": semantic");
                        break;
                    case" irt":
                        Console.WriteLine(message + ": scanning");
                        Console.WriteLine(message + ": parsing");
                        Console.WriteLine(message + ": ast");
                        Console.WriteLine(message + ": semantic");
                        Console.WriteLine(message + ": irt");
                        break;
                    case "codegen":
                        Console.WriteLine(message + ": scanning");
                        Console.WriteLine(message + ": parsing");
                        Console.WriteLine(message + ": ast");
                        Console.WriteLine(message + ": semantic");
                        Console.WriteLine(message + ": irt");
                        Console.WriteLine(message + ": code generation");

                        break;
                    default:
                        Console.WriteLine("Cursed stage >:)");
                        break;
                }
                
            }
        }
    }
}

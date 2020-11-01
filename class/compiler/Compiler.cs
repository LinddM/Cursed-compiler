using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
                        Console.WriteLine("Scan complete");
                        break;
                    case "parse":
                        Console.WriteLine(message + ": scanning");
                        Scanner scan_parse = new Scanner(text, message);
                        
                        Console.WriteLine("Scan complete");

                        Console.WriteLine(message + ": parsing");
                        
                        String [] gramarG = new string [] 
                        {
                            "program : class id open_braces field_decl method_decl close_braces",
                            "field_decl : var_decl open_brackets number close_brackets",
                            "field_decl : var_decl",
                            // "program : class id open_braces field_decl_list method_decl close_braces",
                            // "field_decl : var_decl open_brackets number close_brackets",
                            // "field_decl : var_decl",
                            // "field_decl_list : field_decl",
                            // "field_decl_list : field_decl field_decl_list",
                            "var_decl : type id",
                            "method_decl_type : type id",
                            "method_decl_type : void id",
                            "method_decl : method_decl_type open_parents var_decl close_parents block",
                            "block : open_braces var_decl statement close_braces",
                            "statement : break_op",
                            "statement : location assign_op expr",
                            "statement : method_call",
                            "statement : if_stmt open_parents expr close_parents block",
                            "statement : if_stmt open_parents expr close_parents block else_stmt block",
                            "statement : return expr",
                            "statement : continue",
                            "statement : block",
                            // "method_call : id open_parents expr close_parents",
                            // "method_call : callout open_parents string_literal open_brackets callout_arg close_brackets close_parents",
                            // "location : id",
                            // "location : id open_brackets expr close_brackets",
                            // "expr : location",
                            // "expr : method_call",
                            // "expr : literal",
                            // "expr : expr bin_op expr",
                            // "expr : expr",
                            // "expr : open_parents expr close_parents",
                            // "callout_arg : expr",
                            // "callout_arg : string_literal",
                            // "char_literal : char_op id char_op",
                            // "string_literal : string_op id string_op",
                            // "bin_op : arith_op",
                            // "bin_op : rel_op",
                            // "bin_op : eq_op",
                            // "bin_op : cond_op",
                            // "literal : number",
                            // "literal : char_literal",
                            // "literal : bool_literal"
                        };
                        // cambia estados
                        var setC = Parser.Items(gramarG).ToList();    
                        // forma la tabla
                        var tableAction = Parser.LRTable(setC, gramarG);
                        
                        var terminals = Tools.GetTerminals(gramarG);
                        var noTerminals = Tools.GetNoTerminals (gramarG);
                        var tokens = terminals.Union (noTerminals).ToArray();

                        // poner los tokens en la tabla
                        for(int i=0; i<tokens.Length; i++){
                            tableAction[setC.Count, i]=tokens[i];
                        }

                        // retornar AST
                        var readTable = Parser.readTable(scan_parse.tokensAndTypes, tableAction, gramarG, tokens.Length);
                        
                        Console.WriteLine("Parse complete");
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

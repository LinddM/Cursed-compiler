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
                        Parser parse = new Parser(scan_parse);
                        Console.WriteLine("Parse complete");
                        break;

                    case "ast":
                        Console.WriteLine(message + ": scanning");
                        Scanner scan_ast = new Scanner(text, message);
                        Console.WriteLine("Scan complete");

                        Console.WriteLine(message + ": parsing");
                        Parser parse_ast = new Parser(scan_ast);
                        Console.WriteLine("Parse complete");

                        Console.WriteLine(message + ": ast");
                        Ast tree = new Ast(parse_ast, scan_ast.tokensAndTypes);
                        break;

                    case "semantic":
                        Console.WriteLine(message + ": scanning");
                        Scanner semPhase = new Scanner(text, message);
                        Console.WriteLine("Scan complete");

                        Console.WriteLine(message + ": parsing");
                        Parser parse_sem = new Parser(semPhase);
                        Console.WriteLine("Parse complete");

                        Console.WriteLine(message + ": ast");
                        Ast as_tree = new Ast(parse_sem, semPhase.tokensAndTypes);

                        Console.WriteLine(message + ": semantic");
                        Semantic semanticPhase = new Semantic(semPhase.tokensAndTypes);
                        Console.WriteLine("Fulfills semantic check: " + semanticPhase.uniqueVarsCheck);
                        break;
                        
                    case "irt":
                        Console.WriteLine(message + ": scanning");
                        Scanner scanIrt = new Scanner(text, message);
                        Console.WriteLine("Scan complete");

                        Console.WriteLine(message + ": parsing");
                        Parser parse_irt = new Parser(scanIrt);
                        Console.WriteLine("Parse complete");

                        Console.WriteLine(message + ": ast");
                        Ast irt_tree = new Ast(parse_irt, scanIrt.tokensAndTypes);

                        Console.WriteLine(message + ": semantic");

                        Console.WriteLine(message + ": irt");
                        Irt intermediate = new Irt(irt_tree, scanIrt.tokensAndTypes);
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

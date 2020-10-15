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
                        //scan_parse.tokensAndTypes.Values
                        Console.WriteLine("Scan complete");

                        Console.WriteLine(message + ": parsing");
                        
                        String [] gramarG = new string [] 
                        {
                            "program : class variable open_braces field_decl close_braces",
                            "field_decl : type variable",
                            "field_decl : type variable open_brackets number close_brackets",
                        };
                        // cambia estados
                        var setC = Parser.Items(gramarG).ToList();    
                        // forma la tabla
                        var tableAction = Parser.LRTable(setC, gramarG);
                        
                        var terminals = new String []{"$"}; //Tools.GetTerminals(gramarG);
                        var noTerminals = Tools.GetNoTerminals (gramarG);
                        var tokens = terminals.Union (noTerminals).ToArray();

                        // poner los tokens en la tabla
                        for(int i=0; i<tokens.Length; i++){
                            tableAction[setC.Count, i]=tokens[i];
                        }

                        var readTable = Parser.readTable(scan_parse.tokensAndTypes, tableAction, gramarG, tokens.Length);
                        
                        // System.Console.Write ("\t");
                        // foreach(var token in tokens){
                        //     System.Console.Write (token + "\t");
                        // }
                        // System.Console.Write("\n");
                        // for(int i = 0; i < tableAction.GetLength(0); i++){
                        //     System.Console.Write (i + "\t");
                        //     for(int j = 0; j < tableAction.GetLength(1); j++){
                        //         System.Console.Write (tableAction[i,j] + "\t");
                        //     }
                        //     System.Console.Write ("\n");
                        // }
                        // Console.WriteLine("Parse complete");
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

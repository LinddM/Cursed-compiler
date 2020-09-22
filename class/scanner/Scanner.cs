using System;
using System.Collections;
using System.Collections.Generic;

namespace Cursed_compiler
{
    class Scanner
    {   
        public Scanner(string text)
        {
            Console.WriteLine("Inside scan");

            List<List<String>> myCleanTokens = cleanTokens(text);
            Console.WriteLine("Scan finished");
        }
        static void classify(){
            // tomar cada token y ponerle su tipo (ver hoja de decaf)
        }
        static List<List<String>> cleanTokens(string text){
            /** separamos por lineas, quitamos comentarios y tokenizamos por espacios*/

            String [] Lines = text.Split("\r\n"); // separar texto por lineas
            
            for(int i=0; i<Lines.Length; i++){
                for(int j=0; j<Lines[i].Length; j++){
                    if(Lines[i][j]=='/' && Lines[i][j+1]=='/'){
                        Lines[i]=Lines[i].Substring(0, j); // eliminar comentarios
                    }
                }
            }
            List <string> line = new List<string>(Lines);
            line.RemoveAll(x=>x==""); // quitar lineas sin data
            // tokenizar lineas
            string [][] preTokens= new string [line.Count][];
            for(int i=0; i<line.Count; i++){
                preTokens[i]=line[i].Split();
            }
            // eliminar espacios
            List<List<String>> pseudoTokens = new List<List<string>>();
            for(int i=0; i<preTokens.Length; i++){
                List<String> nonSpaces = new List<string>();
                for(int j=0; j<preTokens[i].Length; j++){
                    if(preTokens[i][j]!=" "){
                        foreach(String element in divideSymbols(preTokens[i][j])){
                            nonSpaces.Add(element);
                        }
                    }
                }
                nonSpaces.RemoveAll(x=>x=="");
                pseudoTokens.Add(nonSpaces);
            }
            return pseudoTokens;
        }

        static List<String> divideSymbols(string token){
            /** quitamos punto y coma y tokenizamos separando por operadores*/

            List<String> myTokens = new List<string>();
            // separar operadores
            char [] tokens = token.ToCharArray();
            String accum="";
            foreach(char elem in tokens){
                // hacer esto con un string de operadores y contains
                if(elem!='>' && elem!='<' && elem!='=' && elem!='+' && elem!='-' && elem!='(' && elem!=')' && elem!='{' && elem!='}' && elem!=(char)34 && elem!=(char)39 && elem!='/' && elem!='*' && elem!='%' && elem!='&' && elem!='|'  && elem!='!' && elem!=','){
                    if(elem!=';'){ // quitar ; (punto y coma)
                        accum+=elem;
                    }
                }else{
                    if(accum!=""){
                        myTokens.Add(accum);
                        myTokens.Add(elem.ToString());
                    }else{
                        myTokens.Add(elem.ToString());
                    }
                    accum="";
                }
            }
            myTokens.Add(accum);
            return myTokens;
        }
    }
}
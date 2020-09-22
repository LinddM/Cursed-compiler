using System;
using System.Collections;
using System.Collections.Generic;

namespace Cursed_compiler
{
    class Scanner
    {   
        public Scanner(string text)
        {
            Console.Write("Inside scan");
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
            string [][] pseudoTokens= new string [preTokens.Length][];
            for(int i=0; i<preTokens.Length; i++){
                for(int j=0; j<preTokens[i].Length; j++){
                    //if(preTokens[i][j]!=" "){
                    //}
                }
            }
        }
    }
}
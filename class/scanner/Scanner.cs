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

            List<List<String>> myCleanTokens = cleanTokens(text); // limpiar tokens
            // tokensAndTypes positions: Id, [line, type, value]
            Dictionary<string, List<string>> tokensAndTypes = new Dictionary<string, List<string>>(classify(myCleanTokens)); // lista de tipos
            Console.WriteLine("Scan finished");
        }
        static Dictionary<string, List<string>> classify(List<List<String>> tokens){
            // tomar cada token y ponerle su tipo (ver hoja de decaf)
            Dictionary<string, List<string>> myDict = new Dictionary<string, List<string>>();
            Hashtable classifyTypes = new Hashtable(typesOfTokens());

            for(int i=0; i<tokens.Count; i++){
                for(int j=0; j<tokens[i].Count; j++){
                    if(classifyTypes.ContainsKey(tokens[i][j])){
                        // lineType positions: line, type, value
                        List <string> lineType = new List<string>(){(i+1).ToString(), classifyTypes[tokens[i][j]].ToString(), tokens[i][j]};
                        myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                    }else{
                        //next step: define regular expression for variables and errors
                        Boolean hasType=false;
                        if(isVariable(tokens[i][j])){
                            hasType=true;
                            List <string> lineType = new List<string>(){(i+1).ToString(),"<variable>", tokens[i][j]};
                            myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                        }
                        if(isObject(tokens[i][j])){
                            hasType=true;
                            List <string> lineType = new List<string>(){(i+1).ToString(),"<object>", tokens[i][j]};
                            myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                        }
                        if(isNumber(tokens[i][j])){
                            hasType=true;
                            List <string> lineType = new List<string>(){(i+1).ToString(),"<number>", tokens[i][j]};
                            myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                        }
                        if(!hasType){
                            List <string> lineType = new List<string>(){(i+1).ToString(),"error", tokens[i][j]};
                            myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                        }
                    }
                }
            }
            
            return myDict;
        }
        static Boolean isVariable(string token){
            Boolean isVar=false;

            SortedList<string, List <string>> states=new SortedList<string, List <string>>();

            // define if we have letters, numbers, underscore or nonvalid (any other char)
            for(int i=0; i<token.Length; i++){
                string type="other";
                if(Char.IsLetter(token[i])){
                    type="letter";
                }
                if(Char.IsDigit(token[i])){
                    type="number";
                }
                if(states.ContainsKey("nonvalid")){
                    break;
                }
                // create list of states
                // id (type_value_position), type, value
                List <string> typeValue = new List<string>(){type, token[i].ToString()};

                // create state
                switch(type){
                    case "letter":
                    states.Add(i.ToString()+"letter_", typeValue);
                        break;
                    case "number":
                        states.Add(i.ToString()+"number_", typeValue);
                        break;
                    case "other":
                        if(token[i]=='_'){
                            states.Add(i.ToString()+"unders_", typeValue);
                        }else{
                            states.Add("nonvalid", typeValue);
                        }
                        break;
                    default:
                        states.Add("nonvalid", typeValue);
                        break;
                }
            }
            // check if we have valid states
            isVar=checkStatesVar(states);

            return isVar;
        }
        static Boolean checkStatesVar(SortedList<string, List <string>> stateList){
            /* Define if list of states belongs to our NFA or not*/
            Boolean isVar=true;
            var vals = stateList.Values;
            if(vals.Count<1){
                return false;
            }
            foreach(string elem in stateList.Keys){
                if(elem=="nonvalid"){
                    return false;
                }
            }
            if(vals[0][0]!="letter"){
                return false;
            }
            return isVar;
        }
        static Boolean isObject(string token){
            Boolean isVar=false;
            return isVar;
        }
        static Boolean isNumber(string token){
            Boolean isVar=false;
            return isVar;
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
            //line.RemoveAll(x=>x==""); // quitar lineas sin data
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
                // poner corchetes
                if(elem!='>' && elem!='<' && elem!='=' && elem!='+' && elem!='-' && elem!='(' && elem!=')' && elem!='{' && elem!='}' && elem!=(char)34 && elem!=(char)39 && elem!='/' && elem!='*' && elem!='%' && elem!='&' && elem!='|'  && elem!='!' && elem!=','  && elem!='[' && elem!=']'){
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
        static Hashtable typesOfTokens(){
            Hashtable classifyTypes = new Hashtable();
            classifyTypes.Add("{","<open_braces>");
            classifyTypes.Add("}","<close_braces>");
            classifyTypes.Add("(","<open_parents>");
            classifyTypes.Add(")","<close_parents>");
            classifyTypes.Add("[","<open_brackets>");
            classifyTypes.Add("]","<close_brackets>");
            classifyTypes.Add("+","<arith_op>");
            classifyTypes.Add("-","<arith_op>");
            classifyTypes.Add("/","<arith_op>");
            classifyTypes.Add("*","<arith_op>");
            classifyTypes.Add("%","<arith_op>");
            classifyTypes.Add("callout","<method_call>");
            classifyTypes.Add("=","<asign_op>");
            classifyTypes.Add("+=","<asign_op>");
            classifyTypes.Add("-=","<asign_op>");
            classifyTypes.Add("==","<eq_op>");
            classifyTypes.Add("!=","<eq_op>");
            classifyTypes.Add("&&","<cond_op>");
            classifyTypes.Add("||","<cond_op>");
            classifyTypes.Add((char)34, "<string_op>"); 
            classifyTypes.Add((char)39, "<char_op>");
            classifyTypes.Add("boolean","<type>");
            classifyTypes.Add("int","<type>");
            classifyTypes.Add("float","<type>");
            classifyTypes.Add("break","<break_op>");
            classifyTypes.Add("false","<bool_literal>");
            classifyTypes.Add("true","<bool_literal>");
            classifyTypes.Add("if","<if_stmt>");
            classifyTypes.Add("else","<else_stmt>");
            classifyTypes.Add("while","<while_stmt>");
            classifyTypes.Add("for","<for_stmt>");
            classifyTypes.Add("new","<new>");
            classifyTypes.Add("null","<null>");
            classifyTypes.Add("private","<private>");
            classifyTypes.Add("public","<public>");
            classifyTypes.Add("return","<return>");
            classifyTypes.Add("static","<static>");
            classifyTypes.Add("super","<super>");
            classifyTypes.Add("this","<this>");
            classifyTypes.Add("void","<void>");
            classifyTypes.Add("continue","<continue>");
            classifyTypes.Add("class","<class>");
            classifyTypes.Add("extends","<extends>");
            classifyTypes.Add("print","<print>");
            return classifyTypes;
        }
    }
}
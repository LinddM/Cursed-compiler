using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cursed_compiler
{
    class Scanner
    {   
        public Dictionary<string, List<string>> tokensAndTypes;
        public Scanner(string text, string mode)
        {
            Console.WriteLine("Inside scan");

            List<List<String>> myCleanTokens = cleanTokens(text, mode); // limpiar tokens
            // tokensAndTypes positions: Id, [line, type, value]
            tokensAndTypes = new Dictionary<string, List<string>>(classify(myCleanTokens)); // lista de tipos
        }        

        static Dictionary<string, List<string>> classify(List<List<String>> tokens){
            // tomar cada token y ponerle su tipo (ver hoja de decaf)
            Dictionary<string, List<string>> myDict = new Dictionary<string, List<string>>();
            Hashtable classifyTypes = new Hashtable(typesOfTokens());

            for(int i=0; i<tokens.Count; i++){
                for(int j=0; j<tokens[i].Count; j++){
                    List <string> lineType = new List<string>();
                    if(classifyTypes.ContainsKey(tokens[i][j])){
                        // lineType positions: line, type, value
                        lineType = new List<string>(){(i+1).ToString(), classifyTypes[tokens[i][j]].ToString(), tokens[i][j]};
                        
                    }else{
                        //next step: define regular expression for variables and errors
                        Boolean hasType=false;
                        if(isVariable(tokens[i][j])){
                            hasType=true;
                            lineType = new List<string>(){(i+1).ToString(),"<variable>", tokens[i][j]};
                        }
                        if(isObject(tokens[i][j])){
                            hasType=true;
                            lineType = new List<string>(){(i+1).ToString(),"<object>", tokens[i][j]};
                        }
                        if(isNumber(tokens[i][j])){
                            hasType=true;
                            lineType = new List<string>(){(i+1).ToString(),"<number>", tokens[i][j]};
                        }
                        if(!hasType){
                            if(tokens[i][j]!=""){
                                lineType = new List<string>(){(i+1).ToString(),"error", tokens[i][j]};
                            }
                        }
                    }
                    myDict.Add(tokens[i][j]+"_"+(i+1).ToString()+"."+(j+1).ToString(), lineType);
                }
            }


            // var lista = myDict.Values.ToList();
            // int h = lista.Count;
            // using (StreamWriter file = new StreamWriter("output.csv"))
            //     foreach (var entry in lista){
            //         if(entry[1]=="error"){
            //             Console.WriteLine("Error en la linea "+entry[0]+": "+entry[2]);
            //         }
            //         file.WriteLine(string.Join(",",entry)); 
            //     }
            //     Console.WriteLine("Lista de variables y tipos en output.csv");            

            return myDict;
        }

        public class ConsoleFriendlyList<T> : List<T>
        {
            public override string ToString()
            {
                return $"List: {string.Join(", ", this)}";
            }
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
            // there's something to check in the token
            if(vals.Count<1){
                return false;
            }
            // all types are valid in the states
            foreach(string elem in stateList.Keys){
                if(elem=="nonvalid"){
                    return false;
                }
            }
            // the fist state allows a letter
            if(vals[0][0]!="letter"){
                return false;
            }
            return isVar;
        }
        static Boolean isObject(string token){
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
                        }if(token[i]=='.'){
                            states.Add(i.ToString()+"period_", typeValue);
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
            return checkStatesObj(states);
        }
        static Boolean checkStatesObj(SortedList<string, List <string>> stateList){
            /* Define if list of states belongs to our NFA or not*/
            Boolean isObj=true;
            var vals = stateList.Values;
            // there's something to check in the token
            if(vals.Count<1){
                return false;
            }
            // the first char must be a letter
            if(vals[0][0]!="letter"){
                return false;
            }
            // it must include just one period
            int countPeriod=0;
            for(int i=0; i<vals.Count; i++){
                if(vals[i][1]=="."){
                    countPeriod++;
                }
            }
            if(countPeriod>1){
                return false;
            }
            if(countPeriod==0){
                return false;
            }
            // all types are valid in the states
            foreach(string elem in stateList.Keys){
                if(elem=="nonvalid"){
                    return false;
                }
            }
            // allow only a letter after the period
            for(int i=0; i<vals.Count; i++){
                if(vals[i][0]=="other" && i<vals.Count-1){
                    if(vals[i+1][0]!="letter"){
                        return false;
                    }
                }
            }
            return isObj;
        }
        static Boolean isNumber(string token){
            /* Checks if a token is a valid number*/
            Boolean isNum=false;

            SortedList<string, List <string>> states=new SortedList<string, List <string>>();

            // define if we have letters, numbers, underscore or nonvalid (any other char)
            for(int i=0; i<token.Length; i++){
                string type="other";
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
                    case "number":
                        states.Add(i.ToString()+"number_", typeValue);
                        break;
                    case "other":
                        if(token[i]=='.'){
                            states.Add(i.ToString()+"period_", typeValue);
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
            isNum=checkStatesNum(states);
            return isNum;
        }
        static Boolean checkStatesNum(SortedList<string, List <string>> stateList){
            /* Define if list of states belongs to our NFA or not*/
            Boolean isNum=true;
            var vals = stateList.Values;
            // there's something to check in the token
            if(vals.Count<1){
                return false;
            }
            // all types are valid in the states
            foreach(string elem in stateList.Keys){
                if(elem=="nonvalid"){
                    return false;
                }
            }
            // the first char must be a number
            if(vals[0][0]!="number"){
                return false;
            }
            // the last char must be a number
            if(vals[stateList.Count-1][0]!="number"){
                return false;
            }
            // it must include just one period
            int countPeriod=0;
            for(int i=0; i<vals.Count; i++){
                if(vals[i][1]=="."){ // vals[i][0]=="other" && vals[i][1]=="."
                    countPeriod++;
                }
            }
            if(countPeriod>1){
                return false;
            }
            return isNum;
        }
        static List<List<String>> cleanTokens(string text, string mode){
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
                            if(mode=="debugging" && element!=""){
                                Console.WriteLine("Leyendo "+element);
                            }
                            nonSpaces.Add(element);
                        }
                    }
                }
                nonSpaces.RemoveAll(x=>x=="");
                pseudoTokens.Add(nonSpaces);
            }
            return pseudoTokens;
        }
        static void removeDuplicates(String str) 
{ 
    List<char> v = new List<char>(); 
    for (int i = 0; i < str.Length; ++i)  
    { 
        v.Add(str[i]); 
  
        if (v.Count > 2)  
        { 
            int sz = v.Count; 
  
            // removing three consecutive duplicates 
            if (v[sz - 1] == v[sz - 2] &&  
                v[sz - 2] == v[sz - 3])  
            { 
                v.RemoveRange(sz-3,3); // Removing three characters 
                                // from the string 
            } 
        } 
    } 
  
} 
                       static List<String> divideSymbols(string token){

           

            List<String> myTokens = new List<string>();
            char [] tokens = token.ToCharArray();
            String accum="";
            String accum1 = "";
            String accum2 = "";
            

            for(int i=0;i<tokens.Length; i++){
                try{
                    if((tokens[i]!='>' && tokens[i]!='<' && tokens[i]!='=' && tokens[i]!='+' && tokens[i]!='-' && tokens[i]!='(' && tokens[i]!=')' 
                    && tokens[i]!='{' && tokens[i]!='}' && tokens[i]!=(char)34 && tokens[i]!=(char)39 && tokens[i]!='/' && tokens[i]!='*' && tokens[i]!='%' /*&& 
                    tokens[i]!='&' && tokens[i]!='|' */ && tokens[i]!='!' && tokens[i]!=','  && tokens[i]!='[' && tokens[i]!=']' ) ){

                    if((tokens[i] == '>' || tokens[i]== '<' || tokens[i]== '!' && tokens[i+1] == '=')){
                        
                        Console.WriteLine("pass");
                        
                    }else if (tokens[i]!=';' ){
                        accum+=tokens[i];
                        

                    }

                }else{
                    

                    if(accum!="" ){
                        myTokens.Add(accum);
                        myTokens.Add(tokens[i].ToString());

                    }else{
                        
                        
                        

                        myTokens.Add(tokens[i].ToString());
                    }
                    accum="";
                }
                if((tokens[i]== '>' ||tokens[i]== '<' || tokens[i]== '!' || tokens[i]== '=' || tokens[i] == '+' || tokens[i] == '-') && tokens[i+1] == '='){
                        removeDuplicates(accum);
                        accum1 = tokens[i].ToString()+tokens[i+1].ToString();
                        myTokens.Add(accum1);
                        i++;
                       
                    }
     
                }catch(IndexOutOfRangeException){

                }

            }

            
            for(var i =0; i <myTokens.Count-1;i++){
                if((myTokens[i] == ">" && myTokens[i+1] == ">=") || (myTokens[i] == "<" && myTokens[i+1] == "<=") || (myTokens[i] == "!" && myTokens[i+1] == "!=") || (myTokens[i] == "-" && myTokens[i+1] == "-=") || (myTokens[i] == "+" && myTokens[i+1] == "+=") ){
                    myTokens.Remove(myTokens[i]);


                    
                }else if ((myTokens[i] == "&" && myTokens[i+1] == "&") || (myTokens[i] == "|" && myTokens[i+1] == "|")  ){
                        accum2 = tokens[i].ToString()+tokens[i+1].ToString();
                        myTokens.Add(accum2);
                        i++;

                }

                
                
            }
            for(var i =0; i <myTokens.Count-1;i++){
                if((myTokens[i] == "&" && myTokens[i+1] == "&" /*&& myTokens[i+2] == "&&")  || (myTokens[i] == "|" && myTokens[i+1] == "|") || (myTokens[i] == "-" && myTokens[i+1] == "=") || (myTokens[i] == "+" && myTokens[i+1] == "="*/)){
                    myTokens.Remove(myTokens[i]);
                    myTokens.Remove(myTokens[i+1]);
                    

                }

            }

            


            
            myTokens.Add(accum);
            
            return myTokens;
        }
        


        static Hashtable typesOfTokens(){
            Hashtable classifyTypes = new Hashtable();
            classifyTypes.Add("{","open_braces");
            classifyTypes.Add(">=","rel_op");
            classifyTypes.Add("<=","rel_op");
            classifyTypes.Add("}","close_braces");
            classifyTypes.Add("(","open_parents");
            classifyTypes.Add(")","close_parents");
            classifyTypes.Add("[","open_brackets");
            classifyTypes.Add("]","close_brackets");
            classifyTypes.Add("+","arith_op");
            classifyTypes.Add("-","arith_op");
            classifyTypes.Add("<","rel_op");
            classifyTypes.Add(">","rel_op");
            classifyTypes.Add("/","arith_op");
            classifyTypes.Add("*","arith_op");
            classifyTypes.Add("%","arith_op");
            classifyTypes.Add("callout","callout");
            classifyTypes.Add("=","asign_op");
            classifyTypes.Add("+=","asign_op");
            classifyTypes.Add("-=","asign_op");
            classifyTypes.Add("==","eq_op");
            classifyTypes.Add("!=","not_eq_op");
            classifyTypes.Add("!","exclam"); // hay que quitarlo
            classifyTypes.Add("&&","cond_op");
            classifyTypes.Add("||","cond_op");
            classifyTypes.Add("\"", "string_op"); // (char)34
            classifyTypes.Add("'", "char_op"); // (char)39
            classifyTypes.Add(",","comma_sep");
            classifyTypes.Add("boolean","type");
            classifyTypes.Add("int","type");
            classifyTypes.Add("float","type");
            classifyTypes.Add("break","break_op");
            classifyTypes.Add("false","bool_literal");
            classifyTypes.Add("true","bool_literal");
            classifyTypes.Add("if","if_stmt");
            classifyTypes.Add("else","else_stmt");
            classifyTypes.Add("while","while_stmt");
            classifyTypes.Add("for","for_stmt");
            classifyTypes.Add("new","new");
            classifyTypes.Add("null","null");
            classifyTypes.Add("private","private");
            classifyTypes.Add("public","public");
            classifyTypes.Add("return","return");
            classifyTypes.Add("static","static");
            classifyTypes.Add("super","super");
            classifyTypes.Add("this","this");
            classifyTypes.Add("void","void");
            classifyTypes.Add("continue","continue");
            classifyTypes.Add("class","class");
            classifyTypes.Add("extends","extends");
            classifyTypes.Add("print","print");
            return classifyTypes;
        }
    }
}
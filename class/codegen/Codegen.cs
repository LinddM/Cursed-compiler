using System;
using System.Collections.Generic;

namespace Cursed_compiler
{
    class Codegen
    {
        public string myCode;
        public Codegen(List<List<string>> tac){
            List<string> accum = new List<string>();
            for(int i=0; i<tac.Count-1; i++){
                if(tac[i][0]!=null){
                    char first=tac[i][0][1];
                    switch(first){
                        case 't':
                            bool op=false;
                            char opType=' ';
                            // define if it requires operation
                            for(int j=0; j<tac[i][1].Length-1; j++){
                                var compare=tac[i][1][j];
                                if(compare=='+' || compare=='-' || compare=='/' || compare=='*'){
                                    opType=compare;
                                    op=true;
                                    break;
                                }
                            }
                            // move first ocurrence
                            bool alone=false;
                            for(int j=0; j<tac[i][1].Length-1; j++){
                                if(tac[i][1][j]=='='){
                                    // get val
                                    alone=true;
                                    string val="";
                                    for(int k=j+1; k<tac[i][1].Length; k++){
                                        char thing=tac[i][1][k];
                                        if(thing!=' ' && thing!='+' && thing!='-' && thing!='/' && thing!='*'){
                                            val+=thing;
                                        }
                                    }
                                    accum.Add("LI " + tac[i][0] + " " + val);
                                    break;
                                }
                            }
                            if(!alone){
                                string [] firstL = tac[i][1].Split(' ');
                                string first1 = firstL[0];
                                accum.Add("LI " + tac[i][0] + " " + first1);
                            }
                            if(op){
                                // mover otros y hacer operacion
                                for(int j=0; j<tac[i][1].Length; j++){
                                    var compare = tac[i][1][j];
                                    if(compare=='+' || compare=='-' || compare=='/' || compare=='*'){
                                        // get val
                                        string val="";
                                        for(int k=j+1; k<tac[i][1].Length; k++){
                                            char thing=tac[i][1][k];
                                            if(thing!=' '){
                                                val+=thing;
                                            }
                                        }
                                        accum.Add("LI " + tac[i][0] + " " + val);
                                        break;
                                    }
                                }
                                // tomo ultimos dos
                                string [] oneL = accum[accum.Count-2].Split(' ');
                                string [] twoL = accum[accum.Count-1].Split(' ');
                                switch(opType){
                                    case '+':
                                        accum.Add("ADD " + oneL[1] + " " + twoL[1]);
                                        break;
                                    case '-':
                                        accum.Add("SUB " + oneL[1] + " " + twoL[1]);
                                        break;
                                    case '*':
                                        accum.Add("MULT " + oneL[1] + " " + twoL[1]);
                                        break;
                                    case '/':
                                        accum.Add("DIV " + oneL[1] + " " + twoL[1]);
                                        break;
                                }
                            }
                            break;
                        case 'L':
                            accum.Add(tac[i][0] + ":");
                            if(tac[i][1].Split(' ')[0] == "_"){
                                goto case 't';
                            }else{
                                bool operation=false;
                                char operationType=' ';
                                // define if it requires operation
                                for(int j=0; j<tac[i][1].Length-1; j++){
                                    var compare=tac[i][1][j];
                                    if(compare=='+' || compare=='-' || compare=='/' || compare=='*'){
                                        operationType=compare;
                                        operation=true;
                                        break;
                                    }
                                }

                                // tomar identificadores
                                List<string> myValues = new List<string>();
                                foreach(string element in tac[i][1].Split('=')){
                                    if(element != " " && element != "+" && element != "-" && element != "/" && element != "=" && element != "*"){
                                        string myThing = "";
                                        foreach(char e in element){
                                            if(e != '+' && e != ' ' && e != '+' && e != '-' && e != '/' && e != '=' && e != '*'){
                                                myThing+=e;
                                            }
                                        }
                                        myValues.Add(myThing);
                                    }
                                }

                                List<string> myRegs= new List<string>();
                                // buscar registros de elementos
                                foreach(string v in myValues){ // valores a buscar en registros
                                    foreach(List<string> inst in tac){
                                        if(v == inst[1].Split(' ')[0]){
                                            myRegs.Add(inst[0]);
                                        }
                                    }
                                }

                                // efectuar operaciones
                                if(operation){
                                    switch(operationType){
                                        case '+':
                                            accum.Add("ADD " + myRegs[0] + " " + myRegs[1]);
                                            break;
                                        case '-':
                                            accum.Add("SUB " + myRegs[0] + " " + myRegs[1]);
                                            break;
                                        case '*':
                                            accum.Add("MULT " + myRegs[0] + " " + myRegs[1]);
                                            break;
                                        case '/':
                                            accum.Add("DIV " + myRegs[0] + " " + myRegs[1]);
                                            break;
                                    }
                                }else{
                                    // move
                                    accum.Add("MOVE " + myRegs[0] + " " + myRegs[1]);
                                }
                            }
                            accum.Add("SYSCALL");
                        break;
                    }
                }else{
                    // find type of comparison
                    string symbol = "";
                    for(int j=0; j<tac[i][1].Length-1; j++){
                        char mySymbol = tac[i][1][j];
                        if(mySymbol=='<' || mySymbol=='>' || mySymbol=='='){
                            if(tac[i][1][j+1] == '='){
                                symbol = mySymbol + "="; 
                            }
                            break;
                        }
                    }

                    // tomar L's
                    List<string> myDirections = new List<string>();
                    for(int j=0; j<tac[i][1].Length-1; j++){
                        if(tac[i][1][j] == 'L'){
                            myDirections.Add(tac[i][1][j-1].ToString() + tac[i][1][j] + tac[i][1][j+1]);
                        }
                    }
                    
                    // tomar valores a comparar
                    List<string> myValues = new List<string>();
                    foreach(string element in tac[i][1].Split(' ')){
                        if(element != "if" && element != "==" && element != "else" && element != "goto"){
                            if(element[0] != '_'){
                               myValues.Add(element); 
                            }
                        }
                    }

                    List<string> myRegs= new List<string>();
                    // buscar registros de elementos
                    foreach(string v in myValues){ // valores a buscar en registros
                        foreach(List<string> inst in tac){
                            if(v == inst[1].Split(' ')[0]){
                                myRegs.Add(inst[0]);
                            }
                        }
                    }

                    // meter elementos
                    switch(symbol){
                        case "<":
                            accum.Add("BLT " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                        case ">":
                            accum.Add("BGT " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                        case "==":
                            accum.Add("BEQ " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                        case "<=":
                            accum.Add("BLE " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                        case ">=":
                            accum.Add("BGE " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                        case "!=":
                            accum.Add("BNE " + myRegs[0] + " " + myRegs[1] + " " + myDirections[0]);
                            break;
                    }
                }
            }
            myCode=accum.ToString();
        }
    }
}

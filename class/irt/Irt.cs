using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Irt
    {
        public Irt(Ast tree, Dictionary<string, List<string>> tokens){

            var pretokens = tokens.Values.ToList();
			String [] inputTokens = new String[tokens.Count];
			String [] valuesTokens = new String[tokens.Count];

			for(int i=0; i<tokens.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
				valuesTokens[i]=pretokens[i].ToList()[2];
			}

            var myTAC = threeAddressCode(inputTokens, valuesTokens);
        }

        // tabla de codigo de tres direcciones o terceto
        // faltan loops
        public List<List<string>> threeAddressCode(String [] inputTokens, String [] valuesTokens){
            List<List<string>> myTable = new List<List<string>>();

            // registro, variable
            for(int i=0; i<inputTokens.Length; i++){
                if(inputTokens[i]=="id" && (inputTokens[i-1]!="class" && inputTokens[i-1]!="void")){
                    // veo si ya existe
                    if(myTable.Count!=0){
                        bool exists=false;
                        foreach(var item in myTable){
                            if(item[1]!=null && item[1].Split(" ")[0] == valuesTokens[i]){
                                exists=true;
                                // operaciones
                                if(inputTokens[i+1]=="asign_op" || inputTokens[i+1]=="arith_op" || inputTokens[i+1]=="cond_op"){
                                    // ingresar registro de la variable que esta operando
                                    myTable.Add(new List<string>{item[0], valuesTokens[i] + " " + valuesTokens[i+1] + " " + valuesTokens[i+2]});
                                    i=i+2;
                                    break;
                                }else if(inputTokens[i+1] == "rel_op" || inputTokens[i+1] == "eq_op" || inputTokens[i+1] == "not_eq_op"){
                                    // aca va saltos
                                    bool exists2=false;
                                    for(int go=myTable.Count-1; go>=0; go--){
                                        if(myTable[go][0][1] == 'L'){
                                            exists2=true;
                                            myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+1).ToString()});
                                            myTable.Add(new List<string>{"_L"+(Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+1).ToString(), valuesTokens[i+5]}); // poner logica aca
                                            myTable.Add(new List<string>{null, "goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString()});
                                            myTable.Add(new List<string>{"_L"+(Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString(), null}); // poner logica aca
                                            i=i+5;
                                            break;
                                        }
                                    }
                                    if(!exists2){
                                        myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L0"});
                                        myTable.Add(new List<string>{"_L0", valuesTokens[i+5]}); // poner logica aca
                                        myTable.Add(new List<string>{null, "goto _L1"});
                                        myTable.Add(new List<string>{"_L1", null}); // poner logica aca
                                        i=i+5;
                                    }
                                    break;
                                }
                            }
                        }
                        if(!exists){
                            // declaraciones
                            if(myTable[myTable.Count-1][0]!="_t7"){
                                if(inputTokens[i+1] != "asign_op"){
                                    myTable.Add(new List<string>{"_t"+(Int16.Parse(myTable[myTable.Count-1][0].ToCharArray()[2].ToString())+1).ToString(), valuesTokens[i]});
                                }else{
                                    myTable.Add(new List<string>{"_t"+(Int16.Parse(myTable[myTable.Count-1][0].ToCharArray()[2].ToString())+1).ToString(), valuesTokens[i] + " " + valuesTokens[i+1] + " " + valuesTokens[i+2]});
                                    i+=2;
                                }
                            }else{
                                // reciclar registros
                            }
                        }
                    }else{
                        if(inputTokens[i+1] != "asign_op"){
                            myTable.Add(new List<string>{"_t0", valuesTokens[i]});
                        }else{
                            myTable.Add(new List<string>{"_t0", valuesTokens[i] + " " + valuesTokens[i+1] + " " + valuesTokens[i+2]});
                            i+=2;
                        }
                        
                    }
                }
            }
            return myTable;
        }
    }
}

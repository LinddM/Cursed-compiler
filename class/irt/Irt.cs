using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Irt
    {
        public List<List<string>> tac;
        public Irt(Ast tree, Dictionary<string, List<string>> tokens){

            var pretokens = tokens.Values.ToList();
			String [] inputTokens = new String[tokens.Count];
			String [] valuesTokens = new String[tokens.Count];

			for(int i=0; i<tokens.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
				valuesTokens[i]=pretokens[i].ToList()[2];
			}

            tac = threeAddressCode(inputTokens, valuesTokens);
        }

        // tabla de codigo de tres direcciones o terceto
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
                                            myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString()});
                                            // revisar si hay un else
                                            string dec="";
                                            int aumento=0;
                                            for(int k=i; k<valuesTokens.Length-1; k++){
                                                if(inputTokens[k]=="else_stmt"){
                                                    for(int l=k+2; l<valuesTokens.Length-1; l++){
                                                        if(inputTokens[l]!="close_braces"){
                                                            dec+=valuesTokens[l];
                                                        }else{
                                                            aumento=l+1;
                                                            break;
                                                        }
                                                    }
                                                    myTable.RemoveAt(myTable.Count-1);
                                                    myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString() + " else goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+3).ToString()});
                                                    // myTable.Add(new List<string>{null, "goto _L" + (Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString()});
                                                    myTable.Add(new List<string>{"_L"+(Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+2).ToString(), dec}); // poner logica aca
                                                    break;
                                                }
                                            }
                                            int more=0;
                                            string add="";
                                            for(int k=i+5; k<inputTokens.Length-1; k++){
                                                if(inputTokens[k]!="close_braces"){
                                                    add+=valuesTokens[k];
                                                }else{
                                                    more=k+1;
                                                    break;
                                                }
                                            }
                                            myTable.Add(new List<string>{"_L"+(Int16.Parse(myTable[go][0].ToCharArray()[2].ToString())+3).ToString(), add});
                                            if(dec==""){
                                                i=more;
                                            }else{
                                                i=aumento;
                                            }
                                            break;
                                        }
                                    }
                                    // si no existe
                                    if(!exists2){
                                        myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L0"});
                                        string dec = "";
                                        int aumento=0;
                                        // buscar else
                                        for(int k=i; k<valuesTokens.Length-1; k++){
                                            if(inputTokens[k]=="else_stmt"){
                                                for(int l=k+2; l<valuesTokens.Length-i; l++){
                                                    if(inputTokens[l]!="close_braces"){
                                                        dec+=valuesTokens[l];
                                                    }else{
                                                        aumento=l+1;
                                                        break;
                                                    }
                                                }
                                                myTable.RemoveAt(myTable.Count-1);
                                                myTable.Add(new List<string>{null, valuesTokens[i-2] + " " + valuesTokens[i] + " " + valuesTokens[i+1] + " "+ valuesTokens[i+2] + " goto _L0 else goto _L1"});
                                                myTable.Add(new List<string>{"_L1", dec});
                                                break;
                                            }
                                        }
                                        // lo que esta dentro del if
                                        string add="";
                                        int more=0;
                                        for(int k=i+5; k<inputTokens.Length-1; k++){
                                            if(inputTokens[k]!="close_braces"){
                                                add+=valuesTokens[k];
                                            }else{
                                                more=k+1;
                                                break;
                                            }
                                        }
                                        myTable.Add(new List<string>{"_L0", add});
                                        if(dec==""){
                                            i=more;
                                        }else{
                                            i=aumento;
                                        }
                                    }else{
                                        // es de otro tipo
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
        public List<List<string>> assign(){
            List<List<string>> myList = new List<List<string>>(){};
            return myList;
        }

        public List<List<string>> relations(){
            List<List<string>> myList = new List<List<string>>(){};
            return myList;
        }
    }
}

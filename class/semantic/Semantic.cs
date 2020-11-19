using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Semantic
    {
        public bool uniqueVarsCheck;
        public Semantic(Dictionary<string, List<string>> tokens){
            bool myScopeCheck;
            bool myTypeCheck;
            // check scope
            if(checkScope(tokens)){
                myScopeCheck=true;
            }else{
                myScopeCheck=false;
            }
            // check use and types
            if(checkTypes(tokens)){
                myTypeCheck=true;
            }else{
                myTypeCheck=false;
            }
            uniqueVarsCheck = myScopeCheck && myTypeCheck;
        }
        public static Boolean checkTypes(Dictionary<string, List<string>> tokens){
            bool ans=true;

            var pretokens = tokens.Values.ToList();
            String [] inputTokens = new String[tokens.Count];
			String [] valuesTokens = new String[tokens.Count];

            for(int i=0; i<tokens.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
				valuesTokens[i]=pretokens[i].ToList()[2];
			}
            for(int i=0; i<tokens.Count; i++){
                if(!ans){
                    break;
                }
                // comparaciones
                if(inputTokens[i]=="id"){
                    if(inputTokens[i+1]=="rel_op" || inputTokens[i+1]=="not_eq_op" || inputTokens[i+1]=="cond_op"){
                        if(inputTokens[i+2]=="id"){
                            // checar tipos en posiciones anteriores
                            String firstType = valuesTokens[i-1]; // creo que esto esta mal
                            if(inputTokens[i-1]!=firstType){
                                ans=false;
                                break;
                            }
                            // comparar strings y chars
                        }else if(inputTokens[i+2]=="string_op" || inputTokens[i+2]=="char_op"){
                            if(i>3){
                                String firstType = inputTokens[i-2]; // creo que esto esta mal
                                if(inputTokens[i+2]=="string_op"){
                                    if(firstType!="string_op"){
                                        ans=false;
                                        break;
                                    }
                                }else{
                                    if(firstType!="char_op"){
                                        ans=false;
                                        break;
                                    }
                                }
                            }
                        }
                    // operaciones
                    }else if(inputTokens[i+1]=="arith_op"){
                        if(inputTokens[i+2]!="id"){
                            ans=false;
                            break;
                        }
                        // checar tipo del primero y del segundo
                        if(i>3){
                            String firstType = valuesTokens[i-1];
                            if(inputTokens[i+2] != firstType){ // creo que esto esta mal
                                ans=false;
                                break;
                            }
                        }
                    // asignaciones
                    }else if(inputTokens[i+1]=="asign_op"){
                        String firstType = valuesTokens[i-1];
                        String lastType = valuesTokens[i+1];
                        // if(firstType==valuesTokens){

                        // }
                    // negacion
                    }else if(inputTokens[i+1]=="exclam"){

                    }
                }
            }
            return ans;
        }
        public static Boolean checkScope(Dictionary<string, List<string>> tokens){
            bool ans=true;
            List<List<string>> varsStack=new List<List<string>>();

            var pretokens = tokens.Values.ToList();
            String [] inputTokens = new String[tokens.Count];
			String [] valuesTokens = new String[tokens.Count];

            for(int i=0; i<tokens.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
				valuesTokens[i]=pretokens[i].ToList()[2];
			}
            
            for(int i=0; i<tokens.Count; i++){
                if(!ans){
                    break;
                }
				if(inputTokens[i]=="open_braces"){
                    varsStack.Add(new List<string>(){"new scope", valuesTokens[i]});
                }else if(inputTokens[i] == "close_braces"){
                    for(int j=varsStack.Count-1; j>=0; j--){
                        if(varsStack[j][0]=="new scope"){
                            varsStack.RemoveAt(j);
                            break;
                        }else{
                            varsStack.RemoveAt(j);
                        }
                    }
                }
                if(i>0){
                    if((inputTokens[i]=="type" && inputTokens[i+1]=="id") || (inputTokens[i]=="class" && inputTokens[i+1]=="id") || inputTokens[i]=="void" && inputTokens[i+1]=="id"){
                        for(int j=varsStack.Count-1; j>=0; j--){
                            if(varsStack[j][0]=="new scope"){
                                break;
                            }
                            if(varsStack[j][1]==valuesTokens[i+1]){
                                ans=false;
                                return ans;
                            }
                        }
                        if(ans){
                            varsStack.Add(new List<string>(){valuesTokens[i], valuesTokens[i+1]});                        
                        }
                    }
                }else{
                    if(inputTokens[i]=="class" && inputTokens[i+1]=="id"){
                        varsStack.Add(new List<string>(){valuesTokens[i], valuesTokens[i+1]});
                    }
                }
			}
            return ans;
        }
    }
}

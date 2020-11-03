using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Semantic
    {
        public bool uniqueVarsCheck;
        public Semantic(Dictionary<string, List<string>> tokens){
            if(checkSemantics(tokens)){
                uniqueVarsCheck=true;
            }else{
                uniqueVarsCheck=false;
            }
        }
        public static Boolean checkSemantics(Dictionary<string, List<string>> tokens){
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
				if(inputTokens[i]=="open_braces" || inputTokens[i] == "open_parents" || inputTokens[i] == "open_brackets"){
                    varsStack.Add(new List<string>(){"new scope", valuesTokens[i]});
                }else if(inputTokens[i] == "close_braces" || inputTokens[i] == "close_parents" || inputTokens[i] == "close_brackets"){
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

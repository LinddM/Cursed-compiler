using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
	// structure to build AST
	class AST{
        public string definition;
		public string data;
		public List<AST> children;
		public List<AST> siblings;
        public AST(string definition, string data){
            this.definition=definition;
			this.data=data;
        }
       
    }
    class Parser
    { 
		public static AST giveHead(List<String> myStack, List<String> newStack, AST myTree, String head, String [] gramarG){
			// agrupar los hermanos para hacer cabeza
			if(head==null){
				AST newTree = new AST("new scope", null);
				newTree.children=new List<AST>(){new AST(myTree.definition, myTree.data)};
				newTree.siblings=new List<AST>();
				for(int i=0; i<myTree.siblings.Count; i++){
					newTree.children.Add(myTree.siblings[i]);
				}
				myTree=newTree;
			}else{
				foreach(String gramarRule in gramarG){
					String rule = gramarRule.Split(' ')[0];
					if(head==rule){
						AST newTree = new AST(head, null);
						newTree.children=new List<AST>(){new AST(myTree.definition, myTree.data)};
						newTree.siblings=new List<AST>();
						for(int i=0; i<myTree.siblings.Count; i++){
							newTree.children.Add(myTree.siblings[i]);
						}
						myTree=newTree;
					}
				}
			}
			
			return myTree;
		}
		public static AST giveBranch(AST myTree, String token, String valToken, String [] gramarG){
			// esto pasa solo al entrar el primer token
			if(myTree.data==null && myTree.definition==null){
				myTree.data=valToken;
				myTree.definition=token;
			}else{
				// ver jerarquia
				bool isHead=false;
				foreach(String gramarRule in gramarG){
					String head = gramarRule.Split(' ')[0];
					if(head==token)
						isHead=true;
				}
				if(isHead){
					myTree.children.Add(new AST(token, valToken));
				}else{
					myTree.siblings.Add(new AST(token, valToken));
				}
			}
			return myTree;
		}
		
		public static List<String> applyReduce(List<String> myStack, String gramarRule){
			String head = gramarRule.Split(' ')[0];
			String theStack = String.Join(" ", myStack.ToArray());

			int temp = gramarRule.IndexOf(": ")+2;
			String rule = gramarRule.Substring(temp);
			var newStack = theStack.Replace(rule, head);

			myStack = newStack.Split(' ').ToList();
			return myStack;
		}
		public static bool allowReduce(String myStack, String rule){
			// arreglar field_decl y var_decl creo
			bool ans=true;
			if(myStack.IndexOf("class id open_braces field_decl method_decl_type open_parents var_decl")!=-1 && rule=="var_decl"){
				ans=false;
			}
			return ans;
		}
		public static List<String> lastCheckReduce(List<String> myStack, String [] gramarRules){
			String theStack = String.Join(" ", myStack.ToArray());
			foreach(String gramarRule in gramarRules){
				int temp = gramarRule.IndexOf(": ")+2;
				String rule = gramarRule.Substring(temp);
				
				bool sub = theStack.Contains(rule); // corregir esto para que lo lea bien
				// maybe ver si la regla es el ultimo token en el string
				if(sub && allowReduce(theStack, rule)){
					String head = gramarRule.Split(' ')[0];
					int myStackInd = myStack.LastIndexOf(rule.Split(' ')[0]);
					myStack = myStack.GetRange(0, myStackInd);
					myStack.Add(head);
					break;
				}
			}
			return myStack;
		}
		public static List<String> readInTable(int myState, String token, int tokensNumber, String [,] tableAction, List<String> myStack, String [] inputTokens, int index, String [] gramarG, List<int> stateStack, String [] valuesOfTokens, String valToken, AST tree){
			// buscar indice en el que se encuentra en la tabla
			for(int j=0; j<tokensNumber; j++){
				int size = tableAction.GetLength(0)-1;
				var tableToken=tableAction[size, j];
				if(token==tableToken){
					var pos=tableAction[myState,j];
					if(pos!=null){
						if(pos[0]=='s'){
							myStack.Add(token);
							myState=int.Parse(pos.Substring(1));
							stateStack.Add(myState);
							// pasar aqui un pedazo para corroborar reducciones
							List <string> newStack = lastCheckReduce(myStack, gramarG);
							if(myStack != newStack){

								// manejo de estados
								int myDiff=0;
								for(int e=0; e<myStack.Count; e++){
									if(e<newStack.Count){
										if(newStack[e]!=myStack[e]){
											myDiff++;
										}
									}else{
										myDiff+=myStack.Count-newStack.Count;
										break;
									}
								}
								for(int y=0; y<myDiff; y++){
									stateStack.RemoveAt(stateStack.Count-1);
								}
								String myReplacement = newStack[newStack.Count-1];

								// poner nueva cabeza en AST
								tree = giveHead(myStack, newStack, tree, myReplacement, gramarG);

								if(myReplacement=="program"){
									return newStack;
								}
								newStack.RemoveAt(newStack.Count-1);

								myStack = readInTable(stateStack[stateStack.Count-1], myReplacement, tokensNumber, tableAction, newStack, inputTokens, index, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index], tree);
							}else{
								// poner nueva rama o cabeza en AST
								if(token=="open_braces" || token=="open_parents" || token=="open_brackets"){
									tree = giveHead(myStack, newStack, tree, null, gramarG);
								}else{
									tree = giveBranch(tree, valToken, token, gramarG);
								}

								myStack = readInTable(myState, inputTokens[index+1], tokensNumber, tableAction, myStack, inputTokens, index+1, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index+1], tree);
							}
							break;
						}
					}
				}
			}
			return myStack;
		}
		public static List<String> readTable(Dictionary<string, List<string>> tokensAndTypes, String [,] tableAction, String [] gramarG, int tokensNumber){
			// tomar tokens de la file
			var pretokens = tokensAndTypes.Values.ToList();
			String [] inputTokens = new String[tokensAndTypes.Count];
			String [] valuesTokens = new String[tokensAndTypes.Count];
			for(int i=0; i<tokensAndTypes.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
				valuesTokens[i]=pretokens[i].ToList()[2];
			}

			List<String> myStack = new List<string>();
			List<int> stateStack = new List<int>();
			int myState=0;

			AST myTree = new AST(null, null);
			myTree.children=new List<AST>();
			myTree.siblings=new List<AST>();
			myStack = readInTable(myState, inputTokens[myState], tokensNumber, tableAction, myStack, inputTokens, myState, gramarG, stateStack, valuesTokens, valuesTokens[myState], myTree);
			
			return myStack;
		}
        public static string[] Closure(string[] sI, string[] gramarGp)
		{
			var I = new List<string>(sI);
			for (var i = 0; i < I.Count; i++) {
				var elements = Tools.GetElements(I[i]);
				var eB = elements [2];
				var beta = elements[3];
				var a = elements [4];
				var aBeta = new []{ beta, a };
				foreach (var gama in Tools.GetGama(eB, gramarGp)){
					foreach (var b in Tools.First(aBeta, gramarGp)){
						var prod = Tools.MakeProduction(eB, gama, b);
						if(!I.Contains(prod))
							I.Add(prod);
					}
				}
			}
			return I.ToArray();

		}

		public static string[] GoTo(string[] I, string x, string[] gramarGp){
			var j = new List<string>();
			foreach(var prod in I){
				var elements = Tools.GetElements (prod);
				var eA = elements[0];
				var alpha = Tools.GetAlpha(prod);
				var eX = elements[2];
				var beta = string.Join(" ", Tools.GetBeta(prod));
				var a = elements [4];
				if (!string.IsNullOrEmpty(x) && eX == x)
				{
					//A -> αX.β, a
					var newElement = Tools.MakeProduction(eA, alpha, x, beta, a);
					if (!j.Contains(newElement))
						j.Add(newElement);
				}
			}
			return Closure(j.ToArray(), gramarGp);
		}

		public static ICollection<string[]> Items(string[] gramarGp){
			// aqui cambiamos de estados
			// tomamos la primera produccion para establecer el primer estado
			var sC = new List<string[]>();
			string first = Tools.FirstProduction(gramarGp[0]);
			// vemos el closure de la primera produccion
			var n = Closure(new[] { first }, gramarGp);
			sC.Add(n);
			
			// determinamos los siguientes estados con lo que falta de gramatica
			for (var i = 0; i < sC.Count; i++){
				var I = sC[i];
				foreach (var prod in I) {
					var elements = Tools.GetElements (prod);
					var x = elements [2];
					var itemSet = GoTo(I, x, gramarGp);
					if(itemSet.Any() && !Tools.ContainsItemsSet(sC, itemSet)) {
						sC.Add (itemSet);
					}
				}
			}
			return sC;
		}

		public static string[,] LRTable(List<string[]> setC, string[] gramarGp){
			var terminals = Tools.GetTerminals(gramarGp);
			var noTerminals = Tools.GetNoTerminals (gramarGp);

			var tokens = terminals.Union (noTerminals).ToArray();
			var action = new string[setC.Count()+1, tokens.Count()];

			for (int i = 0; i < setC.Count(); i++)
			{
				for (int j = 0; j < tokens.Count(); j++)
				{
					var isTerminal = Tools.IsTerminal(tokens[j]);
					var itemSet = GoTo(setC.ElementAt(i), tokens[j], gramarGp);
					var index = Tools.IndexOfItemsSet(setC, itemSet);

					if (index != -1)                    
						action[i, j] = isTerminal ? string.Format("s{0}", index) : index.ToString();

					// si el token no es terminal se busca un reduce
					if (isTerminal)
					{
						for (int prod = 0; prod < setC[i].Count(); prod++)
						{
							var production = setC[i][prod].ToString();
							var e1 = tokens[j].ToString();
							var e2 = Tools.RetLastElement(production);
							if (Tools.IsPointAtEnd(production) && e1 == e2)
							{                                
								index = Tools.IndexOfGramar(gramarGp, production);
								if (index != -1)                                    
									action[i, j] = string.Format("r{0}", index);
								if (index == 0)
									action[i, j] = string.Format("Aceptar", index);
							}
						}
					}
				}
			}
			return action;
		}
    }
}

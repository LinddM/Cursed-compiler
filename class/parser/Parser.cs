using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Parser
    { 

		public static class Globals{
			public static String generatedString="";
		}
		public List<string> generatedStack;
		public String [] gramarG = new string [] {
			"program : class id open_braces field_decl_list method_decl close_braces",
			"field_decl_list : field_decl",
			"field_decl_list : field_decl field_decl_list",
			"field_decl : var_decl open_brackets number close_brackets",
			"field_decl : var_decl",
			"var_decl : type id",
			"method_decl_type : type id",
			"method_decl_type : void id",
			"method_decl : method_decl_type open_parents var_decl close_parents block",
			"block : open_braces var_decl statement_list close_braces",
			"block : open_braces statement_list close_braces",
			"statement_list : statement",
			"statement_list : statement_list statement",
			"statement : break_op",
			"statement : id asign_op expr",
			"statement : if_stmt open_parents expr close_parents block",
			"statement : if_stmt open_parents expr close_parents block else_stmt block",
			"statement : return expr",
			"statement : for_stmt id asign_op expr comma_sep expr block",
			"expr : literal",
			"expr : id bin_op id",
			"expr : id bin_op number",
			"expr : id asign_op literal",
			"literal : number",
			"bin_op : arith_op",
			"bin_op : rel_op",
			"bin_op : eq_op",
			"bin_op : cond_op",
			"literal : char_literal",
			"literal : bool_literal"
			// "var_decl_list : var_decl",
			// "var_decl_list : var_decl var_decl_list"
			// "statement : method_call",
			// "statement : continue",
			// "method_call : id open_parents expr close_parents",
			// "method_call : callout open_parents string_literal open_brackets callout_arg close_brackets close_parents",
			// "location : id",
			// "location : id open_brackets expr close_brackets",
			// "expr : location",
			// "expr : method_call",
			// "expr : expr bin_op id",
			// "expr : id bin_op expr",
			// "expr : open_parents expr close_parents",
			// "callout_arg : expr",
			// "callout_arg : string_literal",
			// "char_literal : char_op id char_op",
			// "string_literal : string_op id string_op",
		};
		public Parser(Scanner scan_parse){ 
			// cambia estados
			var setC = Parser.Items(gramarG).ToList(); 
			// forma la tabla
			var tableAction = Parser.LRTable(setC, gramarG);
			
			var terminals = Tools.GetTerminals(gramarG);
			var noTerminals = Tools.GetNoTerminals (gramarG);
			var tokens = terminals.Union (noTerminals).ToArray();

			// poner los tokens en la tabla
			for(int i=0; i<tokens.Length; i++){
				tableAction[setC.Count, i]=tokens[i];
			}

			var readTable = Parser.readTable(scan_parse.tokensAndTypes, tableAction, gramarG, tokens.Length);
			generatedStack = Globals.generatedString.Split(" ").ToList();
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
			if(myStack.IndexOf("class id open_braces field_decl_list method_decl_type open_parents var_decl")!=-1 && rule=="var_decl"){
				ans=false;
			}
			if(myStack.IndexOf("class id open_braces field_decl_list")!=-1 && rule=="field_decl"){
				ans=false;
			}
			if(myStack.IndexOf("class id open_braces field_decl_list method_decl_type open_parents var_decl close_parents open_braces var_decl statement_list")!=-1 && rule=="statement"){
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

		public static List<String> reduceByRule(List<String> myStack, String theGrammarRule, String [] rules){
			String gramarRule="";
			foreach(char e in theGrammarRule){
				if(e!='r'){
					gramarRule+=e;
				}
			}
			int temp = rules[Int64.Parse(gramarRule)].IndexOf(": ")+2;
			String rule = rules[Int64.Parse(gramarRule)].Substring(temp);

			String head = rules[Int64.Parse(gramarRule)].Split(' ')[0];

			int myStackInd = myStack.LastIndexOf(rule.Split(' ')[0]);
			myStack = myStack.GetRange(0, myStackInd);
			myStack.Add(head);

			return myStack;
		}
		public static List<String> readInTable(int myState, String token, int tokensNumber, String [,] tableAction, List<String> myStack, String [] inputTokens, int index, String [] gramarG, List<int> stateStack, String [] valuesOfTokens, String valToken, String everything){
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
							everything+=" "+token;
							stateStack.Add(myState);
							// pasar aqui un pedazo para corroborar reducciones
							List <string> newStack = lastCheckReduce(myStack, gramarG);

							if(myStack != newStack){
								// manejo de estados
								int myDiff=myStack.Count-newStack.Count;
								// for(int e=0; e<myStack.Count; e++){
								// 	if(e<newStack.Count){
								// 		if(newStack[e]!=myStack[e]){
								// 			myDiff++;
								// 		}
								// 	}else{
								// 		myDiff+=myStack.Count-newStack.Count;
								// 		break;
								// 	}
								// }
								for(int y=0; y<=myDiff; y++){
									stateStack.RemoveAt(stateStack.Count-1);
								}
								String myReplacement = newStack[newStack.Count-1];

								if(myReplacement=="program"){
									return myStack;
								}
								newStack.RemoveAt(newStack.Count-1);

								myStack = readInTable(stateStack[stateStack.Count-1], myReplacement, tokensNumber, tableAction, newStack, inputTokens, index, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index], everything);
							}else{
								if(token=="method_decl" && myStack.LastIndexOf("method_decl")==myStack.Count-1){
									myStack = readInTable(myState, inputTokens[index], tokensNumber, tableAction, myStack, inputTokens, index+1, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index], everything);
								}else{
									myStack = readInTable(myState, inputTokens[index+1], tokensNumber, tableAction, myStack, inputTokens, index+1, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index+1], everything);
								}
							}
							break;
						}else if(pos[0]=='r'){
							List <string> newStack = reduceByRule(myStack, pos, gramarG);

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

								if(myReplacement=="program"){
									return myStack;
								}
								newStack.RemoveAt(newStack.Count-1);

								myStack = readInTable(stateStack[stateStack.Count-1], myReplacement, tokensNumber, tableAction, newStack, inputTokens, index, gramarG, stateStack, valuesOfTokens, valuesOfTokens[index], everything);
							}
						}
					}else{
						// Console.WriteLine("Ocurrió un error");
					}
				}
			}
			if(everything.Length > Globals.generatedString.Length){
				Globals.generatedString = everything;
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
			String everything="";

			List<String> myStack = new List<string>();
			List<int> stateStack = new List<int>();
			int myState=0;

			myStack = readInTable(myState, inputTokens[myState], tokensNumber, tableAction, myStack, inputTokens, myState, gramarG, stateStack, valuesTokens, valuesTokens[myState], everything);
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

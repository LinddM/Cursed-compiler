using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Parser
    { 
		public static List<String> applyReduce(List<String> myStack, String gramarRule){
			String head = gramarRule.Split(' ')[0];
			String theStack = String.Join(" ", myStack.ToArray());

			int temp = gramarRule.IndexOf(": ")+2;
			String rule = gramarRule.Substring(temp);
			var newStack = theStack.Replace(rule, head);

			myStack = newStack.Split(' ').ToList();
			return myStack;
		}
		public static List<String> lastCheckReduce(List<String> myStack, String [] gramarRules){
			String theStack = String.Join(" ", myStack.ToArray());
			foreach(String gramarRule in gramarRules){
				int temp = gramarRule.IndexOf(": ")+2;
				String rule = gramarRule.Substring(temp);
				if(rule == theStack){
					String head = gramarRule.Split(' ')[0];
					myStack = head.Split(' ').ToList();
					break;
				}
			}
			if(myStack[0]=="program"){
				myStack.Clear();
				myStack.Add("$");
			}
			return myStack;
		}
		public static List<String> readInTable(int myState, String token, int tokensNumber, String [,] tableAction, List<String> myStack, String [] inputTokens, int index, String [] gramarG){
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
							myStack = readInTable(myState, inputTokens[index+1], tokensNumber, tableAction, myStack, inputTokens, index+1, gramarG);
						}else if(pos[0]=='r'){
							myStack.Add(token);
							int reduceRuleNum = int.Parse(pos.Substring(1));
							myStack = applyReduce(myStack, gramarG[reduceRuleNum]);
						}else{
							myStack.Add("aceptar");
						}
					}
				}
			}
			//myStack = lastCheckReduce(myStack, gramarG);
			return myStack;
		}
		public static List<String> readTable(Dictionary<string, List<string>> tokensAndTypes, String [,] tableAction, String [] gramarG, int tokensNumber){
			// tomar tokens de la file
			var pretokens = tokensAndTypes.Values.ToList();
			String [] inputTokens = new String[tokensAndTypes.Count];
			for(int i=0; i<tokensAndTypes.Count; i++){
				inputTokens[i]=pretokens[i].ToList()[1];
			}

			List<String> myStack = new List<string>();
			int myState=0;
			// buscar token
			myStack = readInTable(myState, inputTokens[myState], tokensNumber, tableAction, myStack, inputTokens, myState, gramarG);

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

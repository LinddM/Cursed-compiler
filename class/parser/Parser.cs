using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Parser
    {
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
			var sC = new List<string[]>();
			string first = Tools.FirstProduction(gramarGp[0]);
			var n = Closure(new[] { first }, gramarGp);
			sC.Add(n);
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
			var action = new string[setC.Count(), tokens.Count()];

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

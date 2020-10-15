using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cursed_compiler
{

	public static class Tools
	{
		private const string _terminalPattern = "`[^`]+`";
		private const string _noTerminalPattern = @"[\w]+'?";
		private const string _produceElement = ":";
		private const string _orElement = "|";

		public static string[] First(string[] elements, string[] gramar){
			var firsts = new List<string> ();
			foreach (var element in elements) {
				var terminals = First (element, gramar);
				firsts.AddRange(terminals);
			}
			if (!firsts.Any ())
				firsts.Add ("$");
			return firsts.Distinct().ToArray();
		}

		public static string[] First(string element, string[] gramar){
			var firsts = new List<string> ();
			if (!string.IsNullOrEmpty(element) && 
				IsTerminal (element) && 
				element != "$") {
				firsts.Add (element.Trim());
			} else {
				foreach (var production in gramar) {
					var elementA = Regex.Split (production, _produceElement).First ().Trim ();
					if (element == elementA) {
						var prodFirsts = First (production);
						firsts.AddRange (prodFirsts);
					}
				}
			}
			return firsts.Distinct().ToArray();
		}

		public static string[] First(string production){
			string patternTerminal = string.Format ("({0} *|{1} *){2}", _produceElement.ToMeta(), _orElement.ToMeta(), _terminalPattern);
			var terminals = Regex.Matches(production, patternTerminal).Cast<Match>().Select(m=>m.Value);
					
			var terminalsList = terminals.Select(t => 
				t.Replace(_produceElement, string.Empty).Replace(_orElement, string.Empty)
				.Trim()).ToList();

			if (!terminalsList.Any ())
				terminalsList.Add("$");

			return terminalsList.ToArray();
		}

		public static string[] GetGama(string elemA, string[] gramar){
			var gamas = new List<string> ();
			foreach(var production in gramar){
				var elementA = Regex.Split(production, _produceElement).First().Trim();
				if (elementA == elemA) {
					var prodGamas = GetGama (production).ToList();
					gamas = gamas.Union(prodGamas).ToList();
				}
			}
			return gamas.ToArray();
		}


		public static string[] GetGama(string production){
			string gamaPattern = string.Format ("[^{0}^{1}]+", _orElement.ToMeta(), _produceElement.ToMeta());
			var gamas = Regex.Matches (production, gamaPattern).Cast<Match>().Select(m => m.Value.Trim()).ToList();
			gamas.RemoveAt (0);
			return gamas.ToArray();
		}

		public static string[] GetBeta(string production){
			string patternBbeta = string.Format ("\\.({0}|{1}| )+", _terminalPattern, _noTerminalPattern);
			var matchesBbeta = Regex.Matches (production, patternBbeta).Cast<Match>().Select(m=>m.Value);
			var elementBbeta = matchesBbeta.FirstOrDefault();

			if(elementBbeta != null){
				var elements = elementBbeta.Split(' ');
				if (elements.Count () >= 2) {
					var betaElements = elements.ToList();
					betaElements.RemoveAt(0);
					return betaElements.ToArray();
				}
			}
			return new string[]{};
		}


		public static bool IsTerminal(string element)
		{
			return element.All(t => !char.IsUpper(t));
		}

		public static string[] GetElements(string production){
			// ver k pedo aca
			string patternElement = string.Format ("{0}'?", _noTerminalPattern);
			string patternAlpha = string.Format("( )*({0}|{1})?( )*\\.", _noTerminalPattern, _terminalPattern);
			string patternBbeta = string.Format("\\.({0}|{1}| )+", _noTerminalPattern, _terminalPattern);
			string aPattern = string.Format (", *({0}|{1}|\\$)?", _noTerminalPattern, _terminalPattern);

			var matches = Regex.Matches(production, patternElement).Cast<Match>().Select(m=>m.Value);
			var matchesBbeta = Regex.Matches (production, patternBbeta).Cast<Match>().Select(m=>m.Value);

			var elementA = matches.FirstOrDefault();
			var alpha = Regex.Matches (production, patternAlpha).Cast<Match> ().Select (m => m.Value).FirstOrDefault();


			var elementBbeta = matchesBbeta.FirstOrDefault();
			string elementB = null; 
			string beta = null;
			if(elementBbeta != null){
				var elements = elementBbeta.Split(' ');
				if (elements.Any ()) {
					elementB = elements [0];
				}
				if (elements.Count () >= 2) {
					beta = elements[1];
				}
			}

			var a = Regex.Matches(production, aPattern).Cast<Match>().Select(m => m.Value).FirstOrDefault();

			if (alpha != null) {
				alpha = alpha.Replace(".",string.Empty).Trim();
			}
			if (elementB != null) {
				elementB = elementB.Replace (".", string.Empty).Trim();
			}
			a = a != null ? 
				a.Replace (",", string.Empty).Trim() : "$";

			return new []{ elementA, alpha, elementB, beta, a};
		}

		public static bool ContainsItemsSet(List<string[]> itemsSet, string[] item)
		{
			for (var i = 0; i < itemsSet.Count(); i++)
			{
				if(AreEqual(itemsSet[i], item))
					return true;
			}
			return false;
		}

		public static int IndexOfGramar(string[] gramar, string production) {
			int pos = production.IndexOf (',');
			production = production.Remove (pos - 1);

			for (var i = 0; i < gramar.Count(); i++)
			{
				if(production.Trim() == gramar[i].Trim()){
					return i;
				}
			}
			return -1;
		}

		public static int IndexOfItemsSet(List<string[]> itemsSet, string[] item)
		{
			for (var i = 0; i < itemsSet.Count(); i++)
			{
				if(AreEqual(itemsSet[i], item))
					return i;
			}
			return -1;
		}

		public static bool AreEqual(string[] array1, string[] array2)
		{
			if (array1.Count() != array2.Count())
				return false;
			for (var i = 0; i < array1.Count(); i++)
			{
				if (array1[i] != array2[i])
					return false;
			}
			return true;
		}


		public static bool IsPointAtEnd(string production){
			if (production.Contains (".,"))
				return true;
			return false;
		}

		public static string MakeProduction(string elementA, string alpha, string x, string beta, string a){
			var prod = elementA + " " + _produceElement; 
			if (!string.IsNullOrEmpty(alpha))
				prod += " " + alpha;
			if (x != null)
			{
				prod = prod + " " + x;
			}
			prod += ".";
			if (beta != null)
				prod += beta;

			prod += ", " + a;
			return prod;
		}

		public static string MakeProduction(string eB, string gama, string b){
			return string.Format("{0} {1} .{2}, {3}", eB, _produceElement, gama, b);
		}

		public static string[] GetTerminals(string[] gramarGp){
			var totalTerminals = new List<string> ();
			foreach(var production in gramarGp){
				var terminals = Regex.Matches(production, _terminalPattern).Cast<Match>().Select(m => m.Value).Distinct();
				totalTerminals = totalTerminals.Union(terminals).ToList();
			}
			totalTerminals.Add("$");
			return totalTerminals.ToArray();
		}

		public static string[] GetNoTerminals(string[] gramarGp){
			var totalTerminals = new List<string> ();
			foreach(var production in gramarGp){
				var terminals = Regex.Matches(production, _noTerminalPattern).Cast<Match>().Select(m => m.Value).Distinct();
				totalTerminals = totalTerminals.Union(terminals).ToList();
			}
			return totalTerminals.ToArray();
		}

		public static string productionPattern(){
			string ret ="";
			ret = string.Format("{0} *{1}({0}|{2}| )+({3}({0}|{2}| )+)+", 
				_noTerminalPattern, _produceElement.ToMeta(), _terminalPattern, _orElement.ToMeta());   

			return ret;
		}
		public static string[] GetTokens(string stringInput)
		{          

			string pattern = string.Format("{0}|{1}|{2}|{3}", _noTerminalPattern, _terminalPattern, _orElement.ToMeta(), _produceElement);
			var elements = Regex.Matches(stringInput, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();

			return elements;
		}

		public static string[] GetProductions(string gramar)
		{
			var production = new List<string>();
			var array = gramar.Split('\n');
			foreach (var prod in array)
			{               
				string pattern = string.Format("{0}|{1}|{2}|{3}", _noTerminalPattern, _terminalPattern, _orElement.ToMeta(), _produceElement.ToMeta());
				var elements = Regex.Matches(prod, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();
				var prods = GetSubProduction(elements);
				production.AddRange(prods);
			}            
			return production.ToArray<string>();
		}

		public static string RetLastElement(string production)
		{
			string pattern = string.Format("{0}|{1}|{2}|\\$", _noTerminalPattern, _terminalPattern, ",");
			var elements = Regex.Matches(production,pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();
			return elements.Last().Trim() ;            
		}

		public static string Normalization(string[] dato)
		{
			string ret = "";            

			foreach (var linea in dato)
			{
				string patern = string.Format("{0}|{1}|{2}|{3}", _noTerminalPattern, _terminalPattern, _orElement.ToMeta(), _produceElement.ToMeta());
				var elements = Regex.Matches(linea,patern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();
				foreach (var elemnt in elements)                 
					ret = ret + elemnt + " ";                                 
				ret = ret.Trim();
				ret = ret + "\n";
			}
			return ret;
		}

		public static string Increases(string gramar)
		{
			string aum = "";

			var produ = Regex.Match(gramar, _noTerminalPattern);
			var prodfirst = produ.ToString().Replace(">", "'>");

			aum = prodfirst + " " + _produceElement + " " + produ;
			//Queda en Formato <S'> -> <S>
			return aum;
		}

		public static string[] GetSubProduction(string[] elements)
		{
			if (elements == null || !elements.Any())
				return elements;

			List<string> retArrray = new List<string>();
			var r = new Regex(_noTerminalPattern);


			if (!r.IsMatch(elements[0]))
			{
				throw new Exception("Error DE Inicio en Produccion" + elements[0]);
			}

			var first = elements[0];
			var second = elements[1];
			var production = "";
			int inc = 0;
			foreach(var element in elements)
			{
				if (element.Contains(_orElement))
				{
					retArrray.Add(production);
					production = "";
					inc++;
				}
				else
				{
					if (inc == 0)  //si no a entrado ninguna vez
						production = production + element +" ";
					else
					{                      //si ya entro una vez                      
						production = first + " " + second + " " + element +" ";
						inc = 0;
					}   
				}                 
			}
			retArrray.Add(production);
			return retArrray.ToArray();
		}

		public static int GetIndexX(string cadena, string[] tokenSearch)
		{
			for(int i=0; i < tokenSearch.Count(); i++)
			{
				if (tokenSearch[i] == cadena)
					return i;
			}
			return -1;
		}

		public static int GetIndexY(string cadena)
		{            
			string pattern = string.Format("{0}", "[0-9]+");
			var elements = Regex.Matches(cadena, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();            

			return  Convert.ToInt32(elements.Last());
		}

		public static string GetProduction(string[] gramarGp, int ind)
		{
			for (int i = 0; i < gramarGp.Count(); i++)
				if (i == ind)
					return gramarGp[i].ToString();
			return "";
		}

		public static int GetElementosDelete(string cadena, ref string pater)
		{
			int retLength = 0;
			bool band = false;
			string pattern = string.Format("{0}|{1}|{2}|{3}", _noTerminalPattern, _terminalPattern, _orElement.ToMeta(), _produceElement.ToMeta());
			var elements = Regex.Matches(cadena, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();

			foreach (var elemen in elements)
			{
				if (band)
					retLength++;
				if (elemen == _produceElement)                
					band = true;
				if (!band)
					pater = elemen;
			}
			return retLength;
		}

		public static string GetAlpha(string production)
		{
			string retElements = "";
			bool band = false;

			string pattern = string.Format("{0}|{1}|{2}|{3}", _noTerminalPattern, _terminalPattern, _produceElement.ToMeta(), ".");
			var elements = Regex.Matches(production, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();

			foreach (var pat in elements)
			{
				if (band)
				{
					if (pat == ".")                    
						return retElements.Trim();
					if(pat == "")
						retElements = retElements + " ";
					retElements = retElements + pat;
				}
				if (pat == _produceElement)
					band = true;
			}
			return retElements.Trim();
		}

		public static bool VerificateTokens(string[] tokens, string cadena)
		{
			bool band = true;        
			if (cadena != "")
			{
				string pattern = string.Format("{0}|{1}|{2}|{3}", 
					_noTerminalPattern, _terminalPattern, _orElement.ToMeta(), _produceElement.ToMeta());
				var elements = Regex.Matches(cadena, pattern).Cast<Match>().Select(m => m.Value.Trim()).ToArray();

				foreach (var element in elements)
				{
					bool bandVer = false;
					for (int i = 0; i < tokens.Count(); i++)
					{                        
						if (tokens[i] == element)
							bandVer = true;
					}
					if (bandVer == false)
						return false;
				}
			}
			return band;
		}

		public static string FirstProduction(string cadena)
		{
			// reordena para separar produccion
			var a = GetElements (cadena)[0];
			var gama = GetGama (cadena)[0];
			cadena = string.Format("{0} {1} .{2}, $", a, _produceElement, gama);
			return cadena;
		}
	}
}

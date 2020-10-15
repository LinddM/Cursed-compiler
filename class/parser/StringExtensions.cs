using System;

namespace Cursed_compiler
{
	public static class StringExtensions
	{
		public static string ToMeta(this string value)
		{
			var regexOperators = new[] { "-", "+", ".", "(", ")", "|", "?", "*" };
			foreach (var regexOp in regexOperators){
				value = value.Replace(regexOp, @"\" + regexOp);
			}
			return value;
		}
	}
}
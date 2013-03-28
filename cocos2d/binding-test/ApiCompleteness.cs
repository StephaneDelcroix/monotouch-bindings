//
// ApiCompleteness.cs
//
// Author:
//       Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2013 S. Delcroix
//
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using MonoTouch.Foundation;

using NUnit.Framework;


namespace TouchUnit.Bindings
{
	public class ApiCompleteness : ApiBaseTest
	{
		protected class Selector {
			public string Class;
			public string Category;
			public string Sel;
		}

		public ApiCompleteness ()
		{
		}

		protected virtual bool Skip (Type type)
		{
			return false;
		}

		protected virtual bool SkipCategory (string category)
		{
			return false;
		}

		[Test]
		public void SelectorIsBound ()
		{
			int n=0;
			int errors = 0;
			foreach (var sel in Selectors ()) {
				var match = false;
				
				if (SkipCategory (sel.Category))
					continue;
				
				if (LogProgress)
					Console.WriteLine ("{0}. {1}{2}{3}{4} {5}", 
					                   n, 
					                   sel.Class, 
					                   string.IsNullOrEmpty(sel.Category) ? "" : "(",
					                   sel.Category,
					                   string.IsNullOrEmpty(sel.Category) ? "" : ")",
					                   sel.Sel);
				
				
				
				foreach (Type t in Assembly.GetTypes ()) {
					
					if (t.IsNested || !NSObjectType.IsAssignableFrom (t))
						continue;
					
					if (Skip (t))
						continue;
					
					if (t.Name != sel.Class)
						continue;
					
					
					foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
					foreach (var name in m.GetCustomAttributes (true).Where (ca => ca is ExportAttribute).Select(ca => (ca as ExportAttribute).Selector)) {
						if (string.Compare (name, sel.Sel) == 0)
							match = true;
					}					
					
					foreach (var m in t.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
					foreach (var name in m.GetCustomAttributes (true).Where (ca => ca is ExportAttribute).Select(ca => (ca as ExportAttribute).Selector)) {
						if (string.Compare (name, sel.Sel) == 0)
							match = true;
					}					
					
					if (match)
						break;
				}
				
				if (!ContinueOnFailure)
					Assert.IsTrue (match);
				else if (!match) {
					Console.WriteLine ("[FAIL] selector {0} not exported by any method or constructor in {1}", sel.Sel, sel.Class);
					errors ++;
				}
				
				
				n++;
			}
			Assert.AreEqual (0, errors, "{0} errors found in {1} signatures validated", errors, n);
		}

		protected virtual IEnumerable<Selector> Selectors ()
		{
			yield break;
		}
	}
}
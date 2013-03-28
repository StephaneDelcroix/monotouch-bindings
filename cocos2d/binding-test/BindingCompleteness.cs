//
// BindingCompleteness.cs
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
using System.Text.RegularExpressions;

using MonoTouch.Foundation;

using NUnit.Framework;
using TouchUnit.Bindings;

using MonoTouch.Cocos2D;

namespace Cocos2D.Bindings
{
	[TestFixture]
	public class BindingCompleteness : ApiCompleteness
	{
		public BindingCompleteness ()
		{
			LogProgress = true;

			ContinueOnFailure = true;
		}

		protected override System.Reflection.Assembly Assembly {
				get { return typeof (CCAccelAmplitude).Assembly; }
		}

		protected override bool SkipCategory (string category)
		{
			return (new [] {"Deprecated"}).Contains (category);
		}

		protected override IEnumerable<Selector> Selectors ()
		{
			var regex = new Regex (@"^[+-]\[([\w]+)(\(\w+\))? ([\w:]+)].*$");
			using (var reader = new System.IO.StreamReader("libcocos2d-i386.selectors")) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					var match = regex.Match (line);
					if (!match.Success)
						continue;

					var klass = match.Groups[1].Value;
					var category = match.Groups[2].Value;
					if (!string.IsNullOrEmpty (category))
						category = category.Substring (1, category.Length -2);
					var selector = match.Groups[3].Value;

					yield return new Selector {Class = klass, Category = category, Sel = selector};
				}
			}
		}
	}
}
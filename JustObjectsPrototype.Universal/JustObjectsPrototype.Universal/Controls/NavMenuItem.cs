﻿using System;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Controls
{
	/// <summary>
	/// Data to represent an item in the nav menu.
	/// </summary>
	public class NavMenuItem
	{
		public string Label { get; set; }
		public Symbol Symbol { get; set; }
		public char SymbolAsChar
		{
			get
			{
				return (char)this.Symbol;
			}
		}

		public Type DestinationPage { get; set; }
		public object Arguments { get; set; }
	}
}

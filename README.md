JOP.universal - Just Objects Prototype for Windows Universal Platform
=====================================================================
Intuitive super-fast prototyping with skills you already have. Create only your domain objects and get a prototype shell for free.

[![Build status](https://ci.appveyor.com/api/projects/status/tg27yvopapssdquc?svg=true)](https://ci.appveyor.com/project/halllo/jop-universal)
[![Version](https://img.shields.io/nuget/v/JOP.universal.svg)](https://www.nuget.org/packages/JOP.universal/)

How To Use
----------
Just create a new Blank App (Universal Windows) and add POCO classes that implement your domain, like in the example below.
```csharp
public enum Aktenstatus { Potenziell, Angenommen, Abgelehnt }

[JOP.Icon(Symbol.Folder), JOP.Title("Akten")]
public class Akte
{
	public string Name { get; set; }
	public Aktenstatus Status { get; set; }
	public Kunde Mandant { get; set; }
	public DateTime Datum { get; set; }

	[JOP.CustomView("YellowBackgroundTextInput")]
	public string Bemerkungen { get; set; }

	[JOP.Icon(Symbol.Document)]
	public Dokument Rechnug_Schreiben(string inhalt)
	{
		return new Dokument { Adressat = Mandant, Inhalt = inhalt };
	}

	[JOP.Icon(Symbol.Add)]
	public static void Neu(ObservableCollection<Akte> akten)
	{
		akten.Add(new Akte { Name = "Neue Akte " + (akten.Count + 1), Datum = DateTime.Now });
	}

	[JOP.Icon(Symbol.Delete)]
	public void Löschen(ObservableCollection<Akte> akten)
	{
		akten.Remove(this);
	}
}

[JOP.Icon(Symbol.Contact), JOP.Title("Kunden")]
public class Kunde
{
	public string Vorname { get; set; }
	public string Nachname { get; set; }

	public override string ToString()
	{
		return (Vorname + " " + Nachname).Trim();
	}
}

[JOP.Icon(Symbol.Document)]
public class Dokument
{
	public Kunde Adressat { get; set; }
	public string Inhalt { get; set; }

	[JOP.Icon(Symbol.Remove)]
	public void Löschen(ObservableCollection<Dokument> dokumente)
	{
		dokumente.Remove(this);
	}
}
```
Now that you modelled your prototype domain, "Install-Package JOP.universal" and show the prototype UI using a fluent and natural language like API in the App.xaml.cs.OnLaunched method.
```csharp
protected override void OnLaunched(LaunchActivatedEventArgs e)
{
	var objects = new ObservableCollection<object>
	{
		new Kunde { Vorname = "Manuel", Nachname = "Naujoks"},

		new Akte { Name = "Erstes Projekt", Datum = DateTime.Now },
		new Akte { Name = "Große Akte" },
		new Akte { Name = "Sonstiges" },
	};

	Show.Prototype(With.These(objects));
}
```
This gets you an UI like in the screenshot below, where you can 'play' with your object model by creating and deleting instances and invoke methods of your types. Icons, titles and custom view elements can be added by attributing your classes.
![Screenshot](https://raw.githubusercontent.com/halllo/JOP.universal/master/screenshot.png)

Happy prototyping!
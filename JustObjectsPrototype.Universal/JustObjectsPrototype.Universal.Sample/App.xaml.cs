using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Sample
{
	sealed partial class App : Application
	{
		public App()
		{
			this.InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			var objects = new ObservableCollection<object>
			{
				new Akte { Name = "Erstes Projekt", Datum = DateTime.Now },
				new Akte { Name = "Große Akte" },
				new Akte { Name = "Sonstiges" },
				new Kunde { Vorname = "Manuel", Nachname = "Naujoks"},
			};

			Show.Prototype(With.These(objects).AndOpen<Akte>());
		}
	}



























	public enum Aktenstatus
	{
		Potenziell, Angenommen, Abgelehnt
	}

	[JOP.Icon(Symbol.Folder), JOP.Title("Akten")]
	public class Akte
	{
		public string Name { get; set; }
		public Aktenstatus Status { get; set; }
		public Kunde Mandant { get; set; }
		public DateTime Datum { get; set; }

		[JOP.CustomView("YellowBackgroundTextInput")]
		public string Bemerkungen { get; set; }

		[JOP.Icon(Symbol.Document), JOP.JumpToResult()]
		public Dokument Rechnug_Schreiben([JOP.CustomView("YellowBackgroundTextInput")]string inhalt = "neuer Dokumentinhalt")
		{
			return new Dokument { Adressat = Mandant, Inhalt = inhalt };
		}

		[JOP.Icon(Symbol.Document), JOP.JumpToResult()]
		public List<Dokument> Rechnugen_Schreiben(int wie_viele = 3, [JOP.CustomView("YellowBackgroundTextInput")]string inhalt = "neuer Dokumentinhalt")
		{
			return Enumerable.Range(1, wie_viele).Select(i => new Dokument { Adressat = Mandant, Inhalt = inhalt + i }).ToList();
		}

		[JOP.Icon(Symbol.Add)]
		public static void Neu(ObservableCollection<Akte> akten)
		{
			akten.Add(new Akte { Name = "Neue Akte " + (akten.Count + 1), Datum = DateTime.Now });
		}

		[JOP.Title("Löschen"), JOP.Icon(Symbol.Delete)]
		public void Loeschen(ObservableCollection<Akte> akten)
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

		[JOP.JumpToResult]
		public async Task<Kunde> Say_Hello(string name)
		{
			await new MessageDialog("Hello " + name).ShowAsync();
			return new Kunde { Vorname = name, Nachname = DateTime.Now.Ticks + "" };
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
}

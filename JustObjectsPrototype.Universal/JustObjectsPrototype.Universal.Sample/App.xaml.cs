using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Activation;
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

			Show.Prototype(With.These(objects));
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
}

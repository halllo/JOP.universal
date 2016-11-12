using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
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
			Suspending += App_Suspending;
		}

		private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			Prototype.Remember();
		}

		protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
			Prototype = Show.Prototype(
				With.Remembered(Enumerable.Concat<object>(
					new[] { new Kunde { Vorname = "Manuel", Nachname = "Naujoks" } },
					Einstellungen.Alle
				))
				.AndViewOf<Akte>()
				.AndViewOf<Kunde>()
				.AndViewOf<Dokument>()
				.AndViewOf<Einstellungen>()
				.AndOpen<Akte>());
		}

		public static Prototype Prototype;
	}
























	public enum Aktenstatus
	{
		Potenziell, Angenommen, Abgelehnt
	}

	[JOP.Icon(Symbol.Folder), JOP.Title("Akten"), JOP.CustomView("AkteListItem")]
	public class Akte
	{
		[JOP.Editor(hide: true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[JOP.Title("Aktenname")]
		public string Name { get; set; }
		public Aktenstatus Status { get; set; }
		public Kunde Mandant { get; set; }
		public DateTime Datum { get; set; }

		[JOP.CustomView("YellowBackgroundTextInput")]
		public string Bemerkungen { get; set; }

		[JOP.Icon(Symbol.Document), JOP.RequiresConfirmation, JOP.JumpsToResult()]
		public async Task<Dokument> Rechnug_Schreiben(Kunde mandant, [JOP.CustomView("YellowBackgroundTextInput")]string inhalt = "neuer Dokumentinhalt")
		{
			var dokument = new Dokument { Adressat = mandant ?? Mandant, Inhalt = inhalt };
			return dokument;
		}

		[JOP.Icon(Symbol.Document), JOP.RequiresConfirmation, JOP.JumpsToResult()]
		public async Task<List<Dokument>> Rechnugen_Schreiben([JOP.Title("wie viele?")]int wie_viele = 3, [JOP.CustomView("YellowBackgroundTextInput")]string inhalt = "neuer Dokumentinhalt")
		{
			var dokumente = Enumerable.Range(1, wie_viele).Select(i => new Dokument { Adressat = Mandant, Inhalt = inhalt + i }).ToList();
			return dokumente;
		}

		[JOP.Icon(Symbol.Add)]
		public static async void Neu(ObservableCollection<Akte> akten)
		{
			var akte = new Akte { Name = "Neue Akte " + (akten.Count + 1), Datum = DateTime.Now };
			akten.Add(akte);
		}

		[JOP.Title("Löschen"), JOP.Icon(Symbol.Delete), JOP.RequiresConfirmation]
		public async void Loeschen(ObservableCollection<Akte> akten)
		{
			akten.Remove(this);
		}

		public void Change()
		{
			foreach (var item in App.Prototype.Repository.OfType<Akte>())
			{
				item.Name += "!";
			}
		}

		[JOP.WithProgressBar]
		public async Task Warte3Sekunden()
		{
			await Task.Delay(3000);
			await Show.Message("3 Sekunden später.");
		}
	}

	[JOP.Icon(Symbol.Contact), JOP.Title("Kunden")]
	public class Kunde
	{
		[JOP.Editor(hide: true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Vorname { get; set; }
		public string Nachname { get; set; }

		public override string ToString()
		{
			return (Vorname + " " + Nachname).Trim();
		}

		[JOP.JumpsToResult]
		public async Task<Kunde> Say_Hello(string name)
		{
			await Show.Message("Hello " + name);
			var kunde = new Kunde { Vorname = name, Nachname = DateTime.Now.Ticks + "" };
			return kunde;
		}

		[JOP.Icon(Symbol.NewFolder), JOP.JumpsToResult]
		public async static Task<Kunde> Neu(string vorname, string nachname)
		{
			var kunde = new Kunde { Vorname = vorname, Nachname = nachname };
			return kunde;
		}

		public void Ersten_Löschen()
		{
			var erster = App.Prototype.Repository.OfType<Kunde>().First();
			App.Prototype.Repository.Remove(erster);
		}
	}

	[JOP.Icon(Symbol.Document)]
	public class Dokument
	{
		[JOP.Editor(hide: true)]
		public Guid Id { get; set; } = Guid.NewGuid();

		public Kunde Adressat { get; set; }
		public string Inhalt { get; set; }

		[JOP.Icon(Symbol.Remove)]
		public async void Löschen(ObservableCollection<Dokument> dokumente)
		{
			dokumente.Remove(this);
		}
	}

	[DataContract, JOP.Icon(Symbol.Setting)]
	public abstract class Einstellungen
	{
		public static Einstellungen[] Alle => new Einstellungen[] { new AllgemeineEinstellungen(), new SpeicherEinstellungen() };

		[DataContract]
		public class SpeicherEinstellungen : Einstellungen
		{
			public override string ToString() => "Speicher";

			public IEnumerable<object> Gespeicherte_Objekte => App.Prototype.Repository.Where(o => !(o is Einstellungen));

			[JOP.Icon(Symbol.Delete), JOP.RequiresConfirmation]
			public async Task Alles_Löschen()
			{
				App.Prototype.Forget();
				App.Prototype.Repository.Clear();
			}

			[JOP.Icon(Symbol.GoToStart), JOP.JumpsToResult]
			public object Gehe_zu_zweitem_Kunden()
			{
				var zweiterKunde = App.Prototype.Repository.OfType<Kunde>().Skip(1).FirstOrDefault();
				return zweiterKunde;
			}
		}

		[DataContract]
		public class AllgemeineEinstellungen : Einstellungen
		{
			public override string ToString() => "Allgemein";

			[DataMember]
			public string Value1 { get; set; }
			[DataMember]
			public string Value2 { get; set; }
		}
	}
}


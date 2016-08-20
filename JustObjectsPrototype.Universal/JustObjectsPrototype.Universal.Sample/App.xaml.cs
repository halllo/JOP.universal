using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		}

		protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
			if ((await Kundenspeicher.All()).Any() == false)
			{
				await Kundenspeicher.Save(new Kunde { Vorname = "Manuel", Nachname = "Naujoks" });
			}

			//await Aktenspeicher.DeleteAll();
			//await Kundenspeicher.DeleteAll();
			//await Dokumentspeicher.DeleteAll();

			Prototype = Show.Prototype(
				With.These(
					await Aktenspeicher.All(),
					await Kundenspeicher.All(),
					await Dokumentspeicher.All(),
					Einstellungen.Alle)
				.OnChanged<Akte>(async a => await Aktenspeicher.SaveOrUpdate(a))
				.OnChanged<Kunde>(async k => await Kundenspeicher.SaveOrUpdate(k))
				.OnChanged<Dokument>(async d => await Dokumentspeicher.SaveOrUpdate(d))
				.AndOpen<Akte>());
		}

		public static readonly Store<Akte> Aktenspeicher = new Store<Akte>(a => a.Id.ToString());
		public static readonly Store<Kunde> Kundenspeicher = new Store<Kunde>(k => k.Id.ToString());
		public static readonly Store<Dokument> Dokumentspeicher = new Store<Dokument>(d => d.Id.ToString());
		public static Prototype Prototype;
	}
























	public enum Aktenstatus
	{
		Potenziell, Angenommen, Abgelehnt
	}

	[JOP.Icon(Symbol.Folder), JOP.Title("Akten")]
	public class Akte
	{
		[JOP.Editor(hide: true)]
		public Guid Id { get; set; } = Guid.NewGuid();

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
			await App.Dokumentspeicher.Save(dokument);
			return dokument;
		}

		[JOP.Icon(Symbol.Document), JOP.RequiresConfirmation, JOP.JumpsToResult()]
		public async Task<List<Dokument>> Rechnugen_Schreiben(int wie_viele = 3, [JOP.CustomView("YellowBackgroundTextInput")]string inhalt = "neuer Dokumentinhalt")
		{
			var dokumente = Enumerable.Range(1, wie_viele).Select(i => new Dokument { Adressat = Mandant, Inhalt = inhalt + i }).ToList();
			foreach (var dokument in dokumente)
			{
				await App.Dokumentspeicher.Save(dokument);
			}
			return dokumente;
		}

		[JOP.Icon(Symbol.Add)]
		public static async void Neu(ObservableCollection<Akte> akten)
		{
			var akte = new Akte { Name = "Neue Akte " + (akten.Count + 1), Datum = DateTime.Now };
			await App.Aktenspeicher.Save(akte);
			akten.Add(akte);
		}

		[JOP.Title("Löschen"), JOP.Icon(Symbol.Delete), JOP.RequiresConfirmation]
		public async void Loeschen(ObservableCollection<Akte> akten)
		{
			await App.Aktenspeicher.Delete(this);
			akten.Remove(this);
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
			await App.Kundenspeicher.Save(kunde);
			return kunde;
		}

		[JOP.JumpsToResult]
		public async static Task<Kunde> Neu(string vorname, string nachname)
		{
			var kunde = new Kunde { Vorname = vorname, Nachname = nachname };
			await App.Kundenspeicher.Save(kunde);
			return kunde;
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
			await App.Dokumentspeicher.Delete(this);
			dokumente.Remove(this);
		}
	}

	[JOP.Icon(Symbol.Setting)]
	public abstract class Einstellungen
	{
		public static Einstellungen[] Alle => new Einstellungen[] { new AllgemeineEinstellungen(), new SpeicherEinstellungen() };

		class SpeicherEinstellungen : Einstellungen
		{
			public override string ToString() => "Speicher";

			public IEnumerable<object> Gespeicherte_Objekte => App.Prototype.Repository.Where(o => !(o is Einstellungen));

			[JOP.Icon(Symbol.Delete)]
			public async Task Alles_Löschen()
			{
				await App.Aktenspeicher.DeleteAll();
				await App.Kundenspeicher.DeleteAll();
				await App.Dokumentspeicher.DeleteAll();

				var allesAußerEinstellungen = App.Prototype.Repository.Where(o => !(o is Einstellungen)).ToList();
				allesAußerEinstellungen.ForEach(o => App.Prototype.Repository.Remove(o));
			}
		}

		class AllgemeineEinstellungen : Einstellungen
		{
			public override string ToString() => "Allgemein";

			public string Value1 { get; set; }

			public string Value2 { get; set; }
		}
	}
}


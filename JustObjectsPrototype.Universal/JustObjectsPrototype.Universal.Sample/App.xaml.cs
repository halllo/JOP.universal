using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
				new Akte { Name = "Akte 1", Datum = DateTime.Now },
				new Akte { Name = "Akte 2" },
				new Akte { Name = "Akte 3" },
				new Kunde {
					Vorname = "Manuel",
					Nachname = "Naujoks",
					Freunde = new List<Kunde> {
						new Kunde { Vorname = "Tester1", Nachname ="Test1"},
						new Kunde { Vorname = "Tester2", Nachname ="Test2"},
					}
				},
				new Kunde { Vorname = "Max", Nachname = "Musterman" },
				new Kunde { Vorname = "King", Nachname = "Kong" },
				new Dokument { Inhalt = "dokument 1" },
				new Dokument { Inhalt = "dokument 2" },
			};

			Show.Prototype(With.These(objects)
				.OnChanged<Akte>(a => a.ID++)
				.OnChanged<Kunde>(k => k.Geändert = true)
				.OnChanged<Dokument>(d => d.Geändert = true)
				);
		}
	}



























	public enum Aktenstatus
	{
		Potenziell, Angenommen, Abgelehnt
	}

	[JOP.Icon(Symbol.Folder)]
	public class Akte
	{
		public Akte()
		{
		}

		public int ID { get; set; }

		public string Name { get; set; }
		public Aktenstatus Status { get; set; }
		public Kunde Mandant { get; set; }
		public DateTime Datum { get; set; }

		[JOP.Icon(Symbol.Highlight)]
		public void Highlighten()
		{
			Name += "!";
		}

		[JOP.Icon(Symbol.Document)]
		public Dokument Schreiben(string inhalt)
		{
			return new Dokument { Adressat = Mandant, Inhalt = inhalt };
		}

		[JOP.Icon(Symbol.Delete)]
		public void Alle_Dokumente_Löschen(ObservableCollection<Dokument> dokumente)
		{
			dokumente.Clear();
		}

		[JOP.Icon(Symbol.Delete)]
		public void Erstes_Dokumente_Löschen(ObservableCollection<Dokument> dokumente)
		{
			dokumente.Remove(dokumente.First());
		}

		[JOP.Icon(Symbol.Delete)]
		public void Alle_Akten_Löschen(ObservableCollection<Akte> akten)
		{
			akten.Clear();
		}

		[JOP.Icon(Symbol.Delete)]
		public static void Alle_Löschen(ObservableCollection<Akte> akten)
		{
			akten.Clear();
		}

		[JOP.Icon(Symbol.Highlight)]
		public static void Highlight_Second(ObservableCollection<Akte> akten)
		{
			akten[1].Highlighten();
		}

		[JOP.Icon(Symbol.Add)]
		public static void Neu_Erzeugen(ObservableCollection<Akte> akten, int wieviele)
		{
			for (int i = 0; i < wieviele; i++)
			{
				akten.Add(new Akte { Name = "Neue Akte " + (i + 1), Datum = DateTime.Now });
			}
		}
	}

	[JOP.Icon(Symbol.Contact)]
	public class Kunde
	{
		[JOP.Editor(@readonly: true)]
		public bool Geändert { get; set; }

		public string Vorname { get; set; }
		public string Nachname { get; set; }

		public List<Kunde> Freunde { get; set; }
		public string Freunde_Info { get { return string.Join(", ", Freunde?.Select(f => f != null ? f.ToString() : "<NULL>") ?? new List<string>()); } }

		public List<string> Spitznamen { get; set; }
		public string Spitznamen_Info { get { return string.Join(", ", Spitznamen ?? new List<string>()); } }

		public List<int> Ints { get; set; }
		public string Ints_Info { get { return string.Join(", ", Ints ?? new List<int>()); } }

		public override string ToString()
		{
			return (Vorname + " " + Nachname).Trim();
		}

		public static void Alle_Ungeändert(ObservableCollection<Kunde> kunden)
		{
			foreach (var kunde in kunden)
			{
				kunde.Geändert = false;
			}
		}

		[JOP.Icon(Symbol.AddFriend)]
		public void Neuer_Freund(Kunde freund)
		{
			if (Freunde == null)
			{
				Freunde = new List<Kunde>();
			}

			Freunde.Add(freund);
		}
	}

	[JOP.Icon(Symbol.Document)]
	public class Dokument
	{
		[JOP.Editor(@readonly: true)]
		public bool Geändert { get; set; }

		public Kunde Adressat { get; set; }
		public string Inhalt { get; set; }

		[JOP.Editor(hide: true)]
		public string Ersteller { get; set; }

		[JOP.Icon(Symbol.Remove)]
		public void Löschen(ObservableCollection<Dokument> dokumente)
		{
			dokumente.Remove(this);
		}
	}
}

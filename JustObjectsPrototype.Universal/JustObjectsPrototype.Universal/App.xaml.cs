using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			//#if DEBUG
			//			if (System.Diagnostics.Debugger.IsAttached)
			//			{
			//				this.DebugSettings.EnableFrameRateCounter = true;
			//			}
			//#endif
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (e.PrelaunchActivated == false)
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter

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
					};
					JOP.JopViewModel.Instance.Value.ShowMethodInvocationDialog = ps =>
					{
						(Window.Current.Content as Frame).Navigate(typeof(JOP.MethodInvocationPage), null, new DrillInNavigationTransitionInfo());
					};
					JOP.JopViewModel.Instance.Value.Init(objects);

					rootFrame.Navigate(typeof(Shell.MasterDetailPage), e.Arguments);
				}
				// Ensure the current window is active
				Window.Current.Activate();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}
	}











































































	[JOP.Icon(Symbol.Folder)]
	public class Akte
	{
		public Akte()
		{
		}
		public int ID { get; set; }
		public string Name { get; set; }

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
		public static void Alle_Löschen(ObservableCollection<Akte> akten)
		{
			akten.Clear();
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
		public bool Geändert { get; set; }
		public string Vorname { get; set; }
		public string Nachname { get; set; }
		public List<Kunde> Freunde { get; set; }
		public List<string> Spitznamen { get; set; }

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
		public Kunde Adressat { get; set; }
		public string Inhalt { get; set; }

		[JOP.Icon(Symbol.Remove)]
		public void Löschen(ObservableCollection<Dokument> dokumente)
		{
			dokumente.Remove(this);
		}
	}















}

﻿using JustObjectsPrototype.Universal.JOP;
using JustObjectsPrototype.Universal.JOP.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace JustObjectsPrototype.Universal
{
	public static class Show
	{
		public static async Task Message(string text)
		{
			await new MessageDialog(text).ShowAsync();
		}

		public static async Task Message(string text, Action onYes)
		{
			var dialog = new MessageDialog(text);
			dialog.Options = MessageDialogOptions.None;
			dialog.Commands.Add(new UICommand("Yes", cmd => onYes()));
			dialog.Commands.Add(new UICommand("No", cmd => { }));
			dialog.CancelCommandIndex = 1;
			dialog.DefaultCommandIndex = 0;
			await dialog.ShowAsync();
		}

		public static async Task Message(string text, Func<Task> onYes)
		{
			var tcs = new TaskCompletionSource<bool>();

			var dialog = new MessageDialog(text);
			dialog.Options = MessageDialogOptions.None;
			dialog.Commands.Add(new UICommand("Yes", async cmd =>
			{
				await onYes();
				tcs.SetResult(true);
			}));
			dialog.Commands.Add(new UICommand("No", cmd => { tcs.SetResult(false); }));
			dialog.CancelCommandIndex = 1;
			dialog.DefaultCommandIndex = 0;
			await dialog.ShowAsync();

			await tcs.Task;
		}

		public static Prototype Prototype(PrototypeBuilder with)
		{
			var objects = with.Repository;
			if (objects.Any(o => o == null)) throw new ArgumentNullException();

			var types = with.DisplayedTypes;
			if (types.Any(o => o == null)) throw new ArgumentNullException();

			Frame rootFrame = Window.Current.Content as Frame;

			if (rootFrame == null)
			{
				rootFrame = new Frame();
				rootFrame.NavigationFailed += (sender, e) => { throw new Exception("Failed to load Page " + e.SourcePageType.FullName); };

				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				JopViewModel.Instance.Value.ShowMethodInvocationDialog = ps =>
				{
					rootFrame.Navigate(typeof(MethodInvocationPage), null, new DrillInNavigationTransitionInfo());
				};
				JopViewModel.Instance.Value.Init(objects, types, with.ChangedEvents);
				with.InternalSetup.Invoke(JopViewModel.Instance.Value);

				rootFrame.Navigate(typeof(Shell.MasterDetailPage));
			}
			Window.Current.Activate();

			return new Prototype { Repository = objects };
		}
	}

	public class Prototype
	{
		internal Prototype()
		{
		}

		public ICollection<object> Repository { get; internal set; }

		public void Remember()
		{
			var objects = Repository;
			var types = objects.Select(o => o.GetType().AssemblyQualifiedName).Distinct().ToList();
			var typesString = string.Join(Environment.NewLine, types);

			var speicher = new Store("objects");
			speicher.SaveOrUpdateSync("types", typesString);
			speicher.File<ICollection<object>>("instances").SaveOrUpdateSync(objects);
		}

		public void Forget()
		{
			var speicher = new Store("objects");
			speicher.DeleteAllSync();
		}
	}

	public static class With
	{
		public static PrototypeBuilder These(ICollection<object> objects)
		{
			return new PrototypeBuilder { Repository = objects };
		}

		public static PrototypeBuilder These(params IEnumerable<object>[] objects)
		{
			return new PrototypeBuilder { Repository = new ObservableCollection<object>(objects.SelectMany(l => l)) };
		}

		public static PrototypeBuilder Remembered(IEnumerable<object> defaults = null)
		{
			var speicher = new Store("objects");
			var storedObjects = speicher.File<ICollection<object>>("instances");
			IEnumerable<object> objs = null;
			try
			{
				var typesString = speicher.GetSync("types");
				var typeStrings = typesString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				var types = typeStrings.Select(s => Type.GetType(s)).ToList();
				storedObjects.KnownTypes = types;
				objs = storedObjects.ReadSync();
			}
			catch (Exception)
			{
				speicher.DeleteAllSync();
			}

			if (objs == null || !objs.Any())
			{
				objs = defaults ?? Enumerable.Empty<object>();
			}

			return These(new ObservableCollection<object>(objs));
		}
	}

	public class PrototypeBuilder
	{
		internal PrototypeBuilder()
		{
			Repository = new List<object>();
			DisplayedTypes = new List<Type>();
			ChangedEvents = new Dictionary<Type, Action<ObjectChangedEventArgs>>();
			InternalSetup = vm => { };
		}

		internal ICollection<object> Repository { get; set; }
		internal List<Type> DisplayedTypes { get; set; }
		internal Dictionary<Type, Action<ObjectChangedEventArgs>> ChangedEvents { get; set; }
		internal Action<JopViewModel> InternalSetup { get; set; }

		public PrototypeBuilder AndViewOf<TNext>()
		{
			return new PrototypeBuilder
			{
				Repository = Repository,
				DisplayedTypes = DisplayedTypes.Union(new[] { typeof(TNext) }).ToList(),
				ChangedEvents = ChangedEvents,
				InternalSetup = InternalSetup,
			};
		}

		public PrototypeBuilder AndOpen<T>()
		{
			return new PrototypeBuilder
			{
				Repository = Repository,
				DisplayedTypes = DisplayedTypes,
				ChangedEvents = ChangedEvents,
				InternalSetup = vm =>
				{
					InternalSetup(vm);

					var menuItemOfTyp = vm.MenuItems.Where(mi => mi.Tag.Equals(typeof(T))).FirstOrDefault();
					if (menuItemOfTyp != null)
					{
						vm.SelectedMenuItem = menuItemOfTyp;
					}
					else
					{
						vm.ObjectsOfType<T>();
						vm.SelectedMenuItem = vm.MenuItems.Where(mi => mi.Tag.Equals(typeof(T))).FirstOrDefault();
					}
				},
			};
		}

		public PrototypeBuilder OnChanged<T>(Action<T> changedCallback)
		{
			var newChangeEvents = new Dictionary<Type, Action<ObjectChangedEventArgs>>(ChangedEvents);
			newChangeEvents.Add(typeof(T), new Action<ObjectChangedEventArgs>(o => changedCallback((T)o.Object)));

			return new PrototypeBuilder
			{
				Repository = Repository,
				DisplayedTypes = DisplayedTypes,
				ChangedEvents = newChangeEvents,
				InternalSetup = InternalSetup,
			};
		}
	}
}

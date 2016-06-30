using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace JustObjectsPrototype.Universal
{
	public static class Show
	{
		public static Prototype Prototype(PrototypeBuilder with)
		{
			var objects = with.Repository;
			if (objects.Any(o => o == null)) throw new ArgumentNullException();

			JOP.JopViewModel.Instance.Value.ShowMethodInvocationDialog = ps =>
			{
				(Window.Current.Content as Frame).Navigate(typeof(JOP.MethodInvocationPage), null, new DrillInNavigationTransitionInfo());
			};
			JOP.JopViewModel.Instance.Value.Init(objects);

			(Window.Current.Content as Frame).Navigate(typeof(Shell.MasterDetailPage));

			return new Prototype { Repository = objects };
		}
	}

	public class Prototype
	{
		internal Prototype()
		{
		}

		public ICollection<object> Repository { get; internal set; }
	}

	public static class With
	{
		public static PrototypeBuilder These(ICollection<object> objects)
		{
			return new PrototypeBuilder { Repository = objects };
		}
	}

	public class PrototypeBuilder
	{
		internal PrototypeBuilder()
		{
			Repository = new List<object>();
		}

		internal ICollection<object> Repository { get; set; }
	}
}

using JustObjectsPrototype.Universal.JOP;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.ObjectModel;

namespace JustObjectsPrototype.Universal.Tests
{
	[TestClass]
	public class ObjectsTests
	{
		public class DomainObject1 { public int Id { get; set; } }
		public class DomainObject2 { public int Id { get; set; } }
















		[TestMethod]
		public void Add_To_ObjectsList_Also_Adds_To_TypeObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));

			list.Add(new DomainObject1 { Id = 1 });

			var o = (DomainObject1)(dynamic)domainObject1s[0];
			Assert.IsTrue(o.Id == 1);
		}

		[TestMethod]
		public void Add_To_ObjectsList_Also_Adds_To_TypeObjectsList_And_Add_There_Adds_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			list.Add(new DomainObject1 { Id = 1 });

			var domainObject1s = objects.OfType(typeof(DomainObject1));
			domainObject1s.Add(new ObjectProxy(new DomainObject1 { Id = 2 }));

			Assert.IsTrue(((DomainObject1)(dynamic)domainObject1s[0]).Id == 1);
			Assert.IsTrue(((DomainObject1)(dynamic)domainObject1s[1]).Id == 2);

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
			Assert.IsTrue(((DomainObject1)list[1]).Id == 2);
		}

		[TestMethod]
		public void Add_To_TypeObjectsList_Also_Adds_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));

			domainObject1s.Add(new ObjectProxy(new DomainObject1 { Id = 1 }));

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Add_Works_After_Clear()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));
			Assert.IsTrue(list.Count == 1);
			domainObject1s.Clear();
			Assert.IsTrue(list.Count == 0);


			list.Add(new DomainObject1 { Id = 2 });

			var o = (DomainObject1)(dynamic)domainObject1s[0];
			Assert.IsTrue(o.Id == 2);


			domainObject1s.Add(new ObjectProxy(new DomainObject1 { Id = 3 }));

			Assert.IsTrue(((DomainObject1)list[0]).Id == 2);
			Assert.IsTrue(((DomainObject1)list[1]).Id == 3);
		}

		[TestMethod]
		public void Clear_To_TypeObjectsList_Also_Clears_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));
			domainObject1s.Clear();

			Assert.IsTrue(list.Count == 0);
		}

		[TestMethod]
		public void Clear_To_TypeObjectsList_Only_Clears_Of_Same_Type()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });
			list.Add(new DomainObject2 { Id = 2 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));
			var domainObject2s = objects.OfType(typeof(DomainObject2));
			domainObject1s.Clear();

			Assert.IsTrue(domainObject1s.Count == 0);
			Assert.IsTrue(domainObject2s.Count == 1);
			Assert.IsTrue(list.Count == 1);
		}
















		[TestMethod]
		public void Add_To_ObjectsList_DoesNot_Also_Add_To_UnproxiedTypeObjectsList_Generic()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();

			list.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(domainObject1s.Count == 0);
		}

		[TestMethod]
		public void Add_To_ObjectsList_DoesNot_Also_Add_To_UnproxiedTypeObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));

			list.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(domainObject1s.Count == 0);
		}

		[TestMethod]
		public void Add_To_UnproxiedTypeObjectsList_Also_Adds_To_ObjectsList_Generic()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();

			domainObject1s.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Add_To_UnproxiedTypeObjectsList_Also_Adds_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));

			domainObject1s.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Clear_To_UnproxiedTypeObjectsList_Also_Clears_To_ObjectsList_Generic()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();
			domainObject1s.Clear();

			Assert.IsTrue(list.Count == 0);
		}

		[TestMethod]
		public void Clear_To_UnproxiedTypeObjectsList_Also_Clears_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });

			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));
			domainObject1s.Clear();

			Assert.IsTrue(list.Count == 0);
		}

		[TestMethod]
		public void Clear_To_UnproxiedTypeObjectsList_Only_Clears_Of_Same_Type_Generic()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });
			list.Add(new DomainObject2 { Id = 2 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();
			var domainObject2s = objects.OfType_OneWayToSourceChangePropagation<DomainObject2>();
			domainObject1s.Clear();

			Assert.IsTrue(domainObject1s.Count == 0);
			Assert.IsTrue(domainObject2s.Count == 1);
			Assert.IsTrue(list.Count == 1);
		}

		[TestMethod]
		public void Clear_To_UnproxiedTypeObjectsList_Only_Clears_Of_Same_Type()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });
			list.Add(new DomainObject2 { Id = 2 });

			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));
			var domainObject2s = (ObservableCollection<DomainObject2>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject2));
			domainObject1s.Clear();

			Assert.IsTrue(domainObject1s.Count == 0);
			Assert.IsTrue(domainObject2s.Count == 1);
			Assert.IsTrue(list.Count == 1);
		}
	}
}

JOP.universal - Just Objects Prototype for Windows Universal Platform
=====================================================================
Intuitive super fast prototyping by using skills you already know. Create only your domain objects and get a prototype shell for free.

[![Build status](https://ci.appveyor.com/api/projects/status/tg27yvopapssdquc?svg=true)](https://ci.appveyor.com/project/halllo/jop-universal)
[![Version](https://img.shields.io/nuget/v/JOP.universal.svg)](https://www.nuget.org/packages/JOP.universal/)

How To Use
----------
TODO

```csharp
protected override void OnLaunched(LaunchActivatedEventArgs e)
{
	var objects = new ObservableCollection<object>
	{
		new Kunde { Vorname = "Manuel", Nachname = "Naujoks" }
	};

	Show.Prototype(With.These(objects)
		.AndViewOf<Akte>()
		.OnChanged<Akte>(a => { a.ID++; })
		);
}
```

Happy prototyping!
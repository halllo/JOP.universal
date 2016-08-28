using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace JustObjectsPrototype.Universal
{
	public class Store
	{
		string foldername;

		public Store(string foldername)
		{
			this.foldername = foldername;
		}

		protected async Task<StorageFolder> GetFolder()
		{
			var localFolder = ApplicationData.Current.LocalFolder;
			var folderForType = await localFolder.CreateFolderAsync(foldername, CreationCollisionOption.OpenIfExists);
			return folderForType;
		}

		protected async Task Write(string filename, CreationCollisionOption options, Func<IRandomAccessStream, Task> writeAction)
		{
			var folder = await GetFolder();
			var file = await folder.CreateFileAsync(filename, options);
			using (var openedFile = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				await writeAction(openedFile);
			}
		}

		protected async Task Read(string filename, Func<IRandomAccessStream, Task> readAction)
		{
			var folder = await GetFolder();
			var file = await folder.GetFileAsync(filename);
			using (var openedFile = await file.OpenAsync(FileAccessMode.Read))
			{
				await readAction(openedFile);
			}
		}


		public async Task SaveOrUpdate(string filename, string filecontent, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			await Write(filename, options, async openedFile =>
			{
				using (var streamWriter = new StreamWriter(openedFile.AsStreamForWrite()))
				{
					await streamWriter.WriteAsync(filecontent);
					await streamWriter.FlushAsync();
				}
			});
		}

		public void SaveOrUpdateSync(string filename, string filecontent, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			AsyncHelpers.RunSync(() => SaveOrUpdate(filename, filecontent, options));
		}

		public async Task<string> Get(string filename)
		{
			string result = null;
			await Read(filename, async openedFile =>
			{
				using (var streamReader = new StreamReader(openedFile.AsStreamForRead()))
				{
					var filecontent = await streamReader.ReadToEndAsync();
					result = filecontent;
				}
			});
			return result;
		}

		public string GetSync(string filename)
		{
			return AsyncHelpers.RunSync(() => Get(filename));
		}

		public StoreFile<T> File<T>(string filename)
		{
			return new StoreFile<T>(
				readAction: async r => await Read(filename, async openedFile =>
				{
					await Task.CompletedTask; r(openedFile);
				}),
				writeAction: async (o, w) => await Write(filename, o, async openedFile =>
				{
					w(openedFile);
					await openedFile.FlushAsync();
				}));
		}

		public async Task Delete(string filename)
		{
			var folder = await GetFolder();
			var file = await folder.GetFileAsync(filename);
			await file.DeleteAsync();
		}

		public async Task DeleteAll()
		{
			var folder = await GetFolder();
			var files = await folder.GetFilesAsync();
			foreach (var file in files)
			{
				await file.DeleteAsync();
			}
		}

		public void DeleteAllSync()
		{
			AsyncHelpers.RunSync(() => DeleteAll());
		}
	}

	public class StoreFile<T>
	{
		Func<Action<IRandomAccessStream>, Task> readAction;
		Func<CreationCollisionOption, Action<IRandomAccessStream>, Task> writeAction;

		internal StoreFile(
			Func<Action<IRandomAccessStream>, Task> readAction,
			Func<CreationCollisionOption, Action<IRandomAccessStream>, Task> writeAction)
		{
			this.readAction = readAction;
			this.writeAction = writeAction;

		}

		public List<Type> KnownTypes { get; set; }

		public async Task<T> Read()
		{
			var serializer = new DataContractSerializer(typeof(T), new DataContractSerializerSettings { PreserveObjectReferences = true, KnownTypes = KnownTypes });
			T result = default(T);
			await readAction(openedFile =>
			{
				using (var streamReader = new StreamReader(openedFile.AsStreamForRead()))
				{
					result = (T)serializer.ReadObject(openedFile.AsStreamForRead());
				}
			});
			return result;
		}

		public T ReadSync()
		{
			return AsyncHelpers.RunSync(() => Read());
		}

		public async Task SaveOrUpdate(T t, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			var serializer = new DataContractSerializer(typeof(T), new DataContractSerializerSettings { PreserveObjectReferences = true, KnownTypes = KnownTypes });
			await writeAction(options, openedFile =>
			{
				serializer.WriteObject(openedFile.AsStreamForWrite(), t);
			});
		}

		public void SaveOrUpdateSync(T t, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			AsyncHelpers.RunSync(() => SaveOrUpdate(t, options));
		}
	}
}

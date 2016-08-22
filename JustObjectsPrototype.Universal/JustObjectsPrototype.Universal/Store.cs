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

		protected async Task WriteFile(string filename, CreationCollisionOption options, Func<IRandomAccessStream, Task> writeAction)
		{
			var folder = await GetFolder();
			var file = await folder.CreateFileAsync(filename, options);
			using (var openedFile = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				await writeAction(openedFile);
			}
		}

		protected async Task ReadAll(Func<IRandomAccessStream, Task> readAction)
		{
			var folder = await GetFolder();
			var files = await folder.GetFilesAsync();
			foreach (var file in files)
			{
				using (var openedFile = await file.OpenAsync(FileAccessMode.Read))
				{
					await readAction(openedFile);
				}
			}
		}


		public async Task SaveOrUpdate(string filename, string filecontent, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			await WriteFile(filename, options, async openedFile =>
			{
				using (var streamWriter = new StreamWriter(openedFile.AsStreamForWrite()))
				{
					await streamWriter.WriteAsync(filecontent);
					await streamWriter.FlushAsync();
				}
			});
		}

		public async Task<List<string>> All()
		{
			var result = new List<string>();
			await ReadAll(async openedFile =>
			{
				using (var streamReader = new StreamReader(openedFile.AsStreamForRead()))
				{
					var filecontent = await streamReader.ReadToEndAsync();
					result.Add(filecontent);
				}
			});
			return result;
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
	}


	public class Store<T> : Store
	{
		Func<T, string> filenameSelector;

		public Store(Func<T, string> filenameSelector) : base(typeof(T).FullName)
		{
			this.filenameSelector = filenameSelector;
		}

		public async Task Save(T t)
		{
			await SaveOrUpdate(t, CreationCollisionOption.FailIfExists);
		}

		public async Task SaveOrUpdate(T t, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			await WriteFile(filenameSelector(t), options, async openedFile =>
			{
				var serializer = new DataContractSerializer(typeof(T));
				serializer.WriteObject(openedFile.AsStreamForWrite(), t);
				await openedFile.FlushAsync();
			});
		}

		public async Task Delete(T t)
		{
			await Delete(filenameSelector(t));
		}

		public new async Task<List<T>> All()
		{
			var result = new List<T>();
			var serializer = new DataContractSerializer(typeof(T));
			await ReadAll(async openedFile =>
			{
				await Task.CompletedTask;
				var readObject = serializer.ReadObject(openedFile.AsStreamForRead());
				result.Add((T)readObject);
			});
			return result;
		}
	}
}

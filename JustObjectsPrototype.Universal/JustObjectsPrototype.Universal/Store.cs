using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace JustObjectsPrototype.Universal
{
	public class Store<T>
	{
		Func<T, string> filenameSelector;

		public Store(Func<T, string> filenameSelector)
		{
			this.filenameSelector = filenameSelector;
		}

		public async Task Save(T t)
		{
			await SaveOrUpdate(t, CreationCollisionOption.FailIfExists);
		}

		public async Task SaveOrUpdate(T t, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			var folderForType = await GetFolder();

			var file = await folderForType.CreateFileAsync(filenameSelector(t), options);
			using (var openedFile = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				var serializer = new DataContractSerializer(typeof(T));
				serializer.WriteObject(openedFile.AsStreamForWrite(), t);
				await openedFile.FlushAsync();
			}
		}

		public async Task Delete(T t)
		{
			var folderForType = await GetFolder();

			var file = await folderForType.GetFileAsync(filenameSelector(t));
			await file.DeleteAsync();
		}

		public async Task DeleteAll()
		{
			var folderForType = await GetFolder();

			var files = await folderForType.GetFilesAsync();
			foreach (var file in files)
			{
				await file.DeleteAsync();
			}
		}

		public async Task<List<T>> All()
		{
			var folderForType = await GetFolder();

			var files = await folderForType.GetFilesAsync();
			var result = new List<T>();
			var serializer = new DataContractSerializer(typeof(T));
			foreach (var file in files)
			{
				using (var openedFile = await file.OpenAsync(FileAccessMode.Read))
				{
					var readObject = serializer.ReadObject(openedFile.AsStreamForRead());
					result.Add((T)readObject);
				}
			}
			return result;
		}

		private static async Task<StorageFolder> GetFolder()
		{
			var localFolder = ApplicationData.Current.LocalFolder;
			var folderForType = await localFolder.CreateFolderAsync(typeof(T).FullName, CreationCollisionOption.OpenIfExists);
			return folderForType;
		}
	}
}

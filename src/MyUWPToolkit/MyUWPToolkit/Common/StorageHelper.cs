using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyUWPToolkit.Common
{
    public static class StorageHelper
    {
        //static StorageFolder localFolder;
        static object obj;

        static StorageHelper()
        {
            try
            {
                //localFolder = ApplicationData.Current.LocalFolder;
                obj = new object();
            }
            catch (Exception ex)
            {
                
            }
        }

        //判断文件是否存在
        public async static Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //读取文件内容
        public async static Task<string> ReadFileAsync(string fileName)
        {
            string content = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (Stream stream = await storageFile.OpenStreamForReadAsync())
                    {
                        byte[] result = new byte[stream.Length];
                        await stream.ReadAsync(result, 0, (int)stream.Length);
                        content = Encoding.UTF8.GetString(result, 0, result.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return content;
        }

        //读取文件数据
        public async static Task<byte[]> ReadFileDataAsync(string fileName)
        {
            byte[] data = null;
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (Stream stream = await storageFile.OpenStreamForReadAsync())
                    {
                        data = new byte[stream.Length];
                        await stream.ReadAsync(data, 0, (int)stream.Length);
                    }
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        //写文件内容
        public async static Task WriteFileAsync(string fileName, string content, bool isAppendMode = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    CreationCollisionOption fileOperationOption;
                    if (isAppendMode)//如果是日志文件，以追加方式添加
                    {
                        fileOperationOption = CreationCollisionOption.OpenIfExists;
                    }
                    else//如果是其他配置类文件，以替换方式添加
                    {
                        fileOperationOption = CreationCollisionOption.ReplaceExisting;
                    }

                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, fileOperationOption);
                    using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        long offset = stream.Seek(0, SeekOrigin.End);
                        byte[] source = Encoding.UTF8.GetBytes(content);
                        await stream.WriteAsync(source, 0, source.Length);
                    }
                    //await FileIO.WriteTextAsync(storageFile, content);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public async static Task WriteFileAsync(string fileName, byte[] data, bool isAppendMode = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    CreationCollisionOption fileOperationOption;
                    if (isAppendMode)//如果是日志文件，以追加方式添加
                    {
                        fileOperationOption = CreationCollisionOption.OpenIfExists;
                    }
                    else//如果是其他配置类文件，以替换方式添加
                    {
                        fileOperationOption = CreationCollisionOption.ReplaceExisting;
                    }

                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, fileOperationOption);
                    using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        stream.Seek(0, SeekOrigin.End);
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        //删除文件
        public async static Task DeleteFileAsync(string fileName)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    await storageFile.DeleteAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        public static async Task<ulong> GetFileSizeAsync(string fileName)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                var properties = await file.GetBasicPropertiesAsync();
                return properties.Size;
            }
            catch (Exception ex)
            {
               
            }

            return 0;
        }

        public static async Task MoveFileAsync(string sourceFile, string destinyFile)
        {
            var source = await ApplicationData.Current.LocalFolder.GetFileAsync(sourceFile);
            var folderName = Path.GetDirectoryName(destinyFile);
            var fileName = Path.GetFileName(destinyFile);
            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
            await source.MoveAsync(folder, fileName, NameCollisionOption.ReplaceExisting);
        }

        public static async Task<bool> FolderExists(string folder)
        {
            return await FolderExists(ApplicationData.Current.LocalFolder, folder);
        }

        public static async Task<bool> FolderExists(StorageFolder parentFolder, string folder)
        {
            try
            {
                var storageFolder = await parentFolder.GetFolderAsync(folder);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<StorageFile> CreateFileAsync(string fileName)
        {
            return await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<StorageFile> GetFileAsync(string fileName)
        {
            return await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
        }

        /// <summary>
        /// 将Source 文件夹复制到 destination目录下
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinyFolder"></param>
        /// <returns></returns>
        public static async Task<bool> CopyFolder(StorageFolder sourceFolder, StorageFolder destinationFolder)
        {
            try
            {
                destinationFolder = await destinationFolder.CreateFolderAsync(sourceFolder.Name, CreationCollisionOption.ReplaceExisting);

                foreach (var file in await sourceFolder.GetFilesAsync())
                {
                    await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
                }

                foreach (var folder in await sourceFolder.GetFoldersAsync())
                {
                    await CopyFolder(folder, destinationFolder);
                }

                return true;
            }
            catch (Exception ex)
            {
                
            }

            return false;
        }
    }
}

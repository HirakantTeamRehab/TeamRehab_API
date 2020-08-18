using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Team.Rehab.Repository
{
    public static class CloudeStoreg
    {
        public static byte[] ByteFromFile(string path)
        {
            byte[] bytes = new byte[0];
            try
            {
                string folderPath = string.Empty;
                string filename = string.Empty;
                string fileNameToCompare = string.Empty;
                string[] words = path.Split('\\');
                int length = 0;
                foreach (string word in words)
                {
                    if (words.Length - 1 > length)
                    {
                        if (!string.IsNullOrEmpty(folderPath))
                        {
                            folderPath = folderPath + "/" + word;
                            Console.WriteLine("WORD: " + word);
                        }
                        else
                        {
                            folderPath = word;
                        }
                    }
                    if (words.Length - 1 == length)
                    {
                        filename = word;
                    }

                    length++;
                }
                if (!string.IsNullOrEmpty(filename))
                {
                    fileNameToCompare = filename.Replace(".pdf", "");
                }
                var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
                //var storageAccount =s storageAccount.Parse(conString);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
                var fileClient = storageAccount.CreateCloudFileClient();

              
                var rootdir = fileClient.GetShareReference(folderPath).GetRootDirectoryReference();

                foreach (CloudFile fileItem in rootdir.ListFilesAndDirectories())
                {
                 
                    if (fileItem.Name.Equals(fileNameToCompare))
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileItem.DownloadToStream(ms);
                         return   bytes = ms.ToArray();
                        }
                    }
                    // fileItem.Delete();

                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "/CloudSroeg/ByteFromFile");
               
            }
            return bytes;
        }
        public static MemoryStream MemoryStreamFromFile(string path)
        {
            var ms = new MemoryStream();
            byte[] bytes = new byte[0];
            try
            {
                string folderPath = string.Empty;
                string filename = string.Empty;
                string fileNameToCompare = string.Empty;
                string[] words = path.Split('\\');
                int length = 0;
                foreach (string word in words)
                {
                    if (words.Length - 1 > length)
                    {
                        if (!string.IsNullOrEmpty(folderPath))
                        {
                            folderPath = folderPath + "/" + word;
                            Console.WriteLine("WORD: " + word);
                        }
                        else
                        {
                            folderPath = word;
                        }
                    }
                    if (words.Length - 1 == length)
                    {
                        filename = word;
                    }

                    length++;
                }
                if (!string.IsNullOrEmpty(filename))
                {
                    fileNameToCompare = filename.Replace(".pdf", "");
                }
                var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
                //var storageAccount =s storageAccount.Parse(conString);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
                var fileClient = storageAccount.CreateCloudFileClient();


                var rootdir = fileClient.GetShareReference(folderPath).GetRootDirectoryReference();

                foreach (CloudFile fileItem in rootdir.ListFilesAndDirectories())
                {

                    if (fileItem.Name.Equals(fileNameToCompare))
                    {
                       
                            fileItem.DownloadToStream(ms);
                            return ms;
                       
                    }
                    // fileItem.Delete();

                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "/CloudSroeg/ByteFromFile");

            }
            return ms;
        }
        public static void CreateDirectory(string dirPath, string refPath)
        {
            var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
            //var storageAccount =s storageAccount.Parse(conString);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
            var fileClient = storageAccount.CreateCloudFileClient();



            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference(refPath);
            var cloudFileDirectory = share.GetRootDirectoryReference();
            cloudFileDirectory = cloudFileDirectory.GetDirectoryReference(dirPath);
            cloudFileDirectory.CreateIfNotExists();


            //var cloudFileDirectory = share.GetRootDirectoryReference();

            ////Specify the nested folder
            //var nestedFolderStructure = "Folder/SubFolder";
            //var delimiter = new char[] { '/' };
            //var nestedFolderArray = nestedFolderStructure.Split(delimiter);
            //for (var i = 0; i < nestedFolderArray.Length; i++)
            //{
            //    cloudFileDirectory = cloudFileDirectory.GetDirectoryReference(nestedFolderArray[i]);
            //    cloudFileDirectory.CreateIfNotExists();
            //    Console.WriteLine(cloudFileDirectory.Name + " created...");
            //}
        }

        public static string createFileFromBytes(string containerName, string filePath, byte[] byteArray)
        {

            try
            {

                var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
                //var storageAccount =s storageAccount.Parse(conString);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);

                var fileClient = storageAccount.CreateCloudFileClient();
                // Parse the connection string and return a reference to the storage account.
                var rootdir = fileClient.GetShareReference(containerName).GetRootDirectoryReference();
                CloudFile sourceFile = rootdir.GetFileReference(filePath);
           


                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(byteArray))
                    {
                        sourceFile.UploadFromStream(memoryStream);
                        //blockBlob.UploadFromStream(memoryStream);
                    }
                    return "successful";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

              
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
                throw;
            }
        }


        public static void DeleteFromFileStoreg(string path)
        {
        
            try
            {
                string folderPath = string.Empty;
                string filename = string.Empty;
                string fileNameToCompare = string.Empty;
                string[] words = path.Split('\\');
                int length = 0;
                foreach (string word in words)
                {
                    if (words.Length - 1 > length)
                    {
                        if (!string.IsNullOrEmpty(folderPath))
                        {
                            folderPath = folderPath + "/" + word;
                            Console.WriteLine("WORD: " + word);
                        }
                        else
                        {
                            folderPath = word;
                        }
                    }
                    if (words.Length - 1 == length)
                    {
                        filename = word;
                    }

                    length++;
                }
                if (!string.IsNullOrEmpty(filename))
                {
                    fileNameToCompare = filename.Replace(".pdf", "");
                }
                var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
                //var storageAccount =s storageAccount.Parse(conString);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
                var fileClient = storageAccount.CreateCloudFileClient();


                var rootdir = fileClient.GetShareReference(folderPath).GetRootDirectoryReference();

                foreach (CloudFile fileItem in rootdir.ListFilesAndDirectories())
                {

                    if (fileItem.Name.Equals(fileNameToCompare))
                    {
                     
                            fileItem.Delete();
                             return;
                       
                    }
                    // fileItem.Delete();

                }
            }
            catch (Exception ex)
            {
                CustomLogger.SendExcepToDB(ex, "/CloudSroeg/DeleteFromFileStoreg");

            }
           
        }
    }
}

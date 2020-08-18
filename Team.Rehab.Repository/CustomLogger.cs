using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;

namespace Team.Rehab.Repository
{
    public static class CustomLogger
    {
        public static void Log(string message)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
            var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
            var fileClient = storageAccount.CreateCloudFileClient();

            fileName = fileName.Replace("-", "");
            // Parse the connection string and return a reference to the storage account.
            var rootdir = fileClient.GetShareReference("logs/referrerapp").GetRootDirectoryReference();
            rootdir.GetFileReference(fileName + "txt").UploadText(message);

        }
        public static void LogError(string message)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
            var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
            var fileClient = storageAccount.CreateCloudFileClient();

            fileName = fileName.Replace("-", "");
            // Parse the connection string and return a reference to the storage account.
            var rootdir = fileClient.GetShareReference("logs/referrerapp").GetRootDirectoryReference();
            rootdir.GetFileReference(fileName + "Error.txt").UploadText(message);



        }
        public static void LogMessage(string message, string fileName)
        {
            //string fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
            var conString = Convert.ToString(ConfigurationManager.AppSettings["StoregConn"]);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conString);
            var fileClient = storageAccount.CreateCloudFileClient();

            fileName = fileName.Replace("-", "");
            // Parse the connection string and return a reference to the storage account.
            var rootdir = fileClient.GetShareReference("logs/referrerapp").GetRootDirectoryReference();
            rootdir.GetFileReference(fileName + ".txt").UploadText(message);



        }
        public static void SendExcepToDB(Exception exdb, string exepurl)
        {

            using (RehabEntities context = new RehabEntities())
            {
                string ConnectionString = context.Database.Connection.ConnectionString;
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                builder.ConnectTimeout = 2500;
                SqlConnection con = new SqlConnection(builder.ConnectionString);

                con.Open();

                SqlCommand com = new SqlCommand("SP_APIExceptions", con);
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ExceptionMsg", exdb.Message.ToString());
                com.Parameters.AddWithValue("@ExceptionType", exdb.GetType().Name.ToString());
                com.Parameters.AddWithValue("@ExceptionURL", exepurl);
                com.Parameters.AddWithValue("@ExceptionSource", exdb.StackTrace.ToString());
                com.ExecuteNonQuery();
                con.Close();

            }

        }


    }
}

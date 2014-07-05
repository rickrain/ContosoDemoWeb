using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure.Jobs;
using Models.Library;
using Newtonsoft.Json;


namespace PictureLoader.WebJob
{
    class Program
    {
        const string STORAGE_CONN_STR_KEY_NAME = "AzureJobsStorage";
        const string PICTURE_LOADER_QUEUE_NAME = "picture-loader";

        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(
                ConfigurationManager.ConnectionStrings[STORAGE_CONN_STR_KEY_NAME].ConnectionString, 
                out storageAccount))
            {
                // Create the queue we're using to trigger on if it doesn't already exist.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(PICTURE_LOADER_QUEUE_NAME.ToLower());
                queue.CreateIfNotExists();
            }
            else
            {
                Console.WriteLine("Unable to initialize CloudStorageAccount.");
            }

            JobHost jobHost = new JobHost();
            jobHost.RunAndBlock();
        }

        public static void ProcessQueueMsg([QueueTrigger(PICTURE_LOADER_QUEUE_NAME)] Picture picture)
        {
            if (Uri.IsWellFormedUriString(picture.PictureUrl, UriKind.Absolute))
            {
                Pictures picturesDB = new Pictures();
                picturesDB.PictureEntities.Add(picture);
                picturesDB.SaveChanges();
            }
            else
            {
                // Perform some compensating action here...
                Console.WriteLine("Picture Url '{0}' is invalid.");
            }
        }
    }
}

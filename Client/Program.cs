using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        class Picture
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PictureUrl { get; set; }
        }

        const string STORAGE_CONN_STR_KEY_NAME = "AzureJobsStorage";
        const string PICTURE_LOADER_QUEUE_NAME = "picture-loader";
        const string PICTURE_BLOB_CONTAINER_NAME = "pictures";
             
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount;

            if (CloudStorageAccount.TryParse(
                ConfigurationManager.ConnectionStrings[STORAGE_CONN_STR_KEY_NAME].ConnectionString,
                out storageAccount))
            {
                // Create the queue to put messages into after uploading images.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(PICTURE_LOADER_QUEUE_NAME.ToLower());
                queue.CreateIfNotExists();

                // Create a blob container to upload images to.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(PICTURE_BLOB_CONTAINER_NAME.ToLower());
                blobContainer.CreateIfNotExists(); 
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Container;
                blobContainer.SetPermissions(containerPermissions);

                var imagesPath = System.IO.Directory.GetCurrentDirectory() + @"\Images";
                var imageFiles = System.IO.Directory.GetFiles(imagesPath);
                foreach (string imageFile in imageFiles)
                {
                    // Upload to blob storage.
                    var blobUri = UploadImage(imageFile, blobContainer);

                    // Add a message to queue storage referencing the blob.
                    if (!string.IsNullOrEmpty(blobUri))
                    {
                        Console.WriteLine("Adding message to Azure Storage Queue '{0}.", PICTURE_LOADER_QUEUE_NAME.ToLower());
                        var picture = new Picture()
                        {
                            Name = System.IO.Path.GetFileName(imageFile),
                            PictureUrl = blobUri
                        };
                        queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(picture)));
                    }

                    Console.WriteLine();
                }
            }
        }

        public static string UploadImage(string imageFile, CloudBlobContainer blobContainer)
        {
            var imageFileName = System.IO.Path.GetFileName(imageFile);
            Console.WriteLine("Uploading {0} to blob storage.", imageFileName);

            using (var fileStream = System.IO.File.OpenRead(imageFile))
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(imageFileName);
                if (!blob.Exists())
                {
                    blob.UploadFromStream(fileStream);
                    System.Threading.Thread.Sleep(100);
                    Console.WriteLine(blob.Uri);
                    return blob.Uri.ToString();
                }
                else
                {
                    Console.WriteLine("File '{0}' already exists in Azure Storage.  Skipping...", imageFileName);
                    return null;
                }
            }
        }
    }
}

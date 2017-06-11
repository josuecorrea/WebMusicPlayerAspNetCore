using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebMusicPlayerAspNetCore.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.FileProviders;
using WebMusicPlayerAspNetCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebMusicPlayerAspNetCore.Controllers
{
    [Authorize]
    public class MusicController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;

        public MusicController(UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            CloudStorageAccount storageAccount = new CloudStorageAccount(
    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
    "<storage-account-name>",
    "<access-key>"), true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(user.Id);
            await container.CreateIfNotExistsAsync();

            var listCloudBlob = new List<CloudBlockBlob>();

            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        listCloudBlob.Add(blob);
                        Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                    }

                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;

                        Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                    }

                    else if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;

                        Console.WriteLine("Directory: {0}", directory.Uri);
                    }
                }
            } while (token != null);

            var listOfContents = new List<ListOfContents>();
            foreach (var item in listCloudBlob)
            {
                listOfContents.Add(new ListOfContents
                {
                    Name = item.Name,
                    Path = item.Uri.ToString(),
                    Size = item.Properties.Length
                });
            }

            ViewBag.Contents = listOfContents.ToList();

            return View();
        }
    }
}
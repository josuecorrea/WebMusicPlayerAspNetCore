using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMusicPlayerAspNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WebMusicPlayerAspNetCore.Controllers
{
    public class UploadController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;

        public UploadController(UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewUpload(IList<IFormFile> files)
        {
            var _user = await _userManager.GetUserAsync(User);

           

            CloudStorageAccount storageAccount = new CloudStorageAccount(
    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
    "<storage-account-name>",
    "<access-key>"), true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_user.Id);
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            foreach (var file in files)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
                await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            }

            return View("Index");
        }
    }
}
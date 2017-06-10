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

namespace WebMusicPlayerAspNetCore.Controllers
{
    public class MusicController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IFileProvider _fileProvider;

        public MusicController(UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            _env = env;
            // _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "music");
            var userDirectory = Path.Combine(uploads, user.Id);

            IFileProvider provider = new PhysicalFileProvider(userDirectory);
            IDirectoryContents contents = provider.GetDirectoryContents("");
            
            var listOfContents = new List<ListOfContents>();

            foreach (var item in contents)
            {
                listOfContents.Add(new ListOfContents {
                    Name = item.Name,
                    Path = item.PhysicalPath,
                    Size = item.Length
                });
            }

            ViewBag.Contents = listOfContents.ToList();
            
            return View();
        }
    }
}
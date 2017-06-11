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
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "music");
            var userDirectory = Path.Combine(uploads, user.Id);

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            IFileProvider provider = new PhysicalFileProvider(userDirectory);
            IDirectoryContents contents = provider.GetDirectoryContents("");
            
            var listOfContents = new List<ListOfContents>();
            var root = Path.Combine("uploads", "music");

            foreach (var item in contents)
            {
                listOfContents.Add(new ListOfContents {
                    Name = item.Name,
                    Path = Path.Combine(root, user.Id, item.Name),
                    Size = item.Length
                });
            }

            ViewBag.Contents = listOfContents.ToList();
            
            return View();
        }
    }
}
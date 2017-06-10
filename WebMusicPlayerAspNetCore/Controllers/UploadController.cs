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
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "music");

            var userDirectory = Path.Combine(uploads, _user.Id);

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            long size = files.Sum(f => f.Length);

            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            //return Ok(new { count = file.Count, size, filePath });    
            return View("Index");
        }
    }
}
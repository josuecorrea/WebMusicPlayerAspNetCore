using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebMusicPlayerAspNetCore.Controllers
{
    public class MusicController : Controller
    {
        public async Task<IActionResult> Index()
        {


            return View();
        }
    }
}
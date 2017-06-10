using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebMusicPlayerAspNetCore.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public Guid Identificador { get; set; } = new Guid();
    }
}

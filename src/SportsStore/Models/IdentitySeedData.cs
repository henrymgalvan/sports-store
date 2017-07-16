using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "!Secret123$!";

        public static async void SeedData(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();

            var user = await context.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser(adminUser);
                await context.CreateAsync(user, adminPassword);
            }
        }
    }
}

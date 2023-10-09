using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Data
{
    public static class SeedRoles
    {
        public static async Task SeedIdentityRoles(RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Seeding roles database");

            string[] userRoles = new string[] { "AppUser"};

            foreach (string userRole in userRoles)
            {
                //check if the role exist
                bool roleExist = await roleManager.RoleExistsAsync(userRole);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(userRole));
                    Console.WriteLine($"Role: {userRole} added");
                }
                else
                {
                    Console.WriteLine($"Role: {userRole} already exist in the database.");
                }
            }

            Console.WriteLine("Seeding roles database completed...");
        }
    }
}

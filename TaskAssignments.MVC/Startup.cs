using System;
using Microsoft.Owin;
using Owin;
using UsersAndRolesMVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using SharedModels;

[assembly: OwinStartupAttribute(typeof(UsersAndRolesMVC.Startup))]
namespace UsersAndRolesMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUsersAndRoles();
        }

        private void CreateUsersAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var adminRole = new IdentityRole { Name = "Admin" };
                roleManager.Create(adminRole);
            }

            if (userManager.FindByName("Taher") == null)
            {
                var user = new ApplicationUser { Email = "admin@app.net", UserName = "Mohamed" };
                string pwd = "123@Asd";
                var result = userManager.Create(user, pwd);
                if (!userManager.IsInRole(user.Id, "Admin"))
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Manager"))
            {
                var role = new IdentityRole { Name = "Manager" };
                roleManager.Create(role);

            }
            if (userManager.FindByName("Abdallah") == null)
            {
                var user = new ApplicationUser { Email = "man@app.net", UserName = "Abdallah" };
                string pwd = "123@Asd";
                var result = userManager.Create(user, pwd);

                if (!userManager.IsInRole(user.Id, "Manager"))
                {
                    userManager.AddToRole(user.Id, "Manager");
                }


            }
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new IdentityRole { Name = "Employee" };
                roleManager.Create(role);
            }

            if (userManager.FindByName("Ahmed") == null)
            {
                var user = new ApplicationUser { Email = "emp@app.net", UserName = "Ahmed" };
                string pwd = "123@Asd";
                var result = userManager.Create(user, pwd);
                if (!userManager.IsInRole(user.Id, "Employee"))
                {
                    userManager.AddToRole(user.Id, "Employee");
                }


            }

        }
    }
}

namespace LittleLMS.Migrations
{
    using LittleLMSModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LittleLMS.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Courses.AddOrUpdate(
                p => p.Name,
                  new Course { Name = "E-learning", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-2) },
                  new Course { Name = "Java", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-2).AddDays(-20) },
                  new Course { Name = "Systemutveckling .net", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-3).AddDays(-20) }
                );

            context.ActivityTypes.AddOrUpdate(
            p => p.Name,
              new ActivityType { Name = "E-learning" },
              new ActivityType { Name = "Java" },
              new ActivityType { Name = "Systemutveckling .net" }
            );

            //context.Activities.AddOrUpdate(
            //p => p.Name,
            //  new Activity { Name = "E-learning", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-2) },
            //  new Activity { Name = "Java", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-2).AddDays(-20) },
            //  new Activity { Name = "Systemutveckling .net", Description = "Beskrivning", StartDate = DateTime.Now.AddMonths(-3).AddDays(-20) }
            //);

            context.DocumentTypes.AddOrUpdate(
            p => p.Name,
                new DocumentType { Name = "Inlämningsuppgift" },
                new DocumentType { Name = "Övningsuppgift" },
                new DocumentType { Name = "Uppgift som du kör från din dator hemifrån" }
            );

            context.SaveChanges();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string roleNameLärare = "Lärare";
            string roleNameElev = "Elev";
            if (!roleManager.RoleExists(roleNameLärare))
            {
                var roleresult = roleManager.Create(new IdentityRole(roleNameLärare));
            }

            if (!roleManager.RoleExists(roleNameElev))
            {
                var roleresult = roleManager.Create(new IdentityRole(roleNameElev));
            }

            string userName = "stina.larsson@lexicon.se";
            string password = "Lexicon01!";
            ApplicationUser user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Stina",
                    LastName = "Larsson",
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true
                };

                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, roleNameLärare);
                }
            }

            userName = "goran.persson@lexicon.se";
            user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Göran",
                    LastName = "Persson",
                    TimeOfRegistration = DateTime.Now,
                };

                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, roleNameElev);
                }
            }
        }
    }
}

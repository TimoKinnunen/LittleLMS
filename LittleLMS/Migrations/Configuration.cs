namespace LittleLMS.Migrations
{
    using LittleLMSModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LittleLMS.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {

            context.ReceiverTypes.AddOrUpdate(
                 p => p.Name,
                    new ReceiverType { Name = "L�rare" },
                    new ReceiverType { Name = "Elev" },
                    new ReceiverType { Name = "Kurs" },
                    new ReceiverType { Name = "Modul" },
                    new ReceiverType { Name = "Aktivitet" }
                );

            context.SaveChanges();

            context.ActivityTypes.AddOrUpdate(
                 p => p.Name,
                    new ActivityType { Name = "E-learning" },
                    new ActivityType { Name = "�vning" },
                    new ActivityType { Name = "F�rel�sning" },
                    new ActivityType { Name = "Projektarbete" }
                );

            context.SaveChanges();

            context.DocumentTypes.AddOrUpdate(
                p => p.Name,
                    new DocumentType { Name = "Feedback fr�n l�raren" },
                    new DocumentType { Name = "Feedback fr�n eleven" },
                    new DocumentType { Name = "Kursdokument" },
                    new DocumentType { Name = "Moduldokument" },
                    new DocumentType { Name = "Aktivitetsdokument" },
                    new DocumentType { Name = "Inl�mningsuppgift" },
                    new DocumentType { Name = "�vningsuppgift" },
                    new DocumentType { Name = "F�rel�sningsunderlag" }
                );

            context.SaveChanges();

            context.Courses.AddOrUpdate(
                p => p.Name,
                  new Course { Name = "Linux Grund", Description = "Introduktionskurs i Linux", StartDate = DateTime.Parse("2016-07-29") },
                  new Course { Name = "Systemutveckling, Java", Description = "P�byggnadsutbildning i Java f�r Systemutvecklare. Java.", StartDate = DateTime.Parse("2016-10-29") },
                  new Course { Name = "Systemutveckling, .net", Description = "P�byggnadsutbildning i .Net f�r Systemutvecklare. .Net.", StartDate = DateTime.Parse("2016-08-29") },
                  new Course { Name = "Praktisk Projektledning", Description = "P�byggnadskurs f�r yrkesverksamma Projektledare", StartDate = DateTime.Parse("2016-06-12") }
                );

            context.SaveChanges();

            context.Modules.AddOrUpdate(
                p => p.Name,
                new Module { Name = "C#", Description = "Programmering i C#", StartDate = DateTime.Parse("2016-08-29"), EndDate = DateTime.Parse("2016-09-23"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "Webb", Description = "Grundl�ggande Webb-utveckling", StartDate = DateTime.Parse("2016-09-26"), EndDate = DateTime.Parse("2016-10-07"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "MVC", Description = "Introduktion till MVC", StartDate = DateTime.Parse("2016-10-10"), EndDate = DateTime.Parse("2016-10-21"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "Databaser", Description = "Grundl�ggande Databasmetodik", StartDate = DateTime.Parse("2016-10-24"), EndDate = DateTime.Parse("2016-10-28"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "App.Utv.", Description = "Introduktion till Applikationsutveckling", StartDate = DateTime.Parse("2016-10-31"), EndDate = DateTime.Parse("2016-11-07"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "Test", Description = "Introduktion till Applikationsutveckling", StartDate = DateTime.Parse("2016-11-08"), EndDate = DateTime.Parse("2016-11-14"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, .net").Id },
                new Module { Name = "Java 1", Description = "Java, Modul1", StartDate = DateTime.Parse("2016-08-29"), EndDate = DateTime.Parse("2016-09-23"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, Java").Id },
                new Module { Name = "Java 2", Description = "Java, Modul2", StartDate = DateTime.Parse("2016-09-26"), EndDate = DateTime.Parse("2016-10-07"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, Java").Id },
                new Module { Name = "Java 3", Description = "Java, Modul3", StartDate = DateTime.Parse("2016-10-10"), EndDate = DateTime.Parse("2016-10-21"), CourseId = context.Courses.FirstOrDefault(c => c.Name == "Systemutveckling, Java").Id }
              );
            context.SaveChanges();


            context.Activities.AddOrUpdate(
            p => p.Name,
            new Activity
            {
                Name = "E-L 1-3",
                Description = "C# E-learning 1-3",
                StartDate = DateTime.Parse("2016-08-29"),
                EndDate = DateTime.Parse("2016-08-29"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "C#").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "E-learning").Id
            },
            new Activity
            {
                Name = "Frl C# Intro",
                Description = "F�rel�sning 1, C# Intro",
                StartDate = DateTime.Parse("2016-08-30"),
                EndDate = DateTime.Parse("2016-08-30"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "C#").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "F�rel�sning").Id
            },
            new Activity
            {
                Name = "E-L 4",
                Description = "C# E-learning 4",
                StartDate = DateTime.Parse("2016-08-31"),
                EndDate = DateTime.Parse("2016-08-31"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "C#").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "E-learning").Id
            },
            new Activity
            {
                Name = "E-L 5 - 6",
                Description = "C# E-learning 1-3",
                StartDate = DateTime.Parse("2016-09-26"),
                EndDate = DateTime.Parse("2016-09-26"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "Webb").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "E-learning").Id
            },
            new Activity
            {
                Name = "Frl Javascript",
                Description = "F�rel�sning 1, Javascript",
                StartDate = DateTime.Parse("2016-10-03"),
                EndDate = DateTime.Parse("2016-10-03"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "Webb").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "F�rel�sning").Id
            },
            new Activity
            {
                Name = "�vning1",
                Description = "C# �vning1",
                StartDate = DateTime.Parse("2016-10-05"),
                EndDate = DateTime.Parse("2016-10-05"),
                ModuleId = context.Modules.FirstOrDefault(m => m.Name == "Webb").Id,
                ActivityTypeId = context.ActivityTypes.FirstOrDefault(a => a.Name == "�vning").Id
            }
            );

            context.SaveChanges();



            // Users
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string roleNameL�rare = "L�rare";
            string roleNameElev = "Elev";
            if (!roleManager.RoleExists(roleNameL�rare))
            {
                var roleresult = roleManager.Create(new IdentityRole(roleNameL�rare));
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
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Stina",
                    LastName = "Larsson",
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    CourseId = context.Courses.FirstOrDefault(a => a.Name == "Systemutveckling, .net").Id
                };

                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, roleNameL�rare);
                }
            }

            userName = "nisse.hult@lexicon.se";
            user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Nisse",
                    LastName = "Hult",
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    CourseId = context.Courses.FirstOrDefault(a => a.Name == "Systemutveckling, .net").Id
                };

                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, roleNameL�rare);
                }
            }

            userName = "goran.persson@lexicon.se";
            user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "G�ran",
                    LastName = "Persson",
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    CourseId = context.Courses.FirstOrDefault(a => a.Name == "Systemutveckling, .net").Id

                };

                IdentityResult userResult = userManager.Create(user, password);
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(user.Id, roleNameElev);
                }
            }

            userName = "kajsa.kavat@lexicon.se";
            user = userManager.FindByName(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Kajsa",
                    LastName = "Kavat",
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    CourseId = context.Courses.FirstOrDefault(a => a.Name == "Systemutveckling, .net").Id
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

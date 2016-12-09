using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    [Authorize(Roles = "Lärare,Elev")]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public CoursesController()
        {
        }

        public CoursesController(ApplicationUserManager userManager, ApplicationRoleManager signInManager)
        {
            UserManager = userManager;
            RoleManager = signInManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Courses
        public async Task<ActionResult> Index(int? studentModuleId)
        {
            if (User.IsInRole("Elev"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Kursöversikt för eleven " + user.FullName + ".";

                ViewBag.CourseModules = new List<Course>();
                ViewBag.ModuleActivities = new List<Activity>();

                var courseId = user.CourseId;
                Course course = await db.Courses.FindAsync(courseId);
                ViewBag.CourseName = "Kursnamn: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar: " : "Kursen har startat: " + string.Format("{0:dd MMM yyyy}", course.StartDate); // course.StartDate;
                var courseModules = await db.Modules.Where(m => m.CourseId == courseId).ToListAsync();
                ViewBag.CourseModules = courseModules;
                if (courseModules == null)
                {
                    ViewBag.CourseName = "Kurs saknar moduler.";
                }

                ViewBag.ModuleActivities = new List<Activity>();
                if (studentModuleId.HasValue)
                {
                    var existingModule = await db.Modules.Where(m => m.Id == studentModuleId && m.CourseId == courseId).FirstOrDefaultAsync();
                    if (existingModule != null)
                    {
                        ViewBag.ModuleName = "Modul: " + existingModule.Name + ".";
                    }
                    else
                    {
                        ViewBag.ModuleName = "Modul saknas.";
                    }
                    var moduleActivities = await db.Activities.Where(a => a.ModuleId == studentModuleId).ToListAsync();
                    ViewBag.ModuleActivities = moduleActivities;
                    if (moduleActivities == null)
                    {
                        ViewBag.ModuleName = "Modulen saknar aktiviteter.";
                    }
                }
                else
                {
                    var firstModule = await db.Modules.Where(m => m.CourseId == courseId).FirstOrDefaultAsync();
                    if (firstModule != null)
                    {
                        ViewBag.ModuleName = "Modul: " + firstModule.Name + ".";
                        var moduleActivities = await db.Activities.Where(a => a.ModuleId == firstModule.Id).ToListAsync();
                        ViewBag.ModuleActivities = moduleActivities;
                        if (moduleActivities == null)
                        {
                            ViewBag.ModuleName = "Modulen saknar aktiviteter.";
                        }
                    }
                    else
                    {
                        ViewBag.ModuleName = "Modulen: saknar aktiviteter.";
                    }
                }

                var students = new List<ApplicationUser>();

                foreach (var applicationUser in await UserManager.Users.ToListAsync())
                {
                    var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                    if (userRoles.Contains("Elev"))
                    {
                        students.Add(applicationUser);
                    }
                }

                ViewBag.CourseStudentMates = students;

                return View(await db.Courses.Where(c => c.Id == courseId).ToListAsync());
            }

            if (User.IsInRole("Lärare"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Kursöversikt för läraren " + user.FullName + ".";

                return View(await db.Courses.ToListAsync());
            }

            return View(await db.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,StartDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,StartDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Course course = await db.Courses.FindAsync(id);
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

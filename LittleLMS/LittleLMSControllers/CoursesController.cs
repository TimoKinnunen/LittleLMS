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
        public async Task<ActionResult> Index(int? moduleId)
        {
            if (User.IsInRole("Elev"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                var courseId = user.CourseId;
                Course course = await db.Courses.FindAsync(courseId);
                ViewBag.UserName = "Kursöversikt för eleven " + user.FullName + ".";
                ViewBag.CourseName = "Kursnamn: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar: " : "Kursen har startat: " + string.Format("{0:dd MMM yyyy}", course.StartDate); // course.StartDate;
                var courseModules = await db.Modules.Where(m => m.CourseId == courseId).ToListAsync();
                ViewBag.CourseModules = courseModules;
                if (courseModules.Count == 0)
                {
                    ViewBag.CourseName = "Kurs: " + course.Name + " saknar moduler.";
                }

                if (moduleId.HasValue)
                {
                    var existingModule = await db.Modules.Where(m => m.Id == moduleId && m.CourseId == courseId).FirstOrDefaultAsync();
                    ViewBag.ModuleName = "Modul: " + existingModule.Name + ".";
                    var moduleActivities = await db.Activities.Where(a => a.ModuleId == moduleId).ToListAsync();
                    ViewBag.ModuleActivities = moduleActivities;
                    if (moduleActivities.Count == 0)
                    {
                        ViewBag.ModuleName = "Modulen: " + existingModule.Name + " saknar aktiviteter.";
                    }
                }
                else
                {
                    var firstModule = await db.Modules.Where(m => m.CourseId == courseId).FirstOrDefaultAsync();
                    ViewBag.ModuleName = "Modul: " + firstModule.Name + ".";
                    var moduleActivities = await db.Activities.Where(a => a.ModuleId == firstModule.Id).ToListAsync();
                    ViewBag.ModuleActivities = moduleActivities;
                    if (moduleActivities.Count == 0)
                    {
                        ViewBag.ModuleName = "Modulen: " + firstModule.Name + " saknar aktiviteter.";
                    }
                }

                ViewBag.CourseStudentMates = await UserManager.Users.Where(u => u.CourseId == courseId).ToListAsync();

                return View(await db.Courses.Where(c => c.Id == courseId).ToListAsync());
            }

            if (User.IsInRole("Lärare"))
            {
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

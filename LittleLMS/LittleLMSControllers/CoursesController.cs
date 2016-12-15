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
    using System.IO;
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
            ViewBag.CourseModules = new List<Module>();
            ViewBag.ModuleActivities = new List<Activity>();
            ViewBag.CourseStudents = new List<ApplicationUser>();
            ViewBag.CourseModulesMessage = "Kursen saknar moduler.";
            ViewBag.ModuleActivitiesMessage = "Modulen saknar aktiviteter.";
            ViewBag.CourseStudentsMessage = "Kurs saknar deltagare.";
            ViewBag.TeacherDocuments = new List<Document>();
            ViewBag.StudentDocuments = new List<Document>();
            ViewBag.CourseDocuments = new List<Document>();
            ViewBag.ModuleDocuments = new List<Document>();
            ViewBag.ActivityDocuments = new List<Document>();

            if (User.IsInRole("Elev"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Kursöversikt för eleven " + user.FullName + ".";

                var courseId = user.CourseId;
                if (courseId == null)
                {
                    ModelState.AddModelError("", "Eleven " + user.Email + " är inte kopplad till en kurs.");
                    return View();
                }

                #region course
                Course course = await db.Courses.FindAsync(courseId);
                ViewBag.CourseName = "Kursnamn: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar " : "Kursen har startat " + string.Format("{0:d}.", course.StartDate);
                #endregion course

                #region student documents
                var documents = new List<Document>();

                var dbUser = await db.Users.FirstAsync(u => u.Id == userId);
                if (dbUser != null)
                {
                    documents = dbUser.UserDocuments.ToList();
                }

                ViewBag.StudentDocuments = documents;
                ViewBag.StudentDocumentsMessage = "Antal dokument är " + documents.Count + ".";
                #endregion student documents

                #region module
                Module existingModule = null;
                var courseModules = await db.Modules.Where(m => m.CourseId == courseId).ToListAsync();
                if (courseModules.Count > 0)
                {
                    ViewBag.CourseModules = courseModules;
                    ViewBag.CourseModulesMessage = string.Empty;

                    if (moduleId.HasValue)
                    {
                        existingModule = await db.Modules.Where(m => m.Id == moduleId && m.CourseId == courseId).FirstOrDefaultAsync();
                    }
                    else
                    {
                        existingModule = await db.Modules.Where(m => m.CourseId == courseId).FirstOrDefaultAsync();
                    }
                }

                if (existingModule != null)
                {
                    var moduleActivities = await db.Activities.Where(a => a.ModuleId == existingModule.Id).ToListAsync();
                    if (moduleActivities.Count > 0)
                    {
                        ViewBag.ModuleActivities = moduleActivities;
                        ViewBag.ModuleActivitiesMessage = "Modulens namn: " + existingModule.Name + ".";

                        #region activity documents
                        List<Document> activityDocuments = new List<Document>();
                        foreach (var activity in moduleActivities)
                        {
                            foreach (var document in activity.ActivityDocuments)
                            {
                                activityDocuments.Add(document);
                            }
                        }
                        ViewBag.ActivityDocuments = activityDocuments;
                        ViewBag.ActivityDocumentsMessage = "Antal dokument är " + activityDocuments.Count + ".";
                        #endregion activity documents

                    }
                    #region module documents
                    var moduleDocuments = existingModule.ModuleDocuments.ToList();
                    ViewBag.ModuleDocuments = moduleDocuments;
                    ViewBag.ModuleDocumentsMessage = "Antal dokument är " + moduleDocuments.Count + ".";
                    #endregion module documents
                }
                #endregion module

                #region students
                var students = new List<ApplicationUser>();

                foreach (var applicationUser in await UserManager.Users.ToListAsync())
                {
                    var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                    //var userCourseId = applicationUser.CourseId;
                    if (userRoles.Contains("Elev"))
                    {
                        if (applicationUser.CourseId == user.CourseId)
                        {
                            students.Add(applicationUser);
                        }
                    }
                }

                ViewBag.CourseStudents = students;
                ViewBag.CourseStudentsMessage = "Antal elever är " + students.Count + ".";
                #endregion students

                #region course documents
                var courseDocuments = course.CourseDocuments.ToList();
                ViewBag.CourseDocuments = courseDocuments;
                ViewBag.CourseDocumentsMessage = "Antal dokument är " + courseDocuments.Count + ".";
                #endregion course documents

                return View(await db.Courses.Where(c => c.Id == courseId).ToListAsync());
            }

            if (User.IsInRole("Lärare"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Lärare " + user.FullName + ". Du kan lägga till och redigera kurser.";


                #region students
                var students = new List<ApplicationUser>();

                foreach (var applicationUser in await UserManager.Users.ToListAsync())
                {
                    var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                    if (userRoles.Contains("Elev"))
                    {
                        students.Add(applicationUser);
                    }
                }

                ViewBag.CourseStudents = students;
                ViewBag.CourseStudentsMessage = "Antal elever är " + students.Count + ".";
                #endregion students

                #region teacher documents
                var documents = new List<Document>();

                var dbUser = await db.Users.FirstAsync(u => u.Id == userId);
                if (dbUser != null)
                {
                    documents = dbUser.UserDocuments.ToList();
                }

                ViewBag.TeacherDocuments = documents;
                ViewBag.TeacherDocumentsMessage = "Antal dokument är " + documents.Count + ".";
                #endregion teacher documents

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

        // GET: download a file
        [HttpGet]
        public async Task<FileResult> Download(int? id)
        {
            Document document = await db.Documents.FindAsync(id);

            return File(document.Content, document.ContentType, Path.GetFileName(document.FileName));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}

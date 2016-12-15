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
    using System.Text.RegularExpressions;
    using System.Web;

    [Authorize(Roles = "Lärare")]
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public ActivitiesController()
        {
        }

        public ActivitiesController(ApplicationUserManager userManager, ApplicationRoleManager signInManager)
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

        // GET: Activities
        public async Task<ActionResult> Index(int? moduleId)
        {
            if (User.IsInRole("Lärare"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Lärare " + user.FullName + ". Du kan lägga till och redigera aktiviteter.";

                Module module = await db.Modules.FindAsync(moduleId);

                #region course
                Course course = await db.Courses.FindAsync(module.Course.Id);
                ViewBag.CourseId = course.Id;
                ViewBag.CourseName = "Kurs: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar " : "Kursen har startat " + string.Format("{0:d}.", course.StartDate);
                #endregion course

                #region module
                ViewBag.ModuleName = "Modul: " + module.Name;
                ViewBag.ModuleDescription = "Modulbeskrivning: " + module.Description;
                //ViewBag.ModuleInterval = module.StartDate > DateTime.Now ? "Modulen startar " : "Modulen har startat " + string.Format("{0:d}.", module.StartDate);
                #endregion module

                ViewBag.ModuleId = moduleId;
                return View(await db.Activities.Where(a => a.ModuleId == moduleId).ToListAsync());
            }

            ViewBag.ModuleId = moduleId;
            return View(await db.Activities.ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int? moduleId)
        {
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name");
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name");

            #region course
            Module module = db.Modules.Find(moduleId);
            int courseId = module.CourseId;
            Course course = db.Courses.Find(courseId);
            ViewBag.CourseName = course.Name;
            //ViewBag.HeaderText = "Kurs: " + course.Name + " >> " + "Modul: " + module.Name;
            //ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
            //ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar " : "Kursen har startat " + string.Format("{0:d}.", course.StartDate);
            #endregion course

            #region module
            ViewBag.ModuleName = module.Name;
            //ViewBag.HeaderTextModule = "Modul: " + module.Name;
            //ViewBag.ModuleDescription = "Modulbeskrivning: " + module.Description;
            //ViewBag.ModuleInterval = module.StartDate > DateTime.Now ? "Modulen startar " : "Modulen har startat " + string.Format("{0:d}.", module.StartDate);
            #endregion module


            ViewBag.ModuleId = moduleId;
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ActivityTypeId,ModuleId,Name,Description,StartDate,EndDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                //var xxx = activity.ModuleId;
                Module module = await db.Modules.FindAsync(activity.ModuleId);
                db.Activities.Add(activity);
                await db.SaveChangesAsync();
                int m_id = (int)activity.ModuleId;
                return RedirectToAction("Index", new { moduleId = m_id });
            }

            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            ViewBag.ModuleId = activity.ModuleId;
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ActivityTypeId,Name,Description,StartDate,EndDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                int moduleId = db.Activities
                    .AsNoTracking()
                    .Where(a => a.Id == activity.Id)
                    .Select(a => a.ModuleId)
                    .SingleOrDefault();
                activity.ModuleId = moduleId;
                db.Entry(activity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { moduleId });
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleId = activity.ModuleId;
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Activity activity = await db.Activities.FindAsync(id);
            int moduleId = activity.ModuleId;
            db.Activities.Remove(activity);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { moduleId });
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

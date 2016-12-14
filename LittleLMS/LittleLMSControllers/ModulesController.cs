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

    [Authorize(Roles = "Lärare")]
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public ModulesController()
        {
        }

        public ModulesController(ApplicationUserManager userManager, ApplicationRoleManager signInManager)
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

        // GET: Modules
        public async Task<ActionResult> Index(int? courseId)
        {
            ViewBag.CourseModules = new List<Module>();
            ViewBag.CourseModulesMessage = "Kursen saknar moduler.";

            if (User.IsInRole("Lärare"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Lärare " + user.FullName + ". Du kan lägga till och redigera moduler.";

                // läraren använder inte fältet CourseId och vi mellanlagrar där courseId
                if (courseId == null)
                {
                    courseId = user.CourseId;
                }
                else
                {
                    user.CourseId = courseId;
                }

                #region course
                Course course = await db.Courses.FindAsync(courseId);
                ViewBag.CourseId = course.Id;
                ViewBag.CourseName = "Kurs: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar " : "Kursen har startat " + string.Format("{0:dd MMM yyyy}.", course.StartDate);
                #endregion course

                return View(await db.Modules.Where(m => m.CourseId == courseId).ToListAsync());
            }

            return View(await db.Modules.ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = await db.Modules.FindAsync(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        public ActionResult Create(int? courseId)
        {
            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
            #region course
            Course course = db.Courses.Find(courseId);
            ViewBag.CourseId = course.Id;
            ViewBag.HeaderText = "Kurs: " + course.Name;
            //ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
            #endregion course

            //ViewBag.CourseId = courseId;
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CourseId,Name,Description,StartDate,EndDate")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                await db.SaveChangesAsync();
                int c_id = (int)module.CourseId;
                return RedirectToAction("Index", new { courseId = c_id });
            }

            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            return View(module);
        }

        // GET: Modules/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = await db.Modules.FindAsync(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            ViewBag.CourseId = module.CourseId;
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate")] Module module)
        {
            if (ModelState.IsValid)
            {
                int courseId = db.Modules
                    .AsNoTracking()
                    .Where(m => m.Id == module.Id)
                    .Select(m => m.CourseId)
                    .SingleOrDefault();
                module.CourseId = courseId;
                db.Entry(module).State = EntityState.Modified;

                await db.SaveChangesAsync();                
                return RedirectToAction("Index", new { courseId });
            }
            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            return View(module);
        }

        // GET: Modules/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = await db.Modules.FindAsync(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = module.CourseId;
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Module module = await db.Modules.FindAsync(id);
            int courseId = module.CourseId;
            db.Modules.Remove(module);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { courseId });
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

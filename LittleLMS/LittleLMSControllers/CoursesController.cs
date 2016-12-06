using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers {
    using Microsoft.AspNet.Identity.Owin;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    [Authorize(Roles = "Lärare,Elev")]
    public class CoursesController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CoursesController() {
        }

        public CoursesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }
        // GET: Courses
        public async Task<ActionResult> Index() {

            if (User.IsInRole("Elev")) {
                //var user = await UserManager.FindByIdAsync(model.Email);
                return View(await db.Courses.Where(c => c.Id == 1).ToListAsync());
            }

            if (User.IsInRole("Lärare")) {
                return View(await db.Courses.ToListAsync());
            }
            return View(await db.Courses.ToListAsync());

        }

        //public ActionResult Modules(int id) {
         //    var modules = db.Modules.Where(m => m.Id == id).ToList();

        //    return View(modules);
        //}

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);

            if (course == null) {
                return HttpNotFound();
            }

            //Module module = await db.Modules.FindAsync(id);
            //if (module == null) {
            //    return HttpNotFound();
            //}

            //var modules = db.Modules.Where(a => a.Id == course.Id).ToList();
            //if (modules == null) {
            //    return HttpNotFound();
            //}
            //ViewBag.Anders = modules;

            //Overview overview = new Overview();
            //overview.Id = course.Id;
            //overview.CourseName = course.Name;
            //overview.CourseDescription = course.Description;
            //overview.CourseStartDate = course.StartDate;
            //overview.ModuleName = module.Name;
            //overview.ModuleDescription = module.Description;
            //overview.ModuleStartDate = module.StartDate;
            //overview.ModuleEndDate = module.EndDate;
            //return View(overview);

            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,StartDate")] Course course) {
            if (ModelState.IsValid) {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null) {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,StartDate")] Course course) {
            if (ModelState.IsValid) {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null) {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            Course course = await db.Courses.FindAsync(id);
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSViewModels
{
    [Authorize(Roles = "Lärare")]
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public StudentsController()
        {
        }

        public StudentsController(ApplicationUserManager userManager, ApplicationRoleManager signInManager)
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

        // GET: Students
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Students.ToListAsync());
        //}

        // GET: Students/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Student student = await db.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // GET: Students/Create
        public async Task<ActionResult> Create(int? courseId)
        {
            if (User.IsInRole("Lärare"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                ViewBag.UserName = "Lärare " + user.FullName + ". Du kan lägga till en elev.";

                #region course
                Course course = await db.Courses.FindAsync(courseId);
                ViewBag.CourseName = "Kursnamn: " + course.Name;
                ViewBag.CourseDescription = "Kursbeskrivning: " + course.Description;
                ViewBag.CourseInterval = course.StartDate > DateTime.Now ? "Kursen startar " : "Kursen har startat " + string.Format("{0:d}.", course.StartDate);
                #endregion course

                UserViewModel student = new UserViewModel
                {
                    FirstName = "Förnamn",
                    LastName = "Efternamn",
                    Email = "fornman.efternamn@lexicon.se",
                    CourseId = course.Id
                };

                return View(student);
            }
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FirstName,LastName,Email,CourseId")] UserViewModel student)
        {
            if (ModelState.IsValid)
            {
                string userName = student.Email;
                string roleNameElev = "Elev";
                string password = "Lexicon01!"; // alla elever får läsenordet Lexicon01!

                var user = new ApplicationUser
                {
                    UserName = student.Email,
                    Email = student.Email,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    CourseId = student.CourseId,
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true
                };

                //Add User
                var userResult = await UserManager.CreateAsync(user, password);
                if (!userResult.Succeeded)
                {
                    ModelState.AddModelError("", userResult.Errors.First());
                    UserViewModel newStudent = new UserViewModel
                    {
                        FirstName = "Förnamn",
                        LastName = "Efternamn",
                        Email = "fornman.efternamn@lexicon.se",
                        CourseId = student.CourseId
                    };
                    return View(newStudent);
                }

                //Add User to the selected Role
                if (userResult.Succeeded)
                {
                    var result = await UserManager.AddToRolesAsync(user.Id, roleNameElev);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        UserViewModel newStudent = new UserViewModel
                        {
                            FirstName = "Förnamn",
                            LastName = "Efternamn",
                            Email = "fornman.efternamn@lexicon.se",
                            CourseId = student.CourseId
                        };
                        return View(newStudent);
                    }
                }
                return RedirectToAction("Index", "Courses");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Student student = await db.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Course,FirstName,LastName")] Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(student).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(student);
        //}

        // GET: Students/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Student student = await db.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // POST: Students/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Student student = await db.Students.FindAsync(id);
        //    db.Students.Remove(student);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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

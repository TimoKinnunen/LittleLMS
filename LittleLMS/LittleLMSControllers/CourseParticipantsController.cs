using LittleLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers {
    public class CourseParticipantsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public CourseParticipantsController() {
        }

        public CourseParticipantsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager {
            get {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set {
                _roleManager = value;
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

        // GET: CourseParticipants
        public async Task<ActionResult> Index(int? courseId) {
            //if (User.IsInRole("Elev")) {
            //    var userId = User.Identity.GetUserId();
            //    ApplicationUser user = await UserManager.FindByIdAsync(userId);
            //    if (!courseId.HasValue) {
            //        courseId = user.CourseId;
            //    }

            //    return View(await db.Users.Where(u => u.CourseId == courseId).ToListAsync());
            //}

            var roles = await RoleManager.Roles.ToListAsync();
            var users = await UserManager.Users.ToListAsync();

            var students = new List<ApplicationUser>();
            var teachers = new List<ApplicationUser>();

            foreach (var role in roles) {
                if (role.Name == "Elev") {
                    foreach (var user in users) {
                        var userRoles = UserManager.GetRoles(user.Id);
                        if (userRoles.Contains("Elev")) {
                            students.Add(user);
                        }
                    }
                } else {
                    if (role.Name == "Lärare") {
                        foreach (var user in users) {
                            var userRoles = UserManager.GetRoles(user.Id);
                            if (userRoles.Contains("Lärare")) {
                                teachers.Add(user);
                            }
                        }
                    }
                }
            }

            ViewBag.Students = students;
            ViewBag.WhichCourse = students.FirstOrDefault().CourseId;
            //return View(await db.Users.ToListAsync());
            return View(teachers);

            //if (courseId.HasValue)
            //{
            //    ViewBag.WhichCourse = "Kurs :" + courseId;
            //    var participants = UserManager.Users.Where(u => u.CourseId == courseId);
            //    return View(await participants.ToListAsync());
            //}
            //ViewBag.WhichCourse = "Alla kurser";
            //return View(await UserManager.Users.ToListAsync());
        }
    }
}
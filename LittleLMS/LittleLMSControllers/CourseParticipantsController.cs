using LittleLMS.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
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
            var students = new List<ApplicationUser>();
            var teachers = new List<ApplicationUser>();

            foreach (var applicationUser in await UserManager.Users.ToListAsync())
            {
                var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                if (userRoles.Contains("Elev"))
                {
                    students.Add(applicationUser);
                }
                else
                {
                    if (userRoles.Contains("Lärare"))
                    {
                        teachers.Add(applicationUser);
                    }
                }
            }

            ViewBag.Students = students;
            ViewBag.WhichCourse = students.FirstOrDefault().CourseId;

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
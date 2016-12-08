using LittleLMS.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
    public class CourseParticipantsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CourseParticipantsController()
        {
        }

        public CourseParticipantsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
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

        // GET: CourseParticipants
        public async Task<ActionResult> Index(int? courseId)
        {
            if (courseId.HasValue)
            {
                ViewBag.WhichCourse = "Kurs :" + courseId;
                var participants = UserManager.Users.Where(u => u.CourseId == courseId);
                return View(await participants.ToListAsync());
            }
            ViewBag.WhichCourse = "Alla kurser";
            return View(await UserManager.Users.ToListAsync());
        }
    }
}
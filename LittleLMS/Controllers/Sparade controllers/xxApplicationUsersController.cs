using LittleLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.Controllers
{
    public class xxApplicationUsersController : Controller
    {
        private ApplicationSignInManager applicationSignInManager;
        private ApplicationUserManager applicationUserManager;

        public xxApplicationUsersController()
        {
        }

        public xxApplicationUsersController(ApplicationUserManager applicationUserManager, ApplicationSignInManager applicationSignInManager)
        {
            ApplicationUserManager = applicationUserManager;
            SignInManager = applicationSignInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return applicationSignInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                applicationSignInManager = value;
            }
        }

        public ApplicationUserManager ApplicationUserManager
        {
            get
            {
                return applicationUserManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                applicationUserManager = value;
            }
        }

        // GET: ApplicationUsers
        public async Task<ActionResult> Index()
        {
            return View(await ApplicationUserManager.Users.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = await ApplicationUserManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newApplicationUser = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TimeOfRegistration = DateTime.Now,
                    Email = model.Email,
                    UserName = model.Email
                };

                IdentityResult identityResult = await ApplicationUserManager.CreateAsync(newApplicationUser, model.Password);
                if (identityResult.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return RedirectToAction("Index");
                }
                AddErrors(identityResult);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = await ApplicationUserManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            EditApplicationUserViewModel editApplicationUserViewModel = new EditApplicationUserViewModel
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName
            };
            return View(editApplicationUserViewModel);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditApplicationUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser existingApplicationUser = await ApplicationUserManager.FindByIdAsync(model.Id);
                existingApplicationUser.FirstName = model.FirstName;
                existingApplicationUser.LastName = model.LastName;
                IdentityResult identityResult = await ApplicationUserManager.UpdateAsync(existingApplicationUser);
                if (identityResult.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return RedirectToAction("Index");
                }
                AddErrors(identityResult);
            }
            return View(model);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser existingApplicationUser = await ApplicationUserManager.FindByIdAsync(id);
            if (existingApplicationUser == null)
            {
                return HttpNotFound();
            }
            return View(existingApplicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser existingApplicationUser = await ApplicationUserManager.FindByIdAsync(id);
            await ApplicationUserManager.DeleteAsync(existingApplicationUser);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ApplicationUserManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

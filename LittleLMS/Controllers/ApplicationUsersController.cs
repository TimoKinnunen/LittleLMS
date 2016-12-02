﻿using LittleLMS.LittleLMSViewModels;
using LittleLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LittleLMS.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationSignInManager applicationSignInManager;
        private ApplicationUserManager applicationUserManager;

        public ApplicationUsersController()
        {
        }

        public ApplicationUsersController(ApplicationUserManager applicationUserManager, ApplicationSignInManager applicationSignInManager)
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

            ApplicationDbContext db = new ApplicationDbContext();
            List<ApplicationUserViewModel> applicationUserViewModels = new List<ApplicationUserViewModel>();
            foreach (var applicationUser in ApplicationUserManager.Users)
            {
                int courseId;
                string courseName;

                if (applicationUser.CourseId.HasValue)
                {
                    courseId = (int)applicationUser.CourseId;
                    var course = await db.Courses.Where(c => c.Id == courseId).FirstOrDefaultAsync();
                    courseName = course.Name;
                }
                else
                {
                    courseId = 0;
                    courseName = "kursnamn saknas";
                }
                ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel
                {
                    Id = applicationUser.Id,
                    CourseId = courseId,
                    CourseName = courseName,
                    Email = applicationUser.Email,
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    FullName = applicationUser.FirstName,
                    TimeOfRegistration = applicationUser.TimeOfRegistration
                };
                applicationUserViewModels.Add(applicationUserViewModel);
            }

            return View(applicationUserViewModels);
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

            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", newApplicationUser.CourseId);
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

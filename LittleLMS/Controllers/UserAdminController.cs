using LittleLMS.LittleLMSViewModels;
using LittleLMS.Models;
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
    [Authorize(Roles = "Lärare")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
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

        private ApplicationRoleManager _roleManager;
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

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            #region teachers
            var users = new List<UserViewModel>();

            foreach (var applicationUser in await UserManager.Users.ToListAsync()) {
                var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                if (userRoles.Contains("Lärare")) {
                    UserViewModel userViewModel = new UserViewModel {
                        Id = applicationUser.Id,
                        FirstName = applicationUser.FirstName,
                        LastName = applicationUser.LastName,
                        Email = applicationUser.Email,
                        RoleAsText = "Lärare"
                    };
                    users.Add(userViewModel);
                }
            }

            //ViewBag.Teachers = teachers;
            #endregion

            #region students
            //var students = new List<UserViewModel>();

            foreach (var applicationUser in await UserManager.Users.ToListAsync()) {
                var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                if (userRoles.Contains("Elev")) {
                    UserViewModel userViewModel = new UserViewModel {
                        Id = applicationUser.Id,
                        FirstName = applicationUser.FirstName,
                        LastName = applicationUser.LastName,
                        Email = applicationUser.Email,
                        RoleAsText = "Elev"
                    };
                    users.Add(userViewModel);
                }
            }

            //ViewBag.Students = students;
            #endregion students

            //return View(await UserManager.Users.ToListAsync());
            return View(users);
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                //if (selectedRoles == null || selectedRoles.Count() > 1 || selectedRoles.Count() == 0)
                //{
                //    ModelState.AddModelError("", "Välj rollen 'Elev' eller rollen 'Lärare'.");
                //    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                //    return View();
                //}
                string roleNameLärare = "Lärare";
                var user = new ApplicationUser
                {
                    UserName = userViewModel.Email,
                    Email = userViewModel.Email,
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    TimeOfRegistration = DateTime.Now,
                    EmailConfirmed = true
                };
                user.TimeOfRegistration = DateTime.Now;

                //Add Lärare
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                if (!adminresult.Succeeded)
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();
                }

                //Add Lärare to the selected Role
                if (adminresult.Succeeded)
                {
                    //if (selectedRoles != null)
                    //{
                        //var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        var result = await UserManager.AddToRolesAsync(user.Id, roleNameLärare);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    //}
                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,FirstName,LastName,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.FirstName = editUser.FirstName;
                user.LastName = editUser.LastName;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                if (selectedRole == null || selectedRole.Count() > 1 || selectedRole.Count() == 0)
                {
                    ModelState.AddModelError("", "Välj rollen 'Elev' eller rollen 'Lärare'.");
                    //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        Email = editUser.Email,
                        FirstName = editUser.FirstName,
                        LastName = editUser.LastName,
                        RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    });
                }

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
    using LittleLMSViewModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Web;

    [Authorize(Roles = "Lärare")]
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public DocumentsController()
        {
        }

        public DocumentsController(ApplicationUserManager userManager, ApplicationRoleManager signInManager)
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
        // GET: Documents
        public async Task<ActionResult> Index()
        {
            return View(await db.Documents.OrderByDescending(d => d.TimeOfRegistration).ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        public async Task<ActionResult> UploadFile()
        {
            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name");
            ViewBag.ReceiverTypeId = new SelectList(db.ReceiverTypes, "Id", "Name");

            if (User.IsInRole("Lärare") || User.IsInRole("Elev"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);

                Document document = new Document
                {
                    UploadedByName = user.FullName
                };

                #region students
                var students = new List<DocumentStudentsViewModel>();

                foreach (var applicationUser in await UserManager.Users.ToListAsync())
                {
                    var userRoles = await UserManager.GetRolesAsync(applicationUser.Id);
                    if (userRoles.Contains("Elev"))
                    {
                        if (applicationUser.Id != userId)
                        {
                            students.Add(new DocumentStudentsViewModel
                            {
                                StudentId = applicationUser.Id,
                                StudentName = applicationUser.FullName,
                                StudentEmail = applicationUser.Email,
                                IsSelected = false
                            });
                        }
                    }
                }
                ViewBag.AllStudents = students;
                ViewBag.AllStudentsMessage = "Antal elever är " + students.Count + ".";
                #endregion students

                return View(document);
            }
            ModelState.AddModelError("", "Du måste vara lärare eller elev för att ladda upp dokument.");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // http://www.mikesdotnetting.com/article/259/asp-net-mvc-5-with-ef-6-working-with-files
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadFile([Bind(Include = "Id,DocumentTypeId,ReceiverTypeId,FileName,DocumentName,Description,UploadedByName,UploadedByUserId,TimeOfRegistration,ContentType,Content")] Document document, string[] students, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (User.IsInRole("Lärare") || User.IsInRole("Elev"))
                        {
                            var userId = User.Identity.GetUserId();

                            Document dokumentToUpload = new Document
                            {
                                DocumentTypeId = document.DocumentTypeId,
                                ReceiverTypeId = document.ReceiverTypeId,
                                DocumentName = document.DocumentName,
                                FileName = Path.GetFileName(upload.FileName),
                                Description = document.Description,
                                UploadedByName = document.UploadedByName,
                                UploadedByUserId = userId,
                                TimeOfRegistration = DateTime.Now,
                                ContentType = upload.ContentType
                            };

                            using (var binaryReader = new BinaryReader(upload.InputStream))
                            {
                                dokumentToUpload.Content = binaryReader.ReadBytes(upload.ContentLength);
                            }

                            #region add document to inlogged user's list of documents
                            db.Documents.Add(dokumentToUpload);
                            await db.SaveChangesAsync();

                            var dbUser = await db.Users.FirstAsync(u => u.Id == userId);
                            if (dbUser != null)
                            {
                                dbUser.UserDocuments.Add(dokumentToUpload);
                                await db.SaveChangesAsync();
                            }
                            #endregion add document to inlogged user's list of documents

                            #region add document to marked student's list of documents
                            foreach (var student in students)
                            {
                                var elev = db.Users.Where(u => u.Id == student).FirstOrDefault();
                                if (elev != null)
                                {
                                    elev.UserDocuments.Add(dokumentToUpload);
                                }
                            }
                            await db.SaveChangesAsync();
                            #endregion add document to marked students list of documents
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Du måste välja en fil eller gå tillbaka till listan.");
                        ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name", document.DocumentTypeId);
                        return View(document);
                    }
                }

                return RedirectToAction("Index");

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            return View(document);
        }

        // GET: download a file
        [HttpGet]
        public async Task<FileResult> Download(int? id)
        {
            Document document = await db.Documents.FindAsync(id);

            return File(document.Content, document.ContentType, Path.GetFileName(document.FileName));
        }

        // GET: Documents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            ViewBag.ReceiverTypeId = new SelectList(db.ReceiverTypes, "Id", "Name", document.ReceiverTypeId);

            DocumentViewModel documentViewModel = new DocumentViewModel
            {
                Id = document.Id,
                DocumentTypeId = document.DocumentTypeId,
                ReceiverTypeId = document.ReceiverTypeId,
                DocumentName = document.DocumentName,
                Description = document.Description
            };

            return View(documentViewModel);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DocumentTypeId,ReceiverTypeId,DocumentName,Description")] DocumentViewModel documentViewModel)
        {
            if (ModelState.IsValid)
            {
                Document existingDocument = await db.Documents.FindAsync(documentViewModel.Id);
                if (existingDocument != null)
                {
                    existingDocument.DocumentTypeId = documentViewModel.DocumentTypeId;
                    existingDocument.ReceiverTypeId = documentViewModel.ReceiverTypeId;
                    existingDocument.DocumentName = documentViewModel.DocumentName;
                    existingDocument.Description = documentViewModel.Description;
                }

                db.Entry(existingDocument).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            Document document = await db.Documents.FindAsync(documentViewModel.Id);
            DocumentViewModel newDocumentViewModel = new DocumentViewModel
            {
                Id = document.Id,
                DocumentTypeId = document.DocumentTypeId,
                ReceiverTypeId = document.ReceiverTypeId,
                DocumentName = document.DocumentName,
                Description = document.Description
            };

            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            ViewBag.ReceiverTypeId = new SelectList(db.ReceiverTypes, "Id", "Name", document.ReceiverTypeId);

            return View(document);
        }

        // GET: Documents/DeleteDocument/5
        public async Task<ActionResult> DeleteDocument(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/DeleteDocument/5
        [HttpPost, ActionName("DeleteDocument")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Document document = await db.Documents.FindAsync(id);
            db.Documents.Remove(document);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

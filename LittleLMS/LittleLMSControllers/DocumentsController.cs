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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
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
            return View(await db.Documents.ToListAsync());
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

            if (User.IsInRole("Lärare") || User.IsInRole("Elev"))
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);

                Document document = new Document
                {
                    UploadedByName = user.FullName
                };

                return View(document);
            }
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // http://www.mikesdotnetting.com/article/259/asp-net-mvc-5-with-ef-6-working-with-files
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadFile([Bind(Include = "Id,DocumentTypeId,FileName,Description,UploadedByName,TimeOfRegistration,Deadline,ContentType,Content,FeedbackFromTeacherToStudent")] Document document, HttpPostedFileBase upload)
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
                                FileName = Path.GetFileName(upload.FileName),
                                Description = document.Description,
                                UploadedByName = document.UploadedByName,
                                UploadedByUserId = userId,
                                TimeOfRegistration = DateTime.Now,
                                ContentType = upload.ContentType,
                                FeedbackFromTeacherToStudent = string.Empty
                            };

                            using (var binaryReader = new BinaryReader(upload.InputStream))
                            {
                                dokumentToUpload.Content = binaryReader.ReadBytes(upload.ContentLength);
                            }

                            db.Documents.Add(dokumentToUpload);
                            await db.SaveChangesAsync();

                            var dbUser = await db.Users.FirstAsync(u => u.Id == userId);
                            if (dbUser != null)
                            {
                                dbUser.UserDocuments.Add(dokumentToUpload);
                                await db.SaveChangesAsync();
                            }
                        }
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
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DocumentTypeId,FileName,Description,UploadedByName,TimeOfRegistration,Deadline,ContentType,Content,FeedbackFromTeacherToStudent")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "Id", "Name", document.DocumentTypeId);
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

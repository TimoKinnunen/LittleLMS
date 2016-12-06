using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
    using System.Data.Entity;

    [Authorize(Roles = "Lärare,Elev")]
    public class DocumentTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DocumentTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.DocumentTypes.ToListAsync());
        }

        // GET: DocumentTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documentType = await db.DocumentTypes.FindAsync(id);
            if (documentType == null)
            {
                return HttpNotFound();
            }
            return View(documentType);
        }

        // GET: DocumentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] DocumentType documentType)
        {
            if (ModelState.IsValid)
            {
                db.DocumentTypes.Add(documentType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(documentType);
        }

        // GET: DocumentTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documentType = await db.DocumentTypes.FindAsync(id);
            if (documentType == null)
            {
                return HttpNotFound();
            }
            return View(documentType);
        }

        // POST: DocumentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] DocumentType documentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(documentType);
        }

        // GET: DocumentTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documentType = await db.DocumentTypes.FindAsync(id);
            if (documentType == null)
            {
                return HttpNotFound();
            }
            return View(documentType);
        }

        // POST: DocumentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DocumentType documentType = await db.DocumentTypes.FindAsync(id);
            db.DocumentTypes.Remove(documentType);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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

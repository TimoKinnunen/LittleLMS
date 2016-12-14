using LittleLMS.LittleLMSModels;
using LittleLMS.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LittleLMS.LittleLMSControllers
{
    using System.Data.Entity;
    public class ReceiverTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReceiverTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ReceiverTypes.ToListAsync());
        }

        // GET: ReceiverTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiverType receiverType = await db.ReceiverTypes.FindAsync(id);
            if (receiverType == null)
            {
                return HttpNotFound();
            }
            return View(receiverType);
        }

        // GET: ReceiverTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReceiverTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ReceiverType receiverType)
        {
            if (ModelState.IsValid)
            {
                db.ReceiverTypes.Add(receiverType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(receiverType);
        }

        // GET: ReceiverTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiverType receiverType = await db.ReceiverTypes.FindAsync(id);
            if (receiverType == null)
            {
                return HttpNotFound();
            }
            return View(receiverType);
        }

        // POST: ReceiverTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ReceiverType receiverType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receiverType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(receiverType);
        }

        // GET: ReceiverTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiverType receiverType = await db.ReceiverTypes.FindAsync(id);
            if (receiverType == null)
            {
                return HttpNotFound();
            }
            return View(receiverType);
        }

        // POST: ReceiverTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ReceiverType receiverType = await db.ReceiverTypes.FindAsync(id);
            db.ReceiverTypes.Remove(receiverType);
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

namespace CourseManagement.Web.Controllers
{
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Model;
    using Persistence;

    public class AccountsController : Controller
    {
        private CourseManagementContext db = new CourseManagementContext();

        //
        // GET: /Accounts/

        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        //
        // GET: /Accounts/Details/5

        public ActionResult Details(int id = 0)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // GET: /Accounts/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Accounts/Create

        [HttpPost]
        public ActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        //
        // GET: /Accounts/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        //
        // GET: /Accounts/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        //
        // POST: /Accounts/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
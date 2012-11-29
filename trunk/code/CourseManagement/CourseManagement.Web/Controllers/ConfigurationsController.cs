namespace CourseManagement.Web.Controllers
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using Model;
    using Persistence;

    public class ConfigurationsController : Controller
    {
        private CourseManagementContext db = new CourseManagementContext();

        //
        // GET: /Configurations/

        public ActionResult Index()
        {
            var configurations = db.Configurations.Include(c => c.Account);
            return View(configurations.ToList());
        }

        //
        // GET: /Configurations/Details/5

        public ActionResult Details(string id = null)
        {
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        //
        // GET: /Configurations/Create

        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User");
            return View();
        }

        //
        // POST: /Configurations/Create

        [HttpPost]
        public ActionResult Create(Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Configurations.Add(configuration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", configuration.AccountId);
            return View(configuration);
        }

        //
        // GET: /Configurations/Edit/5

        public ActionResult Edit(string id = null)
        {
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", configuration.AccountId);
            return View(configuration);
        }

        //
        // POST: /Configurations/Edit/5

        [HttpPost]
        public ActionResult Edit(Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configuration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", configuration.AccountId);
            return View(configuration);
        }

        //
        // GET: /Configurations/Delete/5

        public ActionResult Delete(string id = null)
        {
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        //
        // POST: /Configurations/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            Configuration configuration = db.Configurations.Find(id);
            db.Configurations.Remove(configuration);
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
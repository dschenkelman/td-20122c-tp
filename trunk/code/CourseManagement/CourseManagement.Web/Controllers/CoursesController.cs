namespace CourseManagement.Web.Controllers
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using Model;
    using Persistence;

    public class CoursesController : Controller
    {
        private CourseManagementContext db = new CourseManagementContext();

        //
        // GET: /Courses/

        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Subject).Include(c => c.Account);
            return View(courses.ToList());
        }

        //
        // GET: /Courses/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // GET: /Courses/Create

        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(db.Subjects, "Code", "Name");
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User");
            return View();
        }

        //
        // POST: /Courses/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SubjectId = new SelectList(db.Subjects, "Code", "Name", course.SubjectId);
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", course.AccountId);
            return View(course);
        }

        //
        // GET: /Courses/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectId = new SelectList(db.Subjects, "Code", "Name", course.SubjectId);
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", course.AccountId);
            return View(course);
        }

        //
        // POST: /Courses/Edit/5

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SubjectId = new SelectList(db.Subjects, "Code", "Name", course.SubjectId);
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "User", course.AccountId);
            return View(course);
        }

        //
        // GET: /Courses/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Courses/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
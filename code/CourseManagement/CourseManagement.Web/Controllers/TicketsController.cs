namespace CourseManagement.Web.Controllers
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using Model;
    using Persistence;

    public class TicketsController : Controller
    {
        private CourseManagementContext db = new CourseManagementContext();

        //
        // GET: /Tickets/

        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedTeacher).Include(t => t.Creator);
            return View(tickets.ToList());
        }

        //
        // GET: /Tickets/Details/5

        public ActionResult Details(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // GET: /Tickets/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name", ticket.TeacherId);
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name", ticket.StudentId);
            return View(ticket);
        }

        //
        // POST: /Tickets/Edit/5

        [HttpPost]
        public ActionResult Edit(Ticket updatedTicket)
        {
            if (ModelState.IsValid)
            {
                var existingTicket = db.Tickets.Find(updatedTicket.Id);
                existingTicket.TeacherId = updatedTicket.TeacherId;
                db.Entry(existingTicket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Name", updatedTicket.TeacherId);
            ViewBag.StudentId = new SelectList(db.Students, "Id", "Name", updatedTicket.StudentId);
            return View(updatedTicket);
        }

        //
        // GET: /Tickets/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // POST: /Tickets/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
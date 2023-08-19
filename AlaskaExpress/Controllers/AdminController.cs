using AlaskaExpress.Models;
using System.Linq;
using System.Web.Mvc;

namespace AlaskaExpress.Controllers
{
    public class AdminController : Controller
    {
        private AlaskaExpressEntities db = new AlaskaExpressEntities();

        public ActionResult Index()
        {
            return View(db.Admins.ToList());
        }

        public ActionResult ManagerList()
        {
            return View(db.Managers.ToList());
        }

        public ActionResult CreateManager()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateManager([Bind(Include = "Manager_email,Manager_password,Manager_fullname,Manager_address,Manager_nid,Manager_phone,Manager_addedby")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Managers.Add(manager);
                db.SaveChanges();
                return RedirectToAction("ManagerList");
            }

            return View();
        }

        public ActionResult DeleteManager()
        {
            ViewBag.managers = db.Managers.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult DeleteManager(string Manager_email)
        {
            Manager manager = db.Managers.Where(temp => temp.Manager_email == Manager_email).FirstOrDefault();
            db.Managers.Remove(manager);
            db.SaveChanges();
            ViewBag.managers = db.Managers.ToList();
            return View();
        }


        public ActionResult SellerList()
        {
            return View(db.Sellers.ToList());
        }

        public ActionResult CustomerList()
        {
            return View(db.Customers.ToList());
        }

        public ActionResult BusList()
        {
            return View(db.Buses.ToList());
        }

        public ActionResult ScheduleList()
        {
            return View(db.Schedules.ToList());
        }

        public ActionResult TicketList()
        {
            return View(db.Tickets.ToList());
        }
    }
}

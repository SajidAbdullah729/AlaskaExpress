using AlaskaExpress.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AlaskaExpress.Controllers
{
    public class SellerController : Controller
    {
        private AlaskaExpressEntities db = new AlaskaExpressEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BusList()
        {
            return View(db.Buses.ToList());
        }

        public ActionResult ScheduleList()
        {
            return View(db.Schedules.ToList());
        }

        public ActionResult AddSchedule(long inputBusIdForAddSchedule, string inputTimeForAddSchedule, string inputDateForAddSchedule)
        {
            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {
                System.Data.SqlClient.SqlConnection con = new SqlConnection(@"Data Source=MEGATRONM609\SQLEXPRESS;Initial Catalog=AlaskaExpress; Integrated Security=True");
                SqlCommand sql;
                con.Open();

                string userEmail = (string)Session["userEmail"];

                sql = new SqlCommand("INSERT INTO Schedule(Bus_journey_time,Bus_journet_day,Bus_id,Schedule_addedby) VALUES('" + inputTimeForAddSchedule + "','" + inputDateForAddSchedule + "', " + inputBusIdForAddSchedule + ", '" + userEmail + "')", con);
                sql.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("ScheduleList", "Seller");
            }
        }

        public ActionResult TicketList()
        {
            var sql = "SELECT * FROM Ticket WHERE Ticket_state='0'";
            List<Ticket> unconfirmedTicket = db.Tickets.SqlQuery(sql).ToList();

            return View(unconfirmedTicket); 
        }

        public ActionResult TicketConfirm(long ticket_id)
        {

            System.Data.SqlClient.SqlConnection con = new SqlConnection(@"Data Source=MEGATRONM609\SQLEXPRESS;Initial Catalog=AlaskaExpress; Integrated Security=True");
            SqlCommand sql;
            con.Open();

            sql = new SqlCommand("UPDATE Ticket SET Ticket_state=1 WHERE Ticket_id = " + ticket_id + "", con);
            sql.ExecuteNonQuery();
            con.Close();

            var sql2 = "SELECT * FROM Ticket WHERE Ticket_state='0'";
            List<Ticket> unconfirmedTicket = db.Tickets.SqlQuery(sql2).ToList();

            return View("TicketList", unconfirmedTicket);
        }

        public ActionResult UserProfile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            ViewBag.Seller_addedby = new SelectList(db.Managers, "Manager_email", "Manager_password", seller.Seller_addedby);
            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile([Bind(Include = "Seller_email,Seller_password,Seller_fullname,Seller_address,Seller_nid,Seller_phone,Seller_addedby")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Seller_addedby = new SelectList(db.Managers, "Manager_email", "Manager_password", seller.Seller_addedby);
            return View("Index");
        }
    }
}

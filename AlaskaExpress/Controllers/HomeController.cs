using AlaskaExpress.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;


namespace AlaskaExpress.Controllers
{
    public class HomeController : Controller
    {
        private AlaskaExpressEntities db = new AlaskaExpressEntities();

        public ActionResult Index()
        {
            var sql = "SELECT * FROM Schedule";
            List<Schedule> searchedBus = db.Schedules.SqlQuery(sql).ToList();

            List<string> startLocation = new List<string>();
            List<string> endLocation = new List<string>();

            foreach (var item in searchedBus)
            {
                if (!startLocation.Contains(item.Bus.Bus_start_location))
                {
                    startLocation.Add(item.Bus.Bus_start_location);
                }

                if (!endLocation.Contains(item.Bus.Bus_end_location))
                {
                    endLocation.Add(item.Bus.Bus_end_location);
                }
            }

            ViewBag.startLocation = startLocation;
            ViewBag.endLocation = endLocation;
            return View();
        }

        public ActionResult SearchedBus(string inputJourneyFrom, string inputJourneyTo, string inputJourneyDate)
        {
            var sql = "SELECT * FROM Schedule INNER JOIN Bus ON Schedule.Bus_id = Bus.Bus_id WHERE Bus_start_location= '" + inputJourneyFrom + "' AND Bus_end_location='" + inputJourneyTo + "' AND Bus_journet_day='" + inputJourneyDate + "' ";
            List<Schedule> busDetails = db.Schedules.SqlQuery(sql).ToList();

            if (busDetails.Count != 0)
            {
                return View("SearchedBus", busDetails);
            }
            else
            {
                Response.Write("<script>alert('No bus found');</script>");

                if (Session["userEmail"] != null)
                {
                    return View(Session["userEmail"]);
                }
                else
                {
                    var sql2 = "SELECT * FROM Schedule";
                    List<Schedule> searchedBus = db.Schedules.SqlQuery(sql2).ToList();

                    List<string> startLocation = new List<string>();
                    List<string> endLocation = new List<string>();

                    foreach (var item in searchedBus)
                    {
                        if (!startLocation.Contains(item.Bus.Bus_start_location))
                        {
                            startLocation.Add(item.Bus.Bus_start_location);
                        }

                        if (!endLocation.Contains(item.Bus.Bus_end_location))
                        {
                            endLocation.Add(item.Bus.Bus_end_location);
                        }
                    }

                    ViewBag.startLocation = startLocation;
                    ViewBag.endLocation = endLocation;
                    return View("Index");
                }
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (Session["userEmail"] != null)
            {
                Response.Write("<script>alert('Already logged in.');</script>");
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult KillSession()
        {
            Session.RemoveAll();
            return RedirectToAction("", "Home");
        }

        [HttpPost]
        public ActionResult AuthorizeLogin(string inputEmailForSignin, string inputPasswordForSignin)
        {
            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {
                var adminDetails = db.Admins.Where(user => user.Admin_email == inputEmailForSignin && user.Admin_password == inputPasswordForSignin).FirstOrDefault();
                var managerDetails = db.Managers.Where(user => user.Manager_email == inputEmailForSignin && user.Manager_password == inputPasswordForSignin).FirstOrDefault();
                var sellerDetails = db.Sellers.Where(user => user.Seller_email == inputEmailForSignin && user.Seller_password == inputPasswordForSignin).FirstOrDefault();
                var customerDetails = db.Customers.Where(user => user.Customer_email == inputEmailForSignin && user.Customer_password == inputPasswordForSignin).FirstOrDefault();

                if (adminDetails != null)
                {
                    Session["userEmail"] = adminDetails.Admin_email;
                    Session["userRole"] = "Admin";
                    return RedirectToAction("Index", "Admin");
                }
                else if (managerDetails != null)
                {
                    Session["userEmail"] = managerDetails.Manager_email;
                    Session["userRole"] = "Manager";
                    return RedirectToAction("Index", "Manager");
                }
                else if (sellerDetails != null)
                {
                    Session["userEmail"] = sellerDetails.Seller_email;
                    Session["userRole"] = "Seller";
                    return RedirectToAction("Index", "Seller");
                }
                else if (customerDetails != null)
                {
                    Session["userEmail"] = customerDetails.Customer_email;
                    Session["userRole"] = "Customer";
                    var rr = Request.UrlReferrer.ToString();

                    try
                    {
                        if (!string.IsNullOrEmpty(rr) && !Equals(rr, "Home/Index") && !Equals(rr, "Index") && !Equals(rr, "~/Views/Home/Index.cshtml") && !FF(rr, "SearchedBus") && !FF(rr, "Index") && !FF(rr, "Home"))
                        {
                            return Redirect(rr);
                        }
                        else
                            return RedirectToAction("Index", "Customer");
                    }
                    catch (Exception e1)
                    {
                        Console.WriteLine("Exception caught: {0}", e1);
                        return RedirectToAction("Index", "Customer");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Incorrect Email or Password');</script>");
                    return View("Login");
                }
            }
        }

        public ActionResult AuthorizeSignup(string inputFullnameForSignup, string inputPhoneForSignup, string inputDobForSignup, string inputNidForSignup, string inputAddressForSignup, string inputEmailForSignup, string inputPasswordForSignup)
        {
            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {
                var customerDetails = db.Customers.Where(user => user.Customer_email == inputEmailForSignup).FirstOrDefault();

                if (customerDetails != null)
                {
                    Response.Write("<script>alert('Email already exists.');</script>");
                    return View("Login");
                }
                else if (customerDetails == null)
                {
                    System.Data.SqlClient.SqlConnection con = new SqlConnection(@"Data Source=MEGATRONM609\SQLEXPRESS;Initial Catalog=AlaskaExpress; Integrated Security=True");
                    SqlCommand sql;
                    con.Open();

                    sql = new SqlCommand("INSERT INTO Customer VALUES('" + inputEmailForSignup + "','" + inputPasswordForSignup + "','" + inputFullnameForSignup + "','" + inputDobForSignup + "','" + inputAddressForSignup + "', '" + inputPhoneForSignup + "', '" + inputNidForSignup + "')", con);
                    sql.ExecuteNonQuery();
                    con.Close();

                    var customerDetails2 = db.Customers.Where(user => user.Customer_email == inputEmailForSignup && user.Customer_password == inputPasswordForSignup).FirstOrDefault();

                    Session["userEmail"] = customerDetails2.Customer_email;
                    Session["userRole"] = "Customer";
                    return RedirectToAction("Index", "Customer");
                }
            }

            return RedirectToAction("Signup", "Home");
        }

        [NonAction]
        public bool FF(string s, string s2)
        {
            int i = 0;
            int len = s2.Length;
            for (; i < s.Length; i++)
            {
                if (i + len >= s.Length) return false;
                if (Equals(s.Substring(i, len), s2)) return true;
            }
            return false;
        }
    }
}
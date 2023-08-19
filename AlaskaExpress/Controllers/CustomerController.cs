using AlaskaExpress.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AlaskaExpress.Controllers
{
    public class CustomerController : Controller
    {
        private AlaskaExpressEntities db = new AlaskaExpressEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindBus()
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
                return View("FindBus");
            }
        }

        public ActionResult TicketPending()
        {
            return View();
        }

        public ActionResult TicketComplete(string scheduleID, string selectedAllSeatsforTicket, string txnId, string calculatedTotalprice)
        {
            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {
                System.Data.SqlClient.SqlConnection con = new SqlConnection(@"Data Source=MEGATRONM609\SQLEXPRESS;Initial Catalog=AlaskaExpress; Integrated Security=True");
                SqlCommand sql;
                con.Open();

                string userEmail = (string)Session["userEmail"];

                sql = new SqlCommand("INSERT INTO Ticket (Bus_seats,Schedule_id,Customer_email,Total_price,TXN_id,Ticket_state) VALUES('" + selectedAllSeatsforTicket + "'," + scheduleID + ",'" + userEmail + "'," + calculatedTotalprice + ",'" + txnId + "', "+0+")", con);
                sql.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MyTickets", "Customer");  
            }
        }

        public ActionResult MyTickets()
        {
            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {

                var sql = "SELECT * FROM Ticket WHERE Customer_email = '" + Session["userEmail"] + "'";
                List<Ticket> ticketDetails = db.Tickets.SqlQuery(sql).ToList();

                return View(ticketDetails);
            }
        }

        public ActionResult TicketDownload(long ticketId)
        {
            string ticket_id = ticketId.ToString();

            using (AlaskaExpressEntities db = new AlaskaExpressEntities())
            {
                var ticketValues = db.Tickets.Where(user => user.Ticket_id == ticketId).FirstOrDefault();

                string cusName = ticketValues.Customer.Customer_fullname;

                using (PdfDocument document = new PdfDocument())
                {
                    //Generate a new PDF document
                    //Adds page settings
                    document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                    document.PageSettings.Margins.All = 50;
                    //Adds a page to the document
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    PdfFont fontHead = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
                    PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
                    //Loads the image from disk
                    // PdfImage image = PdfImage.FromFile(Server.MapPath("~/AdventureCycle.jpg"));
                    RectangleF bounds = new RectangleF(176, 0, 390, 0);
                    //Draws the image to the PDF page
                    //page.Graphics.DrawImage(image, bounds);

                    graphics.DrawString("AlaskaExpress", fontHead, PdfBrushes.Blue, new PointF(300, 50));
                    PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
                    bounds = new RectangleF(0, bounds.Bottom + 90, graphics.ClientSize.Width, 30);
                    //Draws a rectangle to place the heading in that region.
                    graphics.DrawRectangle(solidBrush, bounds);
                    //Creates a font for adding the heading in the page
                    PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
                    //Creates a text element to add the invoice number
                    PdfTextElement element = new PdfTextElement("TICKET NO: " + ticket_id, subHeadingFont);
                    element.Brush = PdfBrushes.White;

                    //Draws the heading on the page
                    PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
                    string currentDate = "DATE " + DateTime.Now.ToString("MM/dd/yyyy");
                    //Measures the width of the text to place it in the correct location
                    SizeF textSize = subHeadingFont.MeasureString(currentDate);
                    PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                    //Draws the date by using DrawString method
                    graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
                    PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
                    //Creates text elements to add the address and draw it to the page.

                    element = new PdfTextElement("Passenger's Info ", fontHead);
                    element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
                    result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 20));

                    element = new PdfTextElement("Customer Name: " + ticketValues.Customer.Customer_fullname, font);
                    result = element.Draw(page, new PointF(10, result.Bounds.Bottom));
                    element = new PdfTextElement("Phone: " + ticketValues.Customer.Customer_phone, font);
                    result = element.Draw(page, new PointF(10, result.Bounds.Bottom));
                    element = new PdfTextElement("Email: " + ticketValues.Customer.Customer_email, font);
                    result = element.Draw(page, new PointF(10, result.Bounds.Bottom));


                    element = new PdfTextElement("Booking Info ", fontHead);
                    element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
                    result = element.Draw(page, new PointF(350, result.Bounds.Top - 60));

                    element = new PdfTextElement("Bus: " + ticketValues.Schedule.Bus.Bus_numberplate, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));
                    element = new PdfTextElement("From: " + ticketValues.Schedule.Bus.Bus_start_location, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));
                    element = new PdfTextElement("To: " + ticketValues.Schedule.Bus.Bus_end_location, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));
                    element = new PdfTextElement("Date: " + ticketValues.Schedule.Bus_journet_day, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));
                    element = new PdfTextElement("Time: " + ticketValues.Schedule.Bus_journey_time, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));
                    element = new PdfTextElement("Seats: " + ticketValues.Bus_seats, font);
                    result = element.Draw(page, new PointF(350, result.Bounds.Bottom));

                    PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
                    PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
                    PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3);
                    //Draws a line at the bottom of the address
                    graphics.DrawLine(linePen, startPoint, endPoint);
                    string ticketName = "Ticket" + ticket_id + ".pdf";
                    // Open the document in browser after saving it
                    document.Save(ticketName, HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                }
            }
            var sql = "SELECT * FROM Ticket WHERE Customer_email = '" + Session["userEmail"] + "'";
            List<Ticket> ticketDetails = db.Tickets.SqlQuery(sql).ToList();

            return View("MyTickets", ticketDetails);
        }

        public ActionResult UserProfile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile([Bind(Include = "Customer_email,Customer_password,Customer_fullname,Customer_dob,Customer_address,Customer_phone,Customer_nid")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}

using CustomComputers.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer = CustomComputers.Models.Customer;

namespace CustomComputers.Controllers
{
    public class StripeController : Controller
    {

        //this displayes the stripe payment view
        //it sends StripePublishKey information to the front allong with the order total and order Id
        public ActionResult Index(CardPayment p)
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];

            ViewBag.Total = p.Total;
            ViewBag.OrderId = p.OrderId;
            return View();
        }

        //this is the POST for the stripe Payment View
       
        
        [HttpPost]
        public ActionResult Charge(string stripeToken, string stripeEmail)
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (Request["OrderId"] != null)
                {
                    int orderId = int.Parse(Request["orderId"].ToString());  //it requests an order Id from the front
                    CustomerOrder o = db.Orders.Find(orderId); //the order is found in the database by its Id 

                    //the orders properties are adjusted appropriately
                    o.Payment = true;// the order is set to 'Paid'
                    o.Pay(o.Id);
                    //the next block finds each product in the order and adjusts their stock levels
                    o.BasketItems = o.GetItems(o.Id);
                    foreach (Basket b in o.BasketItems)
                    {
                        b.GetProduct();
                        int stock = b.Product.Stock - b.Quantity;

                        o.SetStock(b.Product.Id, stock);
                    }

                    //to follow are all the properties of a strip payment
                   
                    Stripe.StripeConfiguration.SetApiKey("pk_test_veUj82xPax3gnKyiqk9losrd00QfIrd1o2");
                    Stripe.StripeConfiguration.ApiKey = "sk_test_wHoSwq3TOM5ZJkaKQrG9GWjo00uhPcOhUa";

                    var myCharge = new Stripe.ChargeCreateOptions();
                    // always set these properties
                    myCharge.Amount = (long.Parse(Request["Total"].ToString())); //the total is requested from the front
                    myCharge.Currency = "gbp";      //currency
                    myCharge.ReceiptEmail = stripeEmail;
                    myCharge.Description = "Sample Charge";
                    myCharge.Source = stripeToken;
                    myCharge.Capture = true;
                    var chargeService = new Stripe.ChargeService();
                    Charge stripeCharge = chargeService.Create(myCharge);
                    return View();
                }
                //this will redirect to the ViewBasket page if the order id is not found for any reason
                else
                {
                    return RedirectToAction("ViewBasket, User");
                }
            }
            //this is a failsafe to send users to their order page if the abouv code doesnt execute properly
            catch
            {
                return RedirectToAction("SeeOrders, Order");
            }
        }
    }
}

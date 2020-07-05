using CustomComputers.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComputers.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order

        private ApplicationDbContext db = new ApplicationDbContext();


        //this controller checks if a user is logged in
        //the logged in users role is checked and the controller redirects accordingly
        [Authorize(Roles = "Customer, Manager, Staff")]
        public ActionResult SeeOrders()
        {
            string userId = User.Identity.GetUserId();
            User u = db.Users.Find(userId);

            Customer c;
            
            if (u == null)//if no logged in user is found it redirects to the login page
            {
                return RedirectToAction("Login", "Account");
            }
            else if (u.Role.Equals("Customer"))//if the logged in user is a customer the CustomerOrders controller is called
            {
                c = (Customer)u;
                return RedirectToAction("CustomerOrders", "Order", c);//the customer object is passed to the controller
            }
            else if (u.Role.Equals("Staff") || u.Role.Equals("Manager"))//if the users role is 'Staff' or 'Manager' it redirects to the ViewAllOrders controller
            {
               
                return RedirectToAction("ViewAllOrders", "Order");
            }

            return View();
        }


        [Authorize(Roles = "Customer")]
        public ActionResult CustomerOrders(Customer c)
        {
           

            
            foreach(CustomerOrder o in db.Orders)//this loops through each order in the database
            {
                if (o.Payment==true && o.CustomerId==c.Id)//if the order is paid and it has the same customerId as the logged in customer it is added to the Orders list in the customer object
                {
                    c.Orders.Add(o);
                    o.BasketItems = o.GetItems(o.Id);
                    foreach (Basket b in o.BasketItems)
                    {
                        b.GetProduct();// each orderline gets a product object
                    }
                }

                
            }

            ViewBag.Customer = c;//the logged in customer is sent to the front

            return View();
        }


        //this controller deals with the buttons pressed in the CustomerOrders page
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public ActionResult ProcessCustomerOrders()
        {
            //if the 'Cancel Order' button is pressed 
            //the order to be canceled is found in the database and deleted
            if(Request["Cancel"]!=null)
            {
                int orderId = int.Parse(Request["orderId"].ToString());//this requests the order id from the front
                CustomerOrder o = db.Orders.Find(orderId);//this finds the order in the database
                o.deleteBaskets(o.Id);//this deletes the orderlines affiliated wiht that order from the database
                db.Orders.Remove(o);//this deletes the order
                db.SaveChanges();
            }

            return RedirectToAction("SeeOrders");
        }


        //this returns the ViewAllorders View
        //it sends a list of orders each with a list of orderlines
        [Authorize(Roles = "Manager, Staff")]
        public ActionResult ViewAllOrders()
        {
            List <CustomerOrder> orders = db.Orders.ToList();//the list of order from the database
            

            foreach (CustomerOrder o in orders)
            {
                o.GetCustomer = (Customer)db.Users.Find(o.CustomerId);//this sets the getCustomer property for each order
                o.BasketItems = o.GetItems(o.Id);
                foreach (Basket b in o.BasketItems)//this gets products for each orderline in the order
                {
                    b.GetProduct();
                }

            }


            
            ViewBag.Orders = orders;//sends list do front
            return View();
        }

        //this controlled deals with buttons pressed on the ViewAllOrders Page

        [Authorize(Roles = "Manager, Staff")]
        [HttpGet]
        public ActionResult ProcessViewAllOrders()
        {
            //if the 'deleteItem' button is pressed this if statement will execute
            if (Request["DeleteItem"] != null)
            {
                int basketId = int.Parse(Request["basketId"].ToString());//this requests the orderline Id from the front
                Basket b = db.Baskets.Find(basketId);//this finds the orderline in the database by its id
                db.Baskets.Remove(b);//this removes the orderline from the database
                db.SaveChanges();
            }
            //if the 'Cancel Order' button is pressed
            

            if (Request["Cancel"] != null)
            {
                int orderId = int.Parse(Request["orderId"].ToString());//the order Id is requested from the front
                CustomerOrder o = db.Orders.Find(orderId);//the order is found in the database by its Id
                o.deleteBaskets(o.Id);//the orderlines aer deleted by the order Id
                db.Orders.Remove(o);// the order is deleted
                db.SaveChanges();
            }
            if (Request["Checkout"] != null)// if the checkout button is pressed the user is directed to the Stripe checkout page
            {
                int orderId = int.Parse(Request["orderId"].ToString());//this requests the order Id from the front
                CustomerOrder o = db.Orders.Find(orderId); //the Order is found in the database by its Id

                o.BasketItems = o.GetItems(o.Id);//this fills the Basket items list 
                List<Product> products = db.Products.ToList();//a list of products from the database

                //the next block of code checks to see if customers have ordered more than stock levels can facilitate
                //it checks each orderlines quantity against the matching products stock level
                //if an order is too large, a redirect happens
                foreach (Basket b in o.BasketItems)
                {

                    foreach (Product p in products)
                    {
                        if (b.ProductId == p.Id)
                        {
                            if (b.Quantity > p.Stock)
                            {
                                return RedirectToAction("OutOfStock", "User", p);
                            }
                        }
                    }
                }

                return RedirectToAction("Index", "Stripe", new { OrderId = orderId, Total = o.Total }); //redirected to the stripe index page allong with a new CardPayment object

            }

            return RedirectToAction("ViewAllOrders"); //Redirection to the ViewAllOrders ActionResult
        }

      
    }
    

}
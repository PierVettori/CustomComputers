using CustomComputers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PayPal.Api;
using System.Configuration;

namespace CustomComputers.Controllers
{
    public class UserController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        //this displays the ViewBasket page
        //first it checks if a user is logged in and casts them as a customer
        //if no user is logged in it redirects to a login page
        //if a user is logged in it finds an unpaid order for that customer
        //if there is an unpaid order, that order is populated with orderlines and those orderlines with products
        //the fully fledged order object is sent to the front 
        [Authorize(Roles = "Customer")]
        public ActionResult ViewBasket()
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            string userId = User.Identity.GetUserId();
            Customer cust = (Customer)db.Users.Find(userId);
           
            
            if(cust==null)
            {
               
                return RedirectToAction("Login", "Account");
               
            }

            CustomerOrder o = cust.getOrder(cust.Id);
            
            if (o != null)
            {
                o.BasketItems = o.GetItems(o.Id);

                foreach (Basket b in o.BasketItems)
                {
                    b.GetProduct();
                }

               
            }
          
            ViewBag.Order = o;





            return View();
        }


        //this controller deals with buttons pressed on the view Basket page
        
        [HttpGet]
        public ActionResult ProcessBasket()
        {
            string userId = User.Identity.GetUserId();
            Customer cust = (Customer)db.Users.Find(userId);

            if (Request["RemoveProduct"] != null)//this section of code executes if the 'Remove product' button is pressed
            {
                int basketId = int.Parse(Request["basketId"].ToString());//this requests the orderline Id from the front

                Basket b = db.Baskets.Find(basketId);//this finds the orderline in the dtatbase by its Id

                CustomerOrder o = db.Orders.Find(b.CustomerOrderId);//this finds the Order from the database by its Id
                o.Total = o.Total - b.Quantity * b.GetProduct().Price;//this sets the new total fot the order

                db.Baskets.Remove(b);
                db.SaveChanges();

            }


            //if the 'Checkout button is pressed the following code will execute
            if (Request["Checkout"] != null)
            {
                
                CustomerOrder o = cust.getOrder(userId);//this gets the customers unpaid order from the database
                o.BasketItems=o.GetItems(o.Id);//this fills the Basket items list 
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
                            if(b.Quantity > p.Stock)
                            {
                                return RedirectToAction("OutOfStock", "User", p);
                            }
                        }
                    }
                }

                    
                    return RedirectToAction("Index","Stripe", new { OrderId = o.Id, Total =o.Total });//redirect to 'Index' of the stripe controller with a new CardPayment object
                
            }

            return RedirectToAction("ViewBasket");
        }



        //this controller is envoked when the greeting message on the navbar is clicked
        //The identity of the User is determined and then redirected to an appropriate controller, based on the users role
        //the user is also casted to its role and passed as aparameter to the appropriate action
        [Authorize(Roles = "Customer, Manager, Staff, StockController") ]
        public ActionResult EditUser()
        {
            string userId = User.Identity.GetUserId();//this grabs the User Id
            User u = db.Users.Find(userId);//this finds the user in the Database
            Customer c;
            Staff s;
            Manager m;
            StockController sc;

            
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }

            else if(u.Role.Equals("Customer"))
            {
                c = (Customer)u;
                return RedirectToAction("EditCustomer", "User", c);
            }
            else if (u.Role.Equals("Staff"))
            {
                s = (Staff)u;
                return RedirectToAction("EditStaff", "User", s);
            }
            else if (u.Role.Equals("Manager"))
            {
                m = (Manager)u;
                return RedirectToAction("EditManager", "User", m);
            }
            else if (u.Role.Equals("StockController"))
            {
                sc = (StockController)u;
                return RedirectToAction("EditStockController", "User", sc);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }



            
        }
        //this returns the EditCustomer view and Passes the logged in user as a customer object to the front
        [Authorize(Roles = "Customer")]
        public ActionResult EditCustomer(Customer c)
        {
            ViewBag.Customer = c;
            return View();
        }


        //this controller deals with the form submition from the EditCustomer View
        //First a user is found in the database by the logged in user Id
        //that user is cast as a customer
        //then all of the customers properties are set to the ones submitted by the form
        //the changes are saved to the database
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public ActionResult ProcessCustomer(Customer c)
        {

            User old = db.Users.Find(c.Id);
            Customer oldCustomer = (Customer)old;
            oldCustomer.FirstName=c.FirstName;
            oldCustomer.LastName = c.LastName;
            oldCustomer.Email = c.Email;
            oldCustomer.Town = c.Town;
            oldCustomer.PostCode = c.PostCode;
            oldCustomer.PhoneNumber = c.PhoneNumber;
            oldCustomer.AddressLn1 = c.AddressLn1;
            oldCustomer.AddressLn2 = c.AddressLn2;
            
            db.SaveChanges();

            return RedirectToAction("EditCustomer","User", oldCustomer);
        }


        //this returns the EditStaff view and Passes the logged in user as a staff object to the front
        [Authorize(Roles = "Manager, Staff")]

        public ActionResult EditStaff(Staff s)
        {
            ViewBag.Staff = s;
            return View();
        }


        //this controller deals with the form submition from the EditStaff View
        //First a user is found in the database by the logged in user Id
        //that user is cast as a Staff
        //then all of the staffs properties are set to the ones submitted by the form
        //the changes are saved to the database
        [Authorize(Roles = " Manager, Staff")]
        [HttpGet]
        public ActionResult ProcessStaff(Staff s)
        {

            User old = db.Users.Find(s.Id);
            Staff oldStaff = (Manager)old;
            oldStaff.FirstName = s.FirstName;
            oldStaff.LastName = s.LastName;
            oldStaff.Email = s.Email;
            oldStaff.PostCode = s.PostCode;
            oldStaff.Town = s.Town;
            oldStaff.PhoneNumber = s.PhoneNumber;
            oldStaff.AddressLn1 = s.AddressLn1;
            oldStaff.AddressLn2 = s.AddressLn2;


            db.SaveChanges();

            return RedirectToAction("EditStaff", "User", oldStaff);
        }


        //this returns the EditManager view and Passes the logged in user as a Manager object to the front
        [Authorize(Roles = "Manager")]
        public ActionResult EditManager(Manager m)
        {
            ViewBag.Manager = m;
            return View();
        }

        //this controller deals with the form submition from the EditManager View
        //First a user is found in the database by the logged in user Id
        //that user is cast as a Manager
        //then all of the Managers properties are set to the ones submitted by the form
        //the changes are saved to the database
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult ProcessManger(Manager m)
        {
          

            User old = db.Users.Find(m.Id);
            Manager oldManager = (Manager)old;
            oldManager.FirstName = m.FirstName;
            oldManager.LastName = m.LastName;
            oldManager.Email = m.Email;
            oldManager.PostCode = m.PostCode;
            oldManager.Town = m.Town;
            oldManager.PhoneNumber = m.PhoneNumber;
            oldManager.AddressLn1 = m.AddressLn1;
            oldManager.AddressLn2 = m.AddressLn2;


            db.SaveChanges();

            return RedirectToAction("EditManager", "User", oldManager);
        }

        //this returns the EditStockController view and Passes the logged in user as a StockController object to the front
        [Authorize(Roles = "Manager, StockController")]
        public ActionResult EditStockController(StockController sc)
        {
            ViewBag.StockController = sc;
            return View();
        }


        //this controller deals with the form submition from the EditStockController View
        //First a user is found in the database by the logged in user Id
        //that user is cast as a StockController
        //then all of the StockControllers properties are set to the ones submitted by the form
        //the changes are saved to the database
        [Authorize(Roles = "Manager, StockController")]
        [HttpGet]
        public ActionResult ProcessStockController(StockController sc)
        {

            User old = db.Users.Find(sc.Id);
            StockController oldStockController = (StockController)old;
            oldStockController.FirstName = sc.FirstName;
            oldStockController.LastName = sc.LastName;
            oldStockController.Email = sc.Email;
            oldStockController.PostCode = sc.PostCode;
            oldStockController.Town = sc.Town;
            oldStockController.PhoneNumber = sc.PhoneNumber;
            oldStockController.AddressLn1 = sc.AddressLn1;
            oldStockController.AddressLn2 = sc.AddressLn2;


            db.SaveChanges();

            return RedirectToAction("EditStockController", "User", oldStockController);
        }


        //this returns the ChangeUserRoles view and deals with button interaction on that page
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult ChangeUserRoles()
        {

            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));//an instance of the usermanager is made

            if (Request["UserRole"] != null)
            {
                //these next two strings request the user Id and their role respectively
                string userId = Request["userId"];
                string role = Request["role"];

                User user = db.Users.Find(userId);//this dips into the database to find the complete User object by its Id requested above

                user.Role = role;//this sets the user to its new role

               
                userManager.RemoveFromRoles(user.Id, user.Role);
                userManager.AddToRole(user.Id, role);//this add the user to the role
                
                db.SaveChanges();
            }
            return View();
        }

        public ActionResult OutOfStock(Product p)
        {
            ViewBag.Prod = p;
            return View();
        }


        }

    }


    

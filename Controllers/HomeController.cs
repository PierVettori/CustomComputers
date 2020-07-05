using CustomComputers.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CustomComputers.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //This is the controller for the HomePage of the application
        //its main function is to display products
        //Users can search for individual products or browse by category of product
        //

     
        [HttpGet]
        public ActionResult Index()
        {
            // 

            List<Product> products = db.Products.ToList(); //this is a list of products from the Database
            
            List<ProductCategory> categories = db.ProductCategories.ToList(); // a list of cateories from the Database
            List<Product> found = new List<Product>();  // a list of products to be populated with products that relate to user input


            //this next block of code populates a list of strings with all the product names in the database
            //this list is then sent to the front in the viewBag
            //this list is used by the predictive search bar
            List<string> autoFillProducts = new List<string>();
            foreach (Product u in products)
            {
                autoFillProducts.Add(u.Name);
            }

            ViewBag.AutoFill = autoFillProducts;



            //if a user presses the search button this next if statement will execute
            //first the database is searched for products that match the search criteria using wildcards
            //for each fond product a review object relating to that products id is attatched to the Review (List) property of the Product
            //these products are then sent to the front in the ViewBag
            if (Request["btnSearch"] != null)
            {

                string search = Request["txtSearch"];

                string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

                sqlCon.Open();
                string commandString = "SELECT Id FROM Products WHERE Name LIKE '%" + search + "%'  ";

                SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
                SqlDataReader reader = sqlCmd.ExecuteReader();


                while (reader.Read())
                {
                    int id = (int)reader["Id"];

                    Product p = new Product(id);

                    Review r = new Review(p.Id);
                      
                    p.Reviews.Add(r);
                        

                     found.Add(p);
                    
                }

                reader.Close();
                sqlCon.Close();

                
                ViewBag.Products = found;

            }
            //if the search bar is empty all the products in the database get reviews attatched and are sent to the front in this next esle statement
            //this else statement will also execute if the "All Products" button is pressed
            else
            {
                foreach (Product p in products)
                {
                    Review r = new Review(p.Id);
                    p.Reviews.Add(r);
                }

                ViewBag.Products = products;
            }


            //this executes if a user selects a category of product to browse in
            //a list of products is populated by products that match the category selected by the user
            if (Request["catagotySearch"]!=null)
            {
                string cat = Request["catagotySearch"];//this requests a string that is the name of the category of product to display

                foreach(Product p in products)
                {
                    if(p.Category.Name.Equals(cat))
                    {
                        found.Add(p);
                    }
                }

                ViewBag.Products = found;
            }


            ViewBag.Cat = categories;
           


            return View();
        }


        //this returns the About view which describes what custome Computers does
        public ActionResult About()
        {
           

            return View();
        }

        //this returns the contact view
        public ActionResult Contact()
        {

            
            return View();
        }

       

        //this controller is is envoked to deal with user input from the ViewProduct page
        //it has three main functions: Adding a product to a customers basket, adding a product to the logged in users basket and creating a review for a product

        [HttpGet]
        public ActionResult ProcessProduct(Product p)
        {
            //this actionResult has a product in as a perameter. this is the Product that would like to be added to an order
            // the next three lines find the logged in user and casts them as a customer
            string userId = User.Identity.GetUserId(); 
            User user = db.Users.Find(userId);
            Customer customer = (Customer)user;


            //this next if statement will execute when a user has written a review and selected a star rating
            //first it checks that the review comment string is not null
            //then the star rating number is checked to be present
            //a new review object is then made with the relevent information and stored to the Database
            if (Request["rating"] != null)
            {
                if (Request["rating"] != "0")
                {
                    string comment = Request["txtComment"];
                    if (comment != "Leave a Review")
                    {
                        Review review = new Review();
                        review.Comment = comment;
                        review.ProductId = p.Id;
                        review.Rating = int.Parse(Request["rating"]);
                        review.UserId = customer.Id;


                        db.Reviews.Add(review);
                        db.SaveChanges();
                    }
                }

            }

            //When the "Add to Customer Basket" button is pressed this next if statement will execute
           
            //
            if (Request["AddToCustomerBasket"] != null)
            {
                //first quantity is requested from a dropdown list in the front
                //then a customer is found in the database thta has a username corresponding to input in the front
                //if no customer is found the current logged in user is the 'customer'
                int quant = int.Parse(Request["drop"].ToString());

                string customerUserName = Request["txtCustomerSearch"];

                foreach(Customer c in db.Users)
                {
                    if(c.UserName.Equals(customerUserName))
                    {
                        customer = c;
                    }
                }

                // if no customer or logged in user is found the user wil be redirected to the login page 
                // a product object is sent along so that after login the customer is directed to the correct viewProduct 
                
                if(customer==null)
                {
                    return RedirectToAction("Login", "Account", new { id = p.Id, Name = p.Name, CategoryId = p.CategoryId, Stock = p.Stock, Price = p.Price });
                }

                //this finds the a customers latest order
                //it will return null if a customer has no unpaid orders
                CustomerOrder oldOrder = customer.getOrder(customer.Id);


                //if an unpaid order is found
                //first the order is checked to see if it already contains the product we are attempting to add to it
                // if the products already exists in the order its quantity is adjusted apropriately and then orders total is amended
                //if the product is not present in the order it is added to the order and the order total is ammended
                //if no unpaid orders are found a new one is created for the selected customer and a list of products is begun with the one in question.
                if(oldOrder.CustomerId!=null)
                {
                    int newQuant = 0;
                    int foundBaskId = 0;
                    double total = 0;
                    bool itemExists = false;
                    oldOrder.BasketItems = oldOrder.GetItems(oldOrder.Id);

                    foreach(Basket b in oldOrder.BasketItems) // this loop checks if the Product already exists in the found order
                    {
                        
                        if(b.ProductId==p.Id)
                        {
                            itemExists = true;
                            newQuant = b.Quantity + quant;
                            foundBaskId = b.Id;
                            

                        }
                        
                    }
                    //if the item exists...
                    if(itemExists==true)
                    {
                        Basket oldBasket = db.Baskets.Find(foundBaskId); //this finds the orderline in the database 


                       

                        oldOrder.dbBasket(foundBaskId, newQuant); //this sets its new quantity

                        total = (p.Price * quant) + oldOrder.Total;//this sets the total for the order
                        

                    }
                    //if the item doesnt exist in the order
                    else if(itemExists == false)
                    {
                        Basket newBasket = new Basket();// a new orderline is made
                        newBasket.dbNewBasket(oldOrder.Id, p.Id, quant);//this saves it to the database
                        

                        total = oldOrder.Total + p.Price * quant; //sets the order total
                        

                    }
                    

                    oldOrder.setTotal(oldOrder.Id, total);//saves the order total to the Database

                }
                //if no unpaid order exists...
                else if(oldOrder.CustomerId==null)
                {
                    CustomerOrder o = new CustomerOrder(); //a new order is created
                    Basket newBasket = new Basket(p.Id, quant);// a new orderline is created

                    //these all set Properties for the newly created order and orderline objects
                    o.CustomerId = customer.Id;
                    o.Payment = false;
                    o.Date = DateTime.Now;

                    newBasket.CustomerOrderId = o.Id;
                    newBasket.Order = o;

                    o.BasketItems.Add(newBasket);
                    o.Total = p.Price * quant;
                    
                    // this saves them to the database
                    db.Orders.Add(o);
                    db.Baskets.Add(newBasket);
                    db.SaveChanges();

                }
                

            }

            //this executes if the Add to Basket button is pressed
            //
            if (Request["Add To Basket"] != null)
            {

                int quant = int.Parse(Request["drop"].ToString());
                //if no user is logged in they are redirected to the login page. a product is passed along with them so they are redirected to the correct product info after login
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account", new { id = p.Id, Name = p.Name, CategoryId = p.CategoryId, Stock = p.Stock, Price = p.Price });
                }

                CustomerOrder oldOrder = customer.getOrder(customer.Id);

                //if an unpaid order is found
                //first the order is checked to see if it already contains the product we are attempting to add to it
                //if the products already exists in the order its quantity is adjusted apropriately and then orders total is amended
                //if the product is not present in the order it is added to the order and the order total is ammended
                //if no unpaid orders are found a new one is created for the selected customer and a list of products is begun with the one in question.

                if (oldOrder.CustomerId != null)
                {
                    int newQuant = 0;
                    int foundBaskId = 0;
                    double total = 0;
                    bool itemExists = false;
                    oldOrder.BasketItems = oldOrder.GetItems(oldOrder.Id);

                    foreach (Basket b in oldOrder.BasketItems)//this is the check to see if the product already exists in the order
                    {

                        if (b.ProductId == p.Id)//if the product exists its quantity is amended and the order total is set
                        {
                            itemExists = true;
                            newQuant = b.Quantity + quant;
                            foundBaskId = b.Id;


                        }

                    }
                    if (itemExists == true)
                    {
                        Basket oldBasket = db.Baskets.Find(foundBaskId);//this finds the orderline in the database




                        oldOrder.dbBasket(foundBaskId, newQuant);//this saves the new quantity to the database

                        total = (p.Price * quant) + oldOrder.Total; //order total is set


                    }
                    // if the item does not exist in the order
                    // a new orderline is made and its details saved to the database
                    //order total is set
                    else if (itemExists == false)
                    {
                        Basket newBasket = new Basket();
                        newBasket.dbNewBasket(oldOrder.Id, p.Id, quant);


                        total = oldOrder.Total + p.Price * quant;


                    }


                    oldOrder.setTotal(oldOrder.Id, total);//order total is saved to the database

                }
                //if no unaid order is found
                //a new order is created
                //a new orderline is created
                else if (oldOrder.CustomerId == null)
                {
                    CustomerOrder o = new CustomerOrder();
                    Basket newBasket = new Basket(p.Id, quant);

                    o.CustomerId = customer.Id;
                    o.Payment = false;
                    o.Date = DateTime.Now;

                    newBasket.CustomerOrderId = o.Id;
                    newBasket.Order = o;

                    o.BasketItems.Add(newBasket);
                    o.Total = p.Price * quant;

                    db.Orders.Add(o);
                    db.Baskets.Add(newBasket);
                    db.SaveChanges();

                }


            }

            //return View();
            return RedirectToAction("ViewProduct", p);//this redirects to the viewProduct controller. it sends a product as a perameter

        }

       

        //this controller returns the ViewProduct page
        //it sends a product to the front
        //it also send a list of customer usernames to the front
        public ActionResult ViewProduct(Product prod)
        {
            //no product is present it redirects to the homepage
            if (prod.Id == 0)
            {
                return RedirectToAction("Index");
            }
            
            else
            {
                List<Review> reviews = db.Reviews.ToList(); //this gets all the reviews from the database

                //the following loop adds each reveiw affiliated with the product to the review property (List<Review>) of that product
                //it also finds the user affiliated with that review
                foreach (Review r in reviews)
                {
                    if (r.ProductId == prod.Id)
                    {
                        prod.Reviews.Add(r);
                        r.User = db.Users.Find(r.UserId);
                    }
                }
                ViewBag.Product = prod;


                //these next few lines send a list of usernames to the front to be used in the predictive search bar
                List<User> users = db.Users.ToList();
                List<string> customerUserNames = new List<string>();

                foreach(User u in users)
                {
                    if(u.Role.Equals("Customer"))
                    {
                        customerUserNames.Add(u.UserName);
                    }
                }

                ViewBag.AutoFill = customerUserNames;
                return View();


            }
        }
    }
}
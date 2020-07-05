using CustomComputers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComputers.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //this controller is the get method for the AddProduct page
        //it creates a new product from user input
        [Authorize(Roles = "Manager, StockController")]
        [HttpGet]
        public ActionResult AddProduct(Product p)
        {
            if (Request["drop"] != null)
            {
                int catId = int.Parse(Request["drop"]);//this is a category the new product is to be added to

                p.CategoryId = catId;//this sets the category property of the product
                db.Products.Add(p);//add the new Product to the database
                db.SaveChanges();
            }

            ViewBag.Categories = db.ProductCategories.ToList();//this sends a list of categories from the databas to the front

            return View();
        }

        [Authorize(Roles = "Manager, StockController")]
        [HttpPost]
        public ActionResult AddProduct()
        {
            ViewBag.Categories = db.ProductCategories.ToList();//this sends a list of categories from the databas to the front

            return View();
        }

        //this controller is responsible for displaying the StockPage view
        //it sends a list of products from the database to the front
       [Authorize(Roles = "Manager, StockController")]
        public ActionResult StockPage()
        {
            List<Product> products = db.Products.ToList();

            ViewBag.Products = products;

            return View();
        }

        
        //this controller deals with all the buttons pressed on the AddProduct Page
        //each product on the front has a number input and an 'add stock' button next to it
        //When one of these buttons is pressed the 'if' statement will execute
        //
        [Authorize(Roles = "Manager, StockController")]
        [HttpGet]
        public ActionResult ProcessStockPage()
        {
            if(Request["addStock"]!=null)//this triggers after the add stock button is pressed
            {
                int prodId = int.Parse(Request["prodId"]);//this requests the relevent product Id from the front
                if (Request["addedStock"] != "false")//this checks that the addedstock input is a valid integer
                {
                    int addedStock = int.Parse(Request["addedStock"].ToString());//the mount of stock to be added is requested from the front

                    Product p = db.Products.Find(prodId);//the product is found in the database by its Id
                    p.Stock = p.Stock + addedStock;//its stock level is adjusted

                    db.SaveChanges();
                }

            }

            return RedirectToAction("StockPage","Product");//redirected to the StockPage controller
        }
    }

}
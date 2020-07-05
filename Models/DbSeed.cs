using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

//15/05/2020 - Pier Vettori

namespace CustomComputers.Models
{
    public class DbSeed : DropCreateDatabaseAlways<ApplicationDbContext>
    {

        //this seeds the database with information. Various Objects are created here
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);//this recreates the database every time the application is launched


            //this is the role manager included in the framework
            //this creates the different roles the system will use
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            roleManager.Create(new IdentityRole("Customer"));
            roleManager.Create(new IdentityRole("Staff"));
            roleManager.Create(new IdentityRole("StockController"));
            roleManager.Create(new IdentityRole("Manager"));



            context.SaveChanges();

            //the auto-generated user manager
            
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));



            // Super liberal password validation for password for seeds
            userManager.PasswordValidator = new PasswordValidator
            {
                RequireDigit = false,
                RequiredLength = 1,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false,
            };

            // creating a new Staff
            Staff Carlos = new Staff
            {
                UserName = "staff@staff.com",
                Email = "staff@staff.com",
                RegisteredAt = DateTime.Now.AddDays(-10),
                EmailConfirmed = true,
                FirstName = "Carlos",
                LastName = "Sanchez",
                Role = "Staff",
                AddressLn1 = " 123",
                AddressLn2= "Road Avenue",
                PostCode = "G41 2PN",
                Town = "Glasgow"

            };

            userManager.Create(Carlos, "1");//the user manager add to the database
            userManager.AddToRole(Carlos.Id, "Staff");// assigns to manager role

            // a new customer object
            Customer Steve = new Customer
            {
                UserName = "customer@customer.com",
                Email = "customer@customer.com",
                RegisteredAt = DateTime.Now.AddDays(-9),
                EmailConfirmed = true,
                FirstName = "Steve",
                LastName = "Harrington",
                Role = "Customer",
                AddressLn1 = " 124",
                AddressLn2 = "Lane Avenue",
                PostCode = "G41 2PN",
                Town= "Glasgow",
                PhoneNumber="07455325654"

            };

            userManager.Create(Steve, "1");//the user manager add to the database
            userManager.AddToRole(Steve.Id, "Customer");// assigns to customer role

            // a new customer object
            Customer Mark = new Customer
            {
                UserName = "customer2@customer.com",
                Email = "customer2@customer.com",
                RegisteredAt = DateTime.Now.AddDays(-10),
                EmailConfirmed = true,
                FirstName = "Mark",
                LastName = "Rancho",
                Role = "Customer",
                AddressLn1 = " 2",
                AddressLn2 = "road street",
                PostCode = "G41 2PN",
                Town = "Glasgow",
                PhoneNumber = "07455325254"

            };

            userManager.Create(Mark, "2");
            userManager.AddToRole(Mark.Id, "Customer");// assigns to customer role

            //A new Manager Object
            Manager Alice = new Manager
            {
                UserName = "manager@manager.com",
                Email = "manager@manager.com",
                RegisteredAt = DateTime.Now.AddDays(-11),
                EmailConfirmed = true,
                FirstName = "Alice",
                LastName = "Brown",
                Role = "Manager",
                AddressLn1 = " 125",
                AddressLn2 = "Street Avenue",
                PostCode = "G41 2PN",
                Town = "Glasgow"

            };

            userManager.Create(Alice, "1");// saves the manager object
            userManager.AddToRole(Alice.Id, "Manager");// assigns to customer role


            //a new Stock controller
            StockController Kyle = new StockController
            {
                UserName = "stock@stock.com",
                Email = "stock@stock.com",
                RegisteredAt = DateTime.Now.AddDays(-10),
                EmailConfirmed = true,
                FirstName = "Kyle",
                LastName = "Bore",
                Role = "StockController",
                AddressLn1 = " 125",
                AddressLn2 = "Snooze Avenue",
                PostCode = "G41 2PN",
                Town = "Glasgow"

            };

            userManager.Create(Kyle, "1");
            userManager.AddToRole(Kyle.Id, "StockController");

            context.SaveChanges();


            //these all make new productCategories and fill their properties
            ProductCategory CPUs = new ProductCategory
            {
                Name = "CPUs"
            };

            ProductCategory RAM = new ProductCategory
            { 
                Name="RAM"
            };

            ProductCategory HardDrives = new ProductCategory
            {
                Name = "HardDrives"
            };

            ProductCategory MotherBoards = new ProductCategory
            {
                Name = "MotherBoards"
            };

            ProductCategory PowerSupplies = new ProductCategory
            {
                Name = "PowerSupplies"
            };

            ProductCategory Monitors = new ProductCategory
            {
                Name = "Monitors"
            };

            ProductCategory Chassis = new ProductCategory
            {
                Name = "Chassis"
            };

            ProductCategory Accessories = new ProductCategory
            {
                Name = "Accessories"
            };
            ProductCategory CompleteMachines = new ProductCategory
            {
                Name = "PreBuilt"
            };
            
            //the new Product categories being adde to the database
            context.ProductCategories.Add(CPUs);
            context.ProductCategories.Add(RAM);
            context.ProductCategories.Add(Monitors);
            context.ProductCategories.Add(Chassis); 
            context.ProductCategories.Add(HardDrives);
            context.ProductCategories.Add(Accessories);
            context.ProductCategories.Add(CompleteMachines);
            

            context.SaveChanges();

            //these are all new Products being made and their Properties filled
            //the 'Image' Property is a string - a file path that will direct to an image saved
            Product FastRam = new Product
            {
                Name ="FastRam",
                Price = 50.50,
                Stock = 1,
              
                Category = RAM,
                Image = "~/Content/Images/CatRam.jpg"
            };

            Product SlowCPU = new Product
            {
                Name="SlowCPU",
                Price = 10,
                Stock = 100,
               
                Category = CPUs,
                Image = "~/Content/Images/Cpu.jpg"
            };

            Product BigHardDrive = new Product
            {
                Name = "BigHardDrive",
                Price = 200.20,
                Stock = 100,
               
                Category = HardDrives,
                Image = "~/Content/Images/HardDrive.jpg"
            };

            Product ShineyMoniter = new Product
            {
                Name = "Shiney3000",
                Price = 1,
                Stock = 100,
               
                Category = Monitors,
                Image = "~/Content/Images/Monitor.jpg"
            };


            Product SmallChassis = new Product
            {
                Name = "Roomy Case",
                Price = 5.25,
                Stock = 99,
                
                Category = Chassis,
                Image = "~/Content/Images/Chassis.jpg"
            };

            Product LEDLight = new Product
            {
                Name = "DumbLight",
                Price = 1000,
                Stock = 100,
               
                Category = Accessories,
                Image = "~/Content/Images/LEDLights.jpg"
            };
            Product PreBuilt = new Product
            {
                Name = "PreBuilt",
                Price = 1000,
                Stock = 100,
               
                Category = CompleteMachines,
                Image = "~/Content/Images/PreBuilt.jpg"
            };

            //all the new products being added to the Database
            context.Products.Add(FastRam);
            context.Products.Add(SlowCPU);
            context.Products.Add(BigHardDrive);
            context.Products.Add(ShineyMoniter);
            context.Products.Add(SmallChassis);
            context.Products.Add(LEDLight);
            context.Products.Add(PreBuilt);

            context.SaveChanges();

            //these are all Review Objects being made
            Review review1 = new Review
            {
                User = Mark,
                UserId = Mark.Id,
                Comment="Very fast ram",
                Rating = 5,
                Product = FastRam,
                ProductId = FastRam.Id

            };

            Review review2 = new Review
            {
                User = Steve,
                UserId = Steve.Id,
                Comment = "Ive seen faster",
                Product = FastRam,
                ProductId = FastRam.Id,
                Rating = 2

            };

            Review review3 = new Review
            {
                User = Mark,
                UserId = Mark.Id,
                Comment = "I liked the box",
                Product = SlowCPU,
                ProductId = SlowCPU.Id,
                Rating = 3

            };

            Review review4 = new Review
            {
                User = Steve,
                UserId = Steve.Id,
                Comment = "Computes well, but makes terrible tea",
                Product = SlowCPU,
                ProductId = SlowCPU.Id,
                Rating = 2

            };

            Review review5 = new Review
            {
                User = Mark,
                UserId = Mark.Id,
                Comment = "Now I have a place to store all my pictures of aquatic reptiles",
                Product = BigHardDrive,
                ProductId = BigHardDrive.Id,
                Rating = 5

            };

            Review review6 = new Review
            {
                User = Steve,
                UserId = Steve.Id,
                Comment = "Perfect door stop",
                Product = BigHardDrive,
                ProductId = BigHardDrive.Id,
                Rating = 3

            };

            Review review7 = new Review
            {
                User = Mark,
                UserId = Mark.Id,
                Comment = "Great Hard drive! This review has been made by a verbose Person. It shows how a long comment will look. This sentence is to add further length to this string. So is this sentence. This sentence is a shout out to Gilbert!",
                Product = BigHardDrive,
                ProductId = BigHardDrive.Id,
                Rating = 3

            };

            //the reviews being added to the database
            context.Reviews.Add(review1);
            context.Reviews.Add(review2);
            context.Reviews.Add(review3);
            context.Reviews.Add(review4);
            context.Reviews.Add(review5);
            context.Reviews.Add(review6);
            context.Reviews.Add(review7);

            context.SaveChanges();

            //Customer Orders being made
            CustomerOrder o1 = new CustomerOrder
            {
                Date = DateTime.Now.AddDays(-1),
                Payment = false,
                CustomerId = Steve.Id,
                GetCustomer = Steve,
                


            };
            //OrderLines being made and assigned to orders
            Basket b1 = new Basket
            {
                CustomerOrderId = o1.Id,
                Order = o1,
                ProductId = FastRam.Id,
                Product = FastRam,
                Quantity = 2,


            };


            Steve.Orders.Add(o1);
            o1.BasketItems.Add(b1);

            o1.Total = b1.Quantity * b1.Product.Price;
            context.Orders.Add(o1);
            context.Baskets.Add(b1);

            //another order
            CustomerOrder o2 = new CustomerOrder
            {
                Date = DateTime.Now.AddDays(-10),
                Payment = true,
                CustomerId = Mark.Id,
                GetCustomer = Mark,



            };
            Mark.Orders.Add(o2);//this assigns an order to a customer

            //orderlines for an order
            Basket b2 = new Basket
            {
                CustomerOrderId = o2.Id,
                Order = o2,
                ProductId = BigHardDrive.Id,
                Product = BigHardDrive,
                Quantity = 2,


            };
            //orderlines for an order
            Basket b3 = new Basket
            {
                CustomerOrderId = o2.Id,
                Order = o2,
                ProductId = SlowCPU.Id,
                Product = SlowCPU,
                Quantity = 2,


            };


            //adding orderlines to orders
            o2.BasketItems.Add(b2);
            o2.BasketItems.Add(b3);
            o2.Total = (b2.Quantity * b2.Product.Price) + (b3.Quantity * b3.Product.Price);//setting the total of the order

           
            //saving orders and orderlines to the database
            context.Orders.Add(o2);
            context.Baskets.Add(b2);
            context.Baskets.Add(b3);
            context.SaveChanges();


            
        }
    }
}
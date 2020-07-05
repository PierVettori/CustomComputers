using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

//15/05/2020 - Pier Vettori
namespace CustomComputers.Models
{
    
    //this is the basket (OrderLine) object
    //
    public class Basket
    {

        private static string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

        private ApplicationDbContext db = new ApplicationDbContext();
        
       //////////////////////////////////////////////// PROPERTIES//////////////////////////////////////////////////////////////////////////////////////////
        public int Id { get; set; }//Primary Key

        [ForeignKey("Order")]
        public int CustomerOrderId { get; set; }//this is a foreign key 
        public CustomerOrder Order { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; } //this is a foreign key 

        public Product Product { get; set; }

        public int Quantity { get; set; }


      
       /// /////////////////////////////////CONSTRUCTORS////////////////////////////////////////////////////
       

        public Basket()//default
        {
           
        }

        
        public Basket(int productId, int quantity)//contructor sets the ProductId and Quantity to the values specified as parameters
        {
            ProductId = productId;
            Quantity = quantity;
        }

        

        public Basket(int orderId, int prodId, int quantity)//contructor sets the CustomerOrderId, ProductId and Quantity to the values specified as parameters
        {
            CustomerOrderId = orderId;

            ProductId = prodId;

           
            Quantity = quantity;
        }


      
        /// /////////////////////////////////////////////////////BEHAVIOURS//////////////////////////////////////
      
        

        //this finds a product from the database and sets the Product property to that found product
        public Product GetProduct() 
        {
            
            Product p = db.Products.Find(ProductId);

            Product = p;

            return p; 
        }

        //this method inserts a new Basket(OrderLine) into the database from the values specified by the parameters
        public void dbNewBasket(int orderId, int prodId, int quant)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "INSERT INTO Baskets  (CustomerOrderId, ProductId, Quantity) VALUES ('" + orderId + "','" + prodId + "','" + quant + "')";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CustomComputers.Models
{
    //this is the CustomerOrder 
    //15/05/2020 - Pier Vettori
    public class CustomerOrder
    {
        //these are for connecting to the database
        private static string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

        /// <summary>
        /// ///////////////////////PROPERTIES///////////////////////////////////////////
        /// </summary>
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public double Total { get; set; }

        [ForeignKey("GetCustomer")]
        public string CustomerId { get; set; }

        public Customer GetCustomer { get; set; }

        public bool Payment { get; set; }

        public List<Basket> BasketItems { get; set; }

        /// <summary>
        /// /////////////////////////CONSTRUCTORS////////////////////////////////////////////////
        /// </summary>
        public CustomerOrder()
        {
            BasketItems = new List<Basket>();
        }

        
       
        /// /////////////////////////////BEHAVIOUR/////////////////////////////////////////////////
        
        //this is a list of orderlines
        //it finds each orderline affiliated to an Order from the database by the order Id
        public List<Basket> GetItems(int orderId)
        {

            List<Basket> foundBasket = new List<Basket>();// a list of OrderLines

            sqlCon.Open();
            string commandString = "SELECT * FROM Baskets WHERE CustomerOrderId = '" + orderId + "' ";//SQL statement finds orderlines that have a specified Order Id

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            SqlDataReader reader = sqlCmd.ExecuteReader();


            while (reader.Read())// each found row in the database is read
            {
                

                int id = int.Parse(reader["Id"].ToString());//the found colomn
                int quantity = int.Parse(reader["Quantity"].ToString());//the found colomn
                int prodId = int.Parse(reader["ProductId"].ToString());//the found colomn

                Basket b = new Basket();//a new orderline object
               
                //the properties of the oderline are set to the results of the database query
                b.Id = id;
                b.ProductId = prodId;
                b.CustomerOrderId = orderId;
                b.Quantity = quantity;

                foundBasket.Add(b);//the orderline is added to the list
            }


            //reader and connection to the database are closed on completion
            reader.Close();
            sqlCon.Close();

            

            return foundBasket;//the list of orderlines is returned


        }


        //this method sets the Quantity value of an orderline object specified by its id in the database 
        public void dbBasket(int id, int quantity)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "Update Baskets SET Quantity='" + quantity + "'  WHERE Id = '" + id + "' ";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }

        //this method sets the Total value of an orderline object specified by its id in the database 
        public void setTotal(int orderId, double total)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "Update CustomerOrders SET Total='" + total + "'  WHERE Id = '" + orderId + "' ";//sql statement

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }

        //this method sets the Payment value of an orderline object specified by its id in the database to true
        public void Pay(int orderId)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "Update CustomerOrders SET Payment = 1  WHERE Id = '" + orderId + "' ";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }

        //this adjusts the stock value of an orderline object in the database by its Id
        public void SetStock(int prodId, int stock)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "Update Products SET Stock =  '" + stock + "'  WHERE Id = '" + prodId + "' ";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }

        //this deletes orderline objects from the database by thier Id
        public void deleteBaskets(int id)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);


            sqlCon.Open();
            string commandString = "DELETE FROM Baskets WHERE CustomerOrderId = '" + id + "' ";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            sqlCmd.ExecuteNonQuery();



            sqlCon.Close();
        }

    }
}
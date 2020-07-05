using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

//15/05/2020 - Pier Vettori
namespace CustomComputers.Models
{
    public class Customer : User
    {
        /// <summary>
        /// /////////////////////////////PROPERTIES///////////////////////////////////
        /// </summary>

        public DateTime RegisteredAt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLn1 { get; set; }

        public string AddressLn2 { get; set; }

        public string PostCode { get; set; }

        public string Town { get; set; }

        public bool IsSuspended { get; set; }


        public List<CustomerOrder> Orders { get; set; }
        

        /// <summary>
        /// //////////////////////////////////CONSTRUCTORS//////////////////////////////////////////////////
        /// </summary>
        public Customer()
        {
            Orders = new List<CustomerOrder>();//this creates a list of CustomerOrders
            

        }


        /// /////////////////////////////////////////////////////BEHAVIOURS//////////////////////////////////////


        

       //this returns a CustomerOrder object from the database
       //it finds an order in the database that has a specified CustomerId and a Payment attribute set to false
       //then it constructs an CustomerOrder Object from the columns in the database and returns it

        public CustomerOrder getOrder(string user)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

            CustomerOrder o = new CustomerOrder();
            

            sqlCon.Open();
            string commandString = "SELECT * FROM CustomerOrders WHERE CustomerId = '" + user + "' AND Payment = 0";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            SqlDataReader reader = sqlCmd.ExecuteReader();


            while (reader.Read())
            {

                o.Id = int.Parse(reader["Id"].ToString());
                o.Total = double.Parse(reader["Total"].ToString());
                o.CustomerId = reader["CustomerId"].ToString();
                o.Date = DateTime.Parse(reader["Date"].ToString());
                o.Payment = false;


            }

            reader.Close();
            sqlCon.Close();

            return o;
            

        }

        
    }
}

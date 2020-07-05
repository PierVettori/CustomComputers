using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CustomComputers.Models
{
    //the Product Object
    //15/05/2020 - Pier Vettori
    public class Product
    {
        /// <summary>
        /// //////////////////////////////////PROPERTIES////////////////////////////////////////////////
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

      
        public ProductCategory Category { get; set; }

        public int CategoryId { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }

        public string  Image { get; set; }

        public List<Review> Reviews  { get; set; }

      
         //////////////////////////////////////CONSTRUCTORS//////////////////////////////////////////////////////////
     
        public Product()
        {
            Reviews = new List<Review>();
        }


     
       //this constructor builds the product object by filling all its properties from the database
       //it finds the desired product by its Id
        public Product(int id)
        {
            Id = id;
            

            Reviews = new List<Review>();
            

            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

            sqlCon.Open();
            string commandString = "SELECT * FROM Products WHERE Id= '"+id+"'  ";

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            SqlDataReader reader = sqlCmd.ExecuteReader();

            //all the properties being set
            while (reader.Read())
            {

                Name = reader["Name"].ToString();
                Price = (double)reader["Price"];
                Stock = (int)reader["Stock"];
                CategoryId = (int)reader["CategoryId"];
                Image = reader["Image"].ToString();

            }

            reader.Close();
            sqlCon.Close();

        }

        

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CustomComputers.Models
{

    //the Review Object
    //15/05/2020 - Pier Vettori
    public class Review
    {
        // // // ////////////////////////////////////////PROPERTIES////////////////////////////////////////////
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public double Rating { get; set; }

        public string Comment { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; }


        // // // ////////////////////////////////////////CONSTRUCTORS////////////////////////////////////////////
        public Review()
        {

        }

        //this constructor sets a Products Objects average rating from the database by finding each review affiliated with a particular product
        //the sum of each rating found is devided by the number of reveiws found to give an average rating
        //the properties for rating and product Id are then set
        public Review(int prodId)
        {
            string SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(SqlConnectionString);

            sqlCon.Open();
            string commandString = "SELECT * FROM Reviews WHERE ProductId = " + prodId + " ";//sql statement finds a review by its Product Id

            SqlCommand sqlCmd = new SqlCommand(commandString, sqlCon);
            SqlDataReader reader = sqlCmd.ExecuteReader();
            double rate = 0;
            int counter = 0;
            while (reader.Read())
            {
                counter++;
                
                
                rate = rate + double.Parse(reader["Rating"].ToString());

            }

            if (counter > 0)
            {
                rate = rate / counter;
            }
            reader.Close();
            sqlCon.Close();

            Rating = rate;
            ProductId = prodId;

        }
    }
}
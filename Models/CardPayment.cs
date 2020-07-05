using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

//15/05/2020 - Pier Vettori
namespace CustomComputers.Models
{
    public class CardPayment
    {
        /// <summary>
        /// /////////////////////////////PROPERTIES///////////////////////////////////////////
        /// </summary>
        public int OrderId { get; set; }

        public CustomerOrder Order  { get; set; }

        public double Total { get; set; }
    }
}
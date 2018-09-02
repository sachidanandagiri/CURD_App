using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CURD_App.Models
{
    public class Product
    {
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public double Product_Price { get; set; }
    }
}
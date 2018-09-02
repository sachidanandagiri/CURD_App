using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace CURD_App.Models
{
    public class DataAccessLayer
    {
        SQLDataAccess sqlDataAccess = new SQLDataAccess();
        
        //Get All Products 
        public IEnumerable<Product> GetProducts()
        {
            List<Product> products = new List<Product>();

            sqlDataAccess.Parameters = new List<SqlParameter>();
            sqlDataAccess.Parameters.Add(new SqlParameter("@Command", "GetData"));

            SqlDataReader dataReader = sqlDataAccess.ExecDataReader(DBCommand.ProductProcedure, CommandType.StoredProcedure);
            while (dataReader.Read())
            {
                Product product = new Product();
                product.Product_Id = Convert.ToInt32(dataReader[0]);
                product.Product_Name = dataReader[1].ToString();
                product.Product_Price = Convert.ToDouble(dataReader[2]);
                products.Add(product);
            }
            return products;
        }

        //Get Individual Product 
        public  Product GetIndividualProduct(string ProductName)
        {
            Product product = new Product();

            sqlDataAccess.Parameters = new List<SqlParameter>();
            sqlDataAccess.Parameters.Add(new SqlParameter("@ProductName", ProductName));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Command", "Find"));

            SqlDataReader dataReader = sqlDataAccess.ExecDataReader(DBCommand.ProductProcedure, CommandType.StoredProcedure);

            while (dataReader.Read())
            {
                product.Product_Id = Convert.ToInt32(dataReader[0]);
                product.Product_Name = dataReader[1].ToString();
                product.Product_Price = Convert.ToDouble(dataReader[2]);
            }

            return product;
        }

        //Update a Product
        public void UpdateProduct(Product product)
        {
            sqlDataAccess.Parameters = new List<SqlParameter>();
            sqlDataAccess.Parameters.Add(new SqlParameter("@ProductId", product.Product_Id));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Product_Name", product.Product_Name));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Product_Cost", product.Product_Price));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Command", "Update"));

            sqlDataAccess.ExecCommand(DBCommand.ProductProcedure, CommandType.StoredProcedure);
        }

        //Delete a Product
        public void DeleteProduct(int? Id)
        {
            sqlDataAccess.Parameters = new List<SqlParameter>();
            sqlDataAccess.Parameters.Add(new SqlParameter("@ProductId", Id));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Command", "Delete"));

            sqlDataAccess.ExecCommand(DBCommand.ProductProcedure, CommandType.StoredProcedure);
        }

        //Add a New product
        public void AddNewProduct(Product product)
        {
            sqlDataAccess.Parameters = new List<SqlParameter>();
            sqlDataAccess.Parameters.Add(new SqlParameter("@Product_Name", product.Product_Name));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Product_Cost", product.Product_Price));
            sqlDataAccess.Parameters.Add(new SqlParameter("@Command", "Insert"));

            sqlDataAccess.ExecCommand(DBCommand.ProductProcedure, CommandType.StoredProcedure);
        }
    }
}
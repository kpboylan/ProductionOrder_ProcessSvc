using MES_ProcessSvc.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MES_ProcessSvc.DAL
{
    internal class ProdOrderRepository : IProdOrderRepository
    {
        public void AddProdOrder(ProdOrder prodOrder)
        {
            prodOrder.Status = "Scheduled";

            string connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MES_Kboy;Integrated Security=True;TrustServerCertificate=True;";

            DateTimeOffset createdDT = DateTimeOffset.Now;
            DateTimeOffset modifiedDT = createdDT;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand command = new SqlCommand("WO_Add", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter { ParameterName = "@WONumber", SqlDbType = SqlDbType.NVarChar, Value = prodOrder.OrderNumber });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@WOStatus", SqlDbType = SqlDbType.NVarChar, Value = prodOrder.Status });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@PartID", SqlDbType = SqlDbType.NVarChar, Value = prodOrder.Material });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@Quantity", SqlDbType = SqlDbType.Float, Value = prodOrder.Quantity });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@UnitOfMeasure", SqlDbType = SqlDbType.NVarChar, Value = prodOrder.Uom });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@CreatedDT", SqlDbType = SqlDbType.DateTimeOffset, Value = createdDT });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@ModifiedDT", SqlDbType = SqlDbType.DateTimeOffset, Value = modifiedDT });

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}

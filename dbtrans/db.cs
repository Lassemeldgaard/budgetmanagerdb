using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace dbtrans
{
    public class db
    {
        Send sender = new Send();
        private SqlConnection connection = new SqlConnection("user id = lasse;" + "password = Dinmor123;" + "server = lassemeldgaard.database.windows.net;" + "database = budgetmanager;");

        public void OpenCon()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CloseCon()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteTransactionById(byte[] data)
        {
            string debug = Encoding.UTF8.GetString(data);
            JObject json = JObject.Parse(debug);
            SqlCommand command = new SqlCommand("DELETE FROM [Transaction] WHERE Id = @Id", connection);
            command.Parameters.Add(CreateParam("@Id", json["Id"], SqlDbType.Int));
            try
            {
                OpenCon();
                command.ExecuteNonQuery();
                JObject confirmation = new JObject();
                confirmation.Add("Status", 200);
                confirmation.Add("Result", "Transaction was deleted");
                sender.EmitData(confirmation, "lasdeletetransactionconfirmation");
                CloseCon();
            }
            catch (Exception)
            {

                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Create failed");
                sender.EmitData(confirmation, "lasdeletetransactionconfirmation");
                connection.Dispose();
                throw;
            }

        }
        public void CreateTransaction(byte[] data)
        {
            string debug = Encoding.UTF8.GetString(data);
            JObject json = JObject.Parse(debug);
            double temp = Convert.ToDouble(json["Value"].ToString());
            SqlCommand command = new SqlCommand("INSERT INTO [Transaction](Value, Text, Date, FK_Category) VALUES (@Value, @Text, @Date, @FK_Category)", connection);
            command.Parameters.Add(CreateParam("@Value", temp, SqlDbType.Float));
            command.Parameters.Add(CreateParam("@Text", json["Text"], SqlDbType.NVarChar));
            command.Parameters.Add(CreateParam("@Date", json["Date"], SqlDbType.Date));
            command.Parameters.Add(CreateParam("@FK_Category", json["FK_Category"], SqlDbType.Int));

            try
            {
                OpenCon();
                command.ExecuteNonQuery();
                JObject confirmation = new JObject();
                confirmation.Add("Status", 200);
                confirmation.Add("Result", "Transaction was created");
                sender.EmitData(confirmation, "lascreatetransactionconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Create failed");
                sender.EmitData(confirmation, "lascreatetransactionconfirmation");
                connection.Dispose();
                throw;
            }

        }

        public void UpdateTransaction(byte[] data)
        {
            string debug = Encoding.UTF8.GetString(data);
            JObject json = JObject.Parse(debug);

            SqlCommand command = new SqlCommand("UPDATE [Transaction] SET Value = @Value, Text = @Text, Date = @Date, FK_Category = @FK_Category WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Value", json["Value"]);
            command.Parameters.AddWithValue("@Text", json["Text"]);
            command.Parameters.AddWithValue("@Date", json["Date"]);
            command.Parameters.AddWithValue("@FK_Category", json["FK_Category"]);
            command.Parameters.AddWithValue("@Id", json["Id"]);

            try
            {
                OpenCon();
                command.ExecuteNonQuery();
                JObject confirmation = new JObject();
                confirmation.Add("Status", 200);
                confirmation.Add("Result", "Transaction was updated");
                sender.EmitData(confirmation, "lasupdatetransactionconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Update failed");
                sender.EmitData(confirmation, "lasupdatetransactionconfirmation");
                connection.Dispose();
                throw;
            }
        }

        internal void GetSingleCategoryById(byte[] data)
        {
            DataTable table = new DataTable();
            string debug = Encoding.UTF8.GetString(data);
            JObject json = JObject.Parse(debug);
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Category WHERE Id = @Id", connection);
            adapter.SelectCommand.Parameters.Add(CreateParam("@Id", json["Id"], SqlDbType.Int));
            try
            {
                JObject confirmation = new JObject();
                adapter.Fill(table);

                confirmation.Add("Status", 200);

                string JSONSTring = string.Empty;
                JSONSTring = JsonConvert.SerializeObject(table);

                confirmation.Add("Result", JSONSTring);
                sender.EmitData(confirmation, "lasgetcategoryconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Get failed");
                sender.EmitData(confirmation, "lasgetcategoryconfirmation");
                connection.Dispose();
                throw;
            }
        }

        internal void GetAllCategories()
        {
           
            DataTable table = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Category", connection);
            try
            {
                JObject confirmation = new JObject();
                adapter.Fill(table);

                confirmation.Add("Status", 200);

                string JSONSTring = string.Empty;
                JSONSTring = JsonConvert.SerializeObject(table);

                confirmation.Add("Result", JSONSTring);
                sender.EmitData(confirmation, "lasgetallcategoriesconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Get failed");
                sender.EmitData(confirmation, "lasgetallcategoriesconfirmation");
                connection.Dispose();
                throw;
            }
        }

        public void GetSingleTransactionById(byte[] data)
        {
            string debug = Encoding.UTF8.GetString(data);
            JObject json = JObject.Parse(debug);
            DataTable table = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM [Transaction] WHERE Id = @id", connection);
            adapter.SelectCommand.Parameters.Add(CreateParam("@Id", json["Id"], SqlDbType.Int));

            try
            {
                JObject confirmation = new JObject();
                adapter.Fill(table);

                confirmation.Add("Status", 200);

                string JSONSTring = string.Empty;
                JSONSTring = JsonConvert.SerializeObject(table);

                confirmation.Add("Result", JSONSTring);
                sender.EmitData(confirmation, "lasgettransactionconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Get failed");
                sender.EmitData(confirmation, "lasgettransactionconfirmation");
                connection.Dispose();
                throw;
            }
        }
        public void GetAllTransactions()
        {
            Console.WriteLine("Trans");
            DataTable table = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT [Transaction].Value, [Transaction].Text, [Transaction].Date, [Category].Name, [Category].Cat_ID, [Category].Id FROM [Transaction] INNER JOIN Category ON [Transaction].FK_Category=Category.Id", connection);


            try
            {
               
                JObject confirmation = new JObject();
                adapter.Fill(table);
               
                confirmation.Add("Status", 200);

                string JSONSTring = string.Empty;
                JSONSTring = JsonConvert.SerializeObject(table);

                confirmation.Add("Result", JSONSTring);
                sender.EmitData(confirmation, "lasgetalltransconfirmation");
                CloseCon();
            }
            catch (Exception)
            {
                JObject confirmation = new JObject();
                confirmation.Add("Status", 400);
                confirmation.Add("Result", "Get failed");
                sender.EmitData(confirmation, "lasgetalltransconfirmation");
                connection.Dispose();
                throw;
            }
        }

        private SqlParameter CreateParam(string name, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter(name, type)
            {
                Value = value
            };
            return param;
        }
    }
}
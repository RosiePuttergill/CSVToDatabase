using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data.Entity.Core.EntityClient;

namespace CsvToDatabase.Pages
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    SqlDataAdapter cmd = new SqlDataAdapter("SELECT * FROM [Clients]", con);
                    con.Open();
                    DataTable dtbl = new DataTable();
                    cmd.Fill(dtbl);
                    DataGrid.DataSource = dtbl;
                    DataGrid.DataBind();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

        }
        private string GetConnectionString()
        {
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString);
            return builder.ProviderConnectionString;
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            ProcessFile();
            PopulateDataGrid();
        }

        private void ProcessFile()
        {
            if (FileUpload.HasFile)
            {
                FileInfo fileInfo = new FileInfo(FileUpload.PostedFile.FileName);
                if (fileInfo.Name.EndsWith(".csv"))
                {
                    string fileName = fileInfo.Name.Replace(".csv", "").ToString();
                    string csvFilePath = Server.MapPath("CSV") + "\\" + fileInfo.Name;

                    FileUpload.SaveAs(csvFilePath);

                    try
                    {
                        int rowsAdded = UploadDataToDatabase("Clients", csvFilePath, ",");
                        Info.Text = string.Format("({0}) records have been uploaded", rowsAdded);
                        ErrorTitle.Text = "";
                        ErrorMessage.Text = "";
                    }
                    catch (SqlException ex)
                    {
                        Info.Text = "";
                        ErrorTitle.Text = "CSV contents not uploaded. See error below:";
                        ErrorMessage.Text = ex.Message;
                    }
                }
                else
                {
                    ErrorTitle.Text = "Unable to recognise file";
                }
            }
        }

        private int UploadDataToDatabase(string tableName, string filePath, string delimeter)
        {
            ClearTable("StagingClients");
            BulkInsertToStagingTable(filePath, delimeter);
            return InsertIntoClientTable();
        }

        private void ClearTable(string tableName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(string.Format("DELETE FROM {0}", tableName));
            ExecuteNonQuery(stringBuilder.ToString());
            ErrorTitle.Text = "";
            ErrorMessage.Text = "";
        }

        private int ExecuteNonQuery(string sqlQuery)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                {
                    int rows = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    return rows;
                }
            }
        }

        private void BulkInsertToStagingTable(string filePath, string delimeter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("BULK INSERT StagingClients");
            stringBuilder.AppendFormat(string.Format(" FROM '{0}'", filePath));
            stringBuilder.AppendFormat(string.Format(" WITH (FIELDTERMINATOR = '{0}' , ROWTERMINATOR = '\n' )", delimeter));
            ExecuteNonQuery(stringBuilder.ToString());
        }

        private int InsertIntoClientTable()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO Clients (Name, DOB, CreditLimit, YearsAtCurrentAddress, PolicyId) ");
            stringBuilder.Append("SELECT Name, DOB, CreditLimit, YearsAtCurrentAddress, PolicyId FROM StagingClients");
            return ExecuteNonQuery(stringBuilder.ToString());
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            ClearTable("Clients");
            PopulateDataGrid();
        }
    }
}
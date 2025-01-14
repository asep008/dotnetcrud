using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dummy_CRUD.Models;

namespace dummy_CRUD.Pages.clients
{
    public class EditModel : PageModel
    {
        private readonly VaultService _vaultService;
        public ClientInfo clientInfo = new ClientInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public EditModel(VaultService vaultService)
        {
            _vaultService = vaultService;
        }
        public async Task OnGet()
        {
            try
            {
                // Ensure id is treated as an integer
                int id = Convert.ToInt32(Request.Query["id"]);

                string dbHost = await _vaultService.GetSecret("host");
                string dbUser = await _vaultService.GetSecret("user");
                string dbPassword = await _vaultService.GetSecret("pass");

                string connectionString = $"Server={dbHost};Initial Catalog=library;User Id={dbUser};Password={dbPassword};MultipleActiveResultSets=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM clients WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientInfo.id = reader.GetInt32(0);  // ID should be an integer
                                clientInfo.name = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.phone = reader.GetString(3);
                                clientInfo.address = reader.GetString(4);
                                clientInfo.created_at = reader.GetDateTime(5);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public async Task OnPost()
        {
            try
            {
                // Ensure id is parsed as an integer from the form
                clientInfo.id = Convert.ToInt32(Request.Form["id"]);
                clientInfo.name = Request.Form["name"];
                clientInfo.email = Request.Form["email"];
                clientInfo.phone = Request.Form["phone"];
                clientInfo.address = Request.Form["address"];

                if (string.IsNullOrEmpty(clientInfo.name) ||
                    string.IsNullOrEmpty(clientInfo.email) ||
                    string.IsNullOrEmpty(clientInfo.phone) ||
                    string.IsNullOrEmpty(clientInfo.address))
                {
                    errorMessage = "All fields are required";
                    return;
                }

                string dbHost = await _vaultService.GetSecret("host");
                string dbUser = await _vaultService.GetSecret("user");
                string dbPassword = await _vaultService.GetSecret("pass");

                string connectionString = $"Server={dbHost};Initial Catalog=library;User Id={dbUser};Password={dbPassword};MultipleActiveResultSets=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE clients SET name=@name, email=@email, phone=@phone, address=@address WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", clientInfo.name);
                        command.Parameters.AddWithValue("@email", clientInfo.email);
                        command.Parameters.AddWithValue("@phone", clientInfo.phone);
                        command.Parameters.AddWithValue("@address", clientInfo.address);
                        command.Parameters.AddWithValue("@id", clientInfo.id);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                successMessage = "Client updated successfully";
                Response.Redirect("/clients/Index");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}

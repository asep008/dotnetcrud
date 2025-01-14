using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dummy_CRUD.Models;

namespace dummy_CRUD.Pages.clients
{
	public class CreateModel : PageModel
    {
        private readonly VaultService _vaultService;
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public CreateModel(VaultService vaultService)
        {
            _vaultService = vaultService;
        }
        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            clientInfo.name = Request.Form["name"];
            clientInfo.email = Request.Form["email"];
            clientInfo.phone = Request.Form["phone"];
            clientInfo.address = Request.Form["address"];

            if (clientInfo.name.Length == 0 || clientInfo.email.Length == 0 || clientInfo.phone.Length == 0 || clientInfo.address.Length == 0) {
                errorMessage = "All the field are required";
                return;
            }

            // save the new client to DB
            try
            {
                string dbHost = await _vaultService.GetSecret("host");
                string dbUser = await _vaultService.GetSecret("user");
                string dbPassword = await _vaultService.GetSecret("pass");

                string connectionString = $"Server={dbHost};Initial Catalog=library;User Id={dbUser};Password={dbPassword};MultipleActiveResultSets=True";

                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    String sql = "insert into clients " +
                                 "(name, email, phone, address) VALUES " +
                                 "(@name, @email, @phone, @address)";
                    using (SqlCommand command = new SqlCommand(sql, connection)) {
                        command.Parameters.AddWithValue("@name", clientInfo.name);
                        command.Parameters.AddWithValue("@email", clientInfo.email);
                        command.Parameters.AddWithValue("@phone", clientInfo.phone);
                        command.Parameters.AddWithValue("@address", clientInfo.address);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            clientInfo.name = ""; clientInfo.email = ""; clientInfo.phone = ""; clientInfo.address = "";
            successMessage = "New client Added correctly";
            Response.Redirect("/clients/Index");

        }

    }
}

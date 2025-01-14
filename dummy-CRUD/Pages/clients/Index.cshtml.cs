using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dummy_CRUD.Models;

namespace dummy_CRUD.Pages.clients
{
    public class IndexModel : PageModel
    {
        private readonly VaultService _vaultService;

        public List<ClientInfo> ListClients { get; set; } = new List<ClientInfo>();

        public IndexModel(VaultService vaultService)
        {
            _vaultService = vaultService;
        }

        public async Task OnGet()
        {
            try
            {
                // Retrieve database credentials from Vault
                string dbHost = await _vaultService.GetSecret("host");
                string dbUser = await _vaultService.GetSecret("user");
                string dbPassword = await _vaultService.GetSecret("pass");

                string connectionString = $"Server={dbHost};Initial Catalog=library;User Id={dbUser};Password={dbPassword};MultipleActiveResultSets=True";

                // Fetch clients from the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM clients";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ClientInfo clientInfo = new ClientInfo
                                {
                                    id = reader.GetInt32(0),
                                    name = reader.GetString(1),
                                    email = reader.GetString(2),
                                    phone = reader.GetString(3),
                                    address = reader.GetString(4),
                                    created_at = reader.GetDateTime(5)
                                };

                                ListClients.Add(clientInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
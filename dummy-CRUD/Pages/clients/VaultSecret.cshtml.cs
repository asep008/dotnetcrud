using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dummy_CRUD.Pages.PertaminaVault;
using Microsoft.Extensions.Configuration;
//using WebService.Database;


namespace dummy_CRUD.Pages.clients
{
    public class VaultSecretModel : PageModel
    {
        public string SecretValue { get; private set; }
        public string HostValue { get; private set; }
        public string ApiKey { get; private set; }
        public string DynamicSecret { get; private set; }
        

        public IActionResult OnGet()
        {

            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var VaultAddress = MyConfig.GetValue<string>("Vault:Address");
            var AppRoleAuthRoleId = MyConfig.GetValue<string>("Vault:RoleId");
            var AppRoleAuthSecretId = MyConfig.GetValue<string>("Vault:SecretId");

            

            //var VaultAddress = "http://localhost:8200";
            //var AppRoleAuthRoleId = "3fcb6c85-29d7-9ef5-81c7-1421afd5898e";
            //var AppRoleAuthSecretId = "ed89630d-922a-6aa6-fca9-f2f07a604104";
            Console.WriteLine(VaultAddress + AppRoleAuthRoleId + AppRoleAuthSecretId);
            try
            {
                VaultWrapper vault = new VaultWrapper(
                new vaultSettingspertamina
                    {
                    Address = VaultAddress,
                    AppRoleAuthRoleId = AppRoleAuthRoleId,
                    AppRoleAuthSecretId = AppRoleAuthSecretId
                    }
                );

                SecretValue = vault.GetSecretApiKey(path: "AZURE_KEY", KeyField: "pass");
                HostValue = vault.GetSecretApiKey(path: "AZURE_KEY", KeyField: "host");
                ApiKey = vault.GetSecretApiKey(path: "AZURE_KEY", KeyField: "api_key");

                //example retrive dynamic credentials
                var DatabaseUsername = vault.GetDatabaseCredentials(DatabaseCredentialsRole: "library_owner");
                var Username = DatabaseUsername.Username;
                var Password = DatabaseUsername.Password;
                //var lease_id = DatabaseUsername.lease_id;

                //UsernamePasswordCredentials credentials = vault.GetDatabaseCredentials(DatabaseCredentialsRole: "library_owner");




                //Console.WriteLine(DatabaseUsername);
                Console.WriteLine("Myusername is = " + Username + "\n Mypassword is = " + Password);
                return Page();



            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                Console.WriteLine("Execption: " + ex.ToString());
                return RedirectToPage("/Error"); // Redirect to an error page
            }

        }
    }
}


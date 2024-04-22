using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace dummy_CRUD.Pages.clients
{
	public class VaultSecretModel : PageModel
    {
        public string SecretValue { get; private set; }
        public IActionResult OnGet()

        {
            try
            {
                string vaultAddress = "http://localhost:8200";
                string roleId = "3fcb6c85-29d7-9ef5-81c7-1421afd5898e";
                string secretId = "ed89630d-922a-6aa6-fca9-f2f07a604104";
                string secretPath = "AZURE_KEY"; // Replace with your actual secret path
                string keyValue = "";

                IVaultClient vaultClient = CreateVaultClient(vaultAddress, roleId, secretId);
                SecretValue = GetSecretFromVault(vaultClient, secretPath);

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToPage("/Error"); // Redirect to an error page
            }
        }
        private IVaultClient CreateVaultClient(string vaultAddress, string roleId, string secretId)
        {
  
  
            IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(roleId, secretId.ToString());
            var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);


            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            return vaultClient;
        }

        private string GetSecretFromVault(IVaultClient vaultClient, string path)
        {
            Secret<SecretData> secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path).Result;

            var host = secret.Data.Data["host"].ToString();
            var user = secret.Data.Data["user"].ToString();
            var pass = secret.Data.Data["pass"].ToString();


            return host + user + pass ;
        }
    }
}

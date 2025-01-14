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
        private readonly IConfiguration _configuration;
        public string SecretValue { get; private set; }
        public string ErrorMessage { get; private set; }

        public VaultSecretModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var vaultConfig = _configuration.GetSection("Vault");

                string vaultAddress = vaultConfig["Address"];
                string roleId = Environment.GetEnvironmentVariable("VAULT_ROLE_ID") ?? throw new Exception("VAULT_ROLE_ID environment variable is not set.");
                string secretId = Environment.GetEnvironmentVariable("VAULT_SECRET_ID") ?? throw new Exception("VAULT_SECRET_ID environment variable is not set.");
                string secretPath = vaultConfig["ApiKeyPath"];
                string keyValue = vaultConfig["ApiKeyField"];

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
            var pass = secret.Data.Data["pass"].ToString(); //hi


            return host + " - " + user + " - " + pass;
        }
    }
}

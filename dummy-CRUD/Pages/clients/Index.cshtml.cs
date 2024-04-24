using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using System.Diagnostics;
using dummy_CRUD.Pages.PertaminaVault;

namespace dummy_CRUD.Pages.clients
{
	public class IndexModel : PageModel
    {
        //private readonly VaultPertamina vaultPertamina;
        public List<ClientInfo> ListClients = new List<ClientInfo>();
        private readonly VaultWrapper _vault; 
        public void OnGet() 
        {
          
            try
            {
                //String connectionString = "Data Source=ServerName;Initial Catalog=library;Integrated Security=False;User Id=sa;dockerStrongPwd123=;MultipleActiveResultSets=True" />
                String connectionString = "Server=localhost;Initial Catalog=library;Integrated Security=False;User Id=sa;Password=dockerStrongPwd123;MultipleActiveResultSets=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Select * from clients";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                ClientInfo clientInfo = new ClientInfo();
                                clientInfo.id = "" + reader.GetInt32(0);
                                clientInfo.name = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.phone = reader.GetString(3);
                                clientInfo.address = reader.GetString(4);
                                clientInfo.created_at = reader.GetDateTime(5).ToString();

                                ListClients.Add(clientInfo);

                            }
                        }
                    }
                    connection.Close();


                }
             }
            catch (Exception ex)
            {
                Console.WriteLine("Execption: " + ex.ToString()); 
            }

            //try
            //{
            //    var vaultAddr = "http://localhost:8200";
            //    var roleId = "3fcb6c85-29d7-9ef5-81c7-1421afd5898e";
            //    var secretId = "ed89630d-922a-6aa6-fca9-f2f07a604104";

            //    IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(roleId, secretId.ToString());
            //    var vaultClientSettings = new VaultClientSettings(vaultAddr, authMethod);

            //    IVaultClient vaultClient = new VaultClient(vaultClientSettings);


            //    Secret<SecretData> kv2Secret = null;
            //    kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "AZURE_KEY").Result;


            //    var host = kv2Secret.Data.Data["host"];
            //    var passAzure = kv2Secret.Data.Data["pass"];
            //    var userAzure = kv2Secret.Data.Data["user"];

            //    Console.WriteLine(host + "\n" + passAzure + "\n" + userAzure);
            //}
            //catch (AggregateException ex)
            //{
            //    foreach (var errInner in ex.InnerExceptions)
            //    {
            //        Debug.WriteLine(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
            //    }
            //}

        }
    }
    public class ApproleAuthExample
    {
        const string DefaultTokenPath = "../../../path/to/wrapping-token";

        /// <summary>
        /// Fetches a key-value secret (kv-v2) after authenticating to Vault via AppRole authentication
        /// </summary>
        public string GetSecretWithAppRole()
        {
            // A combination of a Role ID and Secret ID is required to log in to Vault with an AppRole.
            // The Secret ID is a value that needs to be protected, so instead of the app having knowledge of the secret ID directly,
            // we have a trusted orchestrator (https://developer.hashicorp.com/vault/tutorials/app-integration/secure-introduction?in=vault%2Fapp-integration#trusted-orchestrator)
            // give the app access to a short-lived response-wrapping token (https://developer.hashicorp.com/vault/docs/concepts/response-wrapping).
            // Read more at: https://learn.hashicorp.com/tutorials/vault/approle-best-practices?in=vault/auth-methods#secretid-delivery-best-practices
            var vaultAddr = "http://localhost:8200";
            var roleId = "3fcb6c85-29d7-9ef5-81c7-1421afd5898e";


            //// Get the path to wrapping token or fall back on default path
            //string pathToToken = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WRAPPING_TOKEN_PATH")) ? Environment.GetEnvironmentVariable("WRAPPING_TOKEN_PATH") : DefaultTokenPath;
            //string wrappingToken = File.ReadAllText(pathToToken); // placed here by a trusted orchestrator

            //// We need to create two VaultClient objects for authenticating via AppRole. The first is for
            //// using the unwrap utility. We need to initialize the client with the wrapping token.
            //IAuthMethodInfo wrappedTokenAuthMethod = new TokenAuthMethodInfo(wrappingToken);
            //var vaultClientSettingsForUnwrapping = new VaultClientSettings(vaultAddr, wrappedTokenAuthMethod);

            //IVaultClient vaultClientForUnwrapping = new VaultClient(vaultClientSettingsForUnwrapping);

            //// We pass null here instead of the wrapping token to avoid depleting its single usage
            //// given that we already initialized our client with the wrapping token
            //Secret<Dictionary<string, object>> secretIdData = vaultClientForUnwrapping.V1.System
            //    .UnwrapWrappedResponseDataAsync<Dictionary<string, object>>(null).Result;

            var secretId = "ed89630d-922a-6aa6-fca9-f2f07a604104";

            // We create a second VaultClient and initialize it with the AppRole auth method and our new credentials.
            IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(roleId, secretId.ToString());
            var vaultClientSettings = new VaultClientSettings(vaultAddr, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            // We can retrieve the secret from VaultClient
            Secret<SecretData> kv2Secret = null;
            kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "kv-v2/AZURE_KEY").Result;


            var host = kv2Secret.Data.Data["host"];
            Console.WriteLine(host);

            return host.ToString();
        }
    }

    public class ClientInfo
    {
        public string id;
        public string name;
        public string email;
        public string phone;
        public string address;
        public string created_at;

    }
 
}

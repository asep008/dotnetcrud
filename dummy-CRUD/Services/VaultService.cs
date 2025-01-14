using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.Commons;
using Microsoft.Extensions.Configuration;

public class VaultService
{
    private readonly string _vaultAddress;
    private readonly string _roleId;
    private readonly string _secretId;
    private readonly string _apiKeyPath;
    private readonly string _apiKeyField;

    private IVaultClient _vaultClient;

    public VaultService(IConfiguration configuration)
    {
        // Read Vault configuration from appsettings.json
        var vaultConfig = configuration.GetSection("Vault");
        
        _vaultAddress = vaultConfig["Address"];
        _roleId = vaultConfig["RoleId"];
        _secretId = vaultConfig["SecretId"];
        _apiKeyPath = vaultConfig["ApiKeyPath"];
        _apiKeyField = vaultConfig["ApiKeyField"];

        // Initialize Vault client settings
        IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(_roleId, _secretId);
        var vaultClientSettings = new VaultClientSettings(_vaultAddress, authMethod);

        // Initialize the Vault client
        _vaultClient = new VaultClient(vaultClientSettings);
    }

    public async Task<string> GetDatabaseHost()
    {
        try
        {
            // Retrieve secret from Vault (using the correct path)
            Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(_apiKeyPath);

            // Extract the required field from the secret
            return secret.Data.Data[_apiKeyField]?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Vault secret in get database host: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetSecret(string key)
    {
        try
        {
            // Retrieve secret from Vault using the specified key
            Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(_apiKeyPath);
            
            // Return the secret value for the given key
            return secret.Data.Data[key]?.ToString() ?? throw new Exception($"Key '{key}' not found in Vault.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Vault secret in get secret: {ex.Message}");
            throw;
        }
    }
}

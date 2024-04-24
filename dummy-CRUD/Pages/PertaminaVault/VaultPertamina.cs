﻿// Copyright (c) HashiCorp, Inc.
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
//using Vault;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace dummy_CRUD.Pages.PertaminaVault
{
    public class VaultWrapper
    {
        private readonly ILogger _logger;
        private readonly IVaultClient _client;
        private readonly vaultSettingspertamina _settings;
    

        public VaultWrapper(vaultSettingspertamina settings)
        {
        //_logger = loggerFactory.CreateLogger("Vault");
            _client = AppRoleAuthClient(settings);
        _settings = settings;
        }


        private IVaultClient AppRoleAuthClient(vaultSettingspertamina settings)
        {

            AppRoleAuthMethodInfo appRoleAuth = new AppRoleAuthMethodInfo(
                roleId: settings.AppRoleAuthRoleId,
                secretId: settings.AppRoleAuthSecretId
            );

            IVaultClient client = new VaultClient(
                new VaultClientSettings(settings.Address, appRoleAuth)
            );

            //_logger.LogInformation($"logging in to vault @ {settings.Address} with approle role id {settings.AppRoleAuthRoleId}: done");

            return client;
        }

        public string GetSecretApiKey(string path, string KeyField)
        {
            //_logger.LogInformation("getting secret api key from vault: started");

            //Secret<SecretData> secret = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            //    // vault path within kv-v2/ (e.g. "api-key", not "kv-v2/api-key")
            //    path: _settings.ApiKeyPath
            //).Result;

       
            //_logger.LogInformation("getting secret api key from vault: done");
            Secret<SecretData> secret = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path).Result;
            string apiKey = secret.Data.Data[KeyField].ToString();

            return apiKey;
        }

        public UsernamePasswordCredentials GetDatabaseCredentials(string DatabaseCredentialsRole)
        {
            //    _logger.LogInformation("getting temporary database credentials from vault: started");

            Secret<UsernamePasswordCredentials> dynamicDatabaseCredentials = _client.V1.Secrets.Database.GetCredentialsAsync(
                // vault path within database/roles/ (e.g. "dev-readonly", not "database/roles/dev-readonly")
                roleName: DatabaseCredentialsRole
            ).Result;

            //_logger.LogInformation("getting temporary database credentials from vault: done");

            return dynamicDatabaseCredentials.Data;
        }

        public int sum(int x, int y) {
            return x + y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace dummy_CRUD.Pages.PertaminaVault
{
	public class vaultSettingspertamina
	{
		//vaul address Server
		public string Address { set; get; }
		// AppRole credentials used to authenticate with Vault
		//public string AppRoleAuthRoleId = "3fcb6c85-29d7-9ef5-81c7-1421afd5898e";
		//public string AppRoleAuthSecretId = "ed89630d-922a-6aa6-fca9-f2f07a604104";

        public string AppRoleAuthRoleId { set; get; }
        public string AppRoleAuthSecretId { set; get; }


        //public string AppRoleAuthSecretIdFile = "";

        //keyPath secret
        public string ApiKeyPath { set; get; }
		public string ApiKeyField { set; get;  }

		//public string ApiKeyPath = "AZURE_KEY";
		//public string ApiKeyField = "host";



        public vaultSettingspertamina()
		{
		}
	}
}


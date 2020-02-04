using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Tokens;
using System;

namespace OAuth.Services
{
    public class KeyVaultHelper
    {
        private static string GetKeyVaultEndpoint() => "https://oauth-0-kv.vault.azure.net";

        public static RsaSecurityKey GetSigningKey()
        {
            var keyVaultEndpoint = GetKeyVaultEndpoint();
            if (string.IsNullOrEmpty(keyVaultEndpoint)) throw new InvalidOperationException();
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));
            var key = keyVaultClient.GetKeyAsync("https://oauth-0-kv.vault.azure.net/keys/OAuthRSA/8bc6ec6c8ca14bb4a1ef1d32f1df29e0").GetAwaiter().GetResult();
            if (key == null)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving the signing key from Azure Key Vault.");
            }

            return new RsaSecurityKey(keyVaultClient.ToRSA(key));
        }
    }
}
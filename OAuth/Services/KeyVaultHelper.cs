using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Tokens;
using System;

namespace OAuth.Services
{
    public class KeyVaultHelper
    {
        public static RsaSecurityKey GetSigningKey(string keyVaultEndpoint, string keyPath)
        {
            if (string.IsNullOrEmpty(keyVaultEndpoint)) throw new InvalidOperationException();
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));
            var key = keyVaultClient.GetKeyAsync(keyPath).GetAwaiter().GetResult();
            if (key == null)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving the signing key from Azure Key Vault.");
            }

            return new RsaSecurityKey(keyVaultClient.ToRSA(key));
        }
    }
}
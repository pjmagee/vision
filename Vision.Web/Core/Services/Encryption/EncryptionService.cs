using Microsoft.AspNetCore.DataProtection;

namespace Vision.Web.Core
{

    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector protector;

        public EncryptionService(IDataProtectionProvider provider)
        {
            protector = provider.CreateProtector(purpose: "Encryption");
        }

        public void Decrypt(VersionControlDto versionControl)
        {
            versionControl.ApiKey = Decrypt(versionControl.ApiKey);
        }

        public void Decrypt(CiCdDto cicd)
        {
            cicd.Username = Decrypt(cicd.Username);
            cicd.Password = Decrypt(cicd.Password);
            cicd.ApiKey = Decrypt(cicd.ApiKey);
        }

        public void Decrypt(RegistryDto registry)
        {
            registry.Password = Decrypt(registry.Password);
            registry.Username = Decrypt(registry.Username);
            registry.ApiKey = Decrypt(registry.ApiKey);
        }
                

        public void Encrypt(VersionControlDto versionControl)
        {
            versionControl.ApiKey = Encrypt(versionControl.ApiKey);
        }

        public void Encrypt(CiCdDto cicd)
        {
            cicd.Username = Encrypt(cicd.Username);
            cicd.ApiKey = Encrypt(cicd.ApiKey);
            cicd.Password = Encrypt(cicd.Password);
        }

        public void Encrypt(RegistryDto registry)
        {
            registry.ApiKey = Encrypt(registry.ApiKey);
            registry.Username = Encrypt(registry.Username);
            registry.Password = Encrypt(registry.Password);
        }

        public string Encrypt(string text)
        {
            if(!string.IsNullOrWhiteSpace(text))
                return protector.Protect(text);

            return text;
        }

        public string Decrypt(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                return protector.Unprotect(text);

            return text;
        }
    }
}

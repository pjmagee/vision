namespace Vision.Core
{
    public interface IEncryptionService
    {
        string Encrypt(string text);
        string Decrypt(string text);

        void Encrypt(VersionControlDto versionControl);
        void Decrypt(VersionControlDto versionControl);

        void Encrypt(CiCdDto cicd);
        void Decrypt(CiCdDto cicd);

        void Encrypt(RegistryDto registry);
        void Decrypt(RegistryDto registry);
    }
}

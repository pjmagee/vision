using Microsoft.AspNetCore.DataProtection;

namespace Vision.Tests
{
    public class DataProtectionStub : IDataProtectionProvider, IDataProtector
    {
        public IDataProtector CreateProtector(string purpose) => new DataProtectionStub();

        public byte[] Protect(byte[] plaintext) => plaintext;

        public byte[] Unprotect(byte[] protectedData) => protectedData;
    }
}

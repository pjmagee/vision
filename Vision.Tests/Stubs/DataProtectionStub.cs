using Microsoft.AspNetCore.DataProtection;

namespace Vision.Tests
{
    public class DataProtectionStub : IDataProtectionProvider, IDataProtector
    {
        public DataProtectionStub()
        {

        }

        public IDataProtector CreateProtector(string purpose)
        {
            return new DataProtectionStub();
        }                

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}

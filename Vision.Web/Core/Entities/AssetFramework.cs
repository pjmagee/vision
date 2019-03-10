using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Vision.Web.Core
{
    public class AssetFramework : Entity, IEquatable<AssetFramework>
    {        
        public Guid AssetId { get; set; }        
        public Guid FrameworkId { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual Framework Framework { get; set; }
        public bool Equals(AssetFramework other) => Id.Equals(other.Id);
    }

    //public class PasswordHasher
    //{
    //    public PasswordHasherResult Compute(string password)
    //    {
    //        // generate a 128-bit salt using a secure PRNG
    //        byte[] salt = new byte[128 / 8];

    //        using (var rng = RandomNumberGenerator.Create())
    //        {
    //            rng.GetBytes(salt);
    //        }

    //        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    //        password: password,
    //        salt: salt,
    //        prf: KeyDerivationPrf.HMACSHA1,
    //        iterationCount: 10000,
    //        numBytesRequested: 256 / 8));

    //        return new PasswordHasherResult(salt, hashed);
    //    }
    //}

    //public class PasswordHasherResult
    //{
    //    public byte[] Salt { get; }
    //    public string Hash { get; }

    //    public PasswordHasherResult(byte[] salt, string hash)
    //    {
    //        this.Salt = salt;
    //        this.Hash = hash;
    //    }
    //}
}

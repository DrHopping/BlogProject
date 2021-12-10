using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Digests;
using Sodium;

namespace Blog.Hashing
{
 public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private readonly Argon2PasswordHasherOptions options;

        public Argon2PasswordHasher(IOptions<Argon2PasswordHasherOptions> optionsAccessor = null)
        {
            options = optionsAccessor?.Value ?? new Argon2PasswordHasherOptions();
        }

        public virtual string HashPassword(TUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            
            return PasswordHash.ArgonHashString(GetSha3(password), ParseStrength()).TrimEnd('\0');
        }

        public virtual PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword)) throw new ArgumentNullException(nameof(hashedPassword));
            if (string.IsNullOrWhiteSpace(providedPassword)) throw new ArgumentNullException(nameof(providedPassword));
            
            var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, GetSha3(providedPassword));

            if (isValid && PasswordHash.ArgonPasswordNeedsRehash(hashedPassword, ParseStrength()))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private PasswordHash.StrengthArgon ParseStrength()
        {
            return options.Strength switch
            {
                Argon2HashStrength.Interactive => PasswordHash.StrengthArgon.Interactive,
                Argon2HashStrength.Moderate => PasswordHash.StrengthArgon.Moderate,
                Argon2HashStrength.Sensitive => PasswordHash.StrengthArgon.Sensitive,
                Argon2HashStrength.Medium => PasswordHash.StrengthArgon.Medium,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetSha3(string input)
        {
            var sha3 = new Sha3Digest(512);
            var inputBytes = Encoding.ASCII.GetBytes(input);
            sha3.BlockUpdate(inputBytes, 0, inputBytes.Length);
            var result = new byte[64];
            sha3.DoFinal(result, 0);
            var hashString = BitConverter.ToString(result);
            hashString = hashString.Replace("-", "").ToLowerInvariant();
            return hashString;
        }
    }
}
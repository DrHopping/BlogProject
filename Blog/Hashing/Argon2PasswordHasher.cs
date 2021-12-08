using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
            
            return PasswordHash.ArgonHashString(password, ParseStrength()).TrimEnd('\0');
        }

        public virtual PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword)) throw new ArgumentNullException(nameof(hashedPassword));
            if (string.IsNullOrWhiteSpace(providedPassword)) throw new ArgumentNullException(nameof(providedPassword));

            var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword);

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
    }
}
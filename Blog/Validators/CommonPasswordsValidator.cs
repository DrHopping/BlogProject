using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Validators
{
    public class CommonPasswordsValidator : IPasswordValidator<User> 
    {
        private List<string> commonPasswords;
        
        public CommonPasswordsValidator()
        {
            commonPasswords = File.ReadAllLines(@"Resources/top-100k-passwords.txt").ToList();
        }
        
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            if (commonPasswords.Contains(password))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "Common password",
                    Description = "You cannot use this password because it is too common"
                }));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Net.Http.Headers;

namespace Blog.Extensions
{
    public static class RequestExtensions
    {
        public static string GetToken(this HttpRequest request)
        {
            var accessToken = request.Headers[HeaderNames.Authorization].ToString().Split(' ')[1];
            if (accessToken == null) throw new ArgumentNullException("Couldn't get the token user authorized with");
            return accessToken;
        }
    }
}
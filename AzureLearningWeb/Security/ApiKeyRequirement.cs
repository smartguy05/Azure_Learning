using AzureLearning.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace AzureLearningWeb.Security
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        private List<string> ApiKeys { get; set; }

        public ApiKeyRequirement(AppSettings settings)
        {
            ApiKeys = settings.ApiKeys.ToList();
        }

        public bool IsValidApiKey(string apiKey)
        {
            return ApiKeys.Contains(apiKey);
        }
    }
}
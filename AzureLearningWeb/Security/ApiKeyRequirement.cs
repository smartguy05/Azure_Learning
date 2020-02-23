using AzureLearning.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace AzureLearningWeb.Security
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        public IReadOnlyList<string> ApiKeys { get; set; }

        public ApiKeyRequirement(AppSettings appSettings)
        {
            ApiKeys = appSettings.ApiKeys.ToList();
        }
    }
}
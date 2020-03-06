using AzureLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureLearningWeb.ApiControllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "ApiKey")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppSettings _settings;

        public SettingsController(AppSettings settings)
        {
            _settings = settings;
        }

        [HttpGet("")]
        public async Task<AppSettings> GetSettings()
        {
            return _settings;
        }
    }
}
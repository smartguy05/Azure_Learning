using AzureLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureLearningWeb.ApiControllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "ApiKeyPolicy")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        public SettingsController(AppSettings settings)
        {
            _settings = settings;
        }

        [HttpGet("")]
        public async Task<AppSettings> GetSettings()
        {
            return _settings;
        }

        private readonly AppSettings _settings;
    }
}
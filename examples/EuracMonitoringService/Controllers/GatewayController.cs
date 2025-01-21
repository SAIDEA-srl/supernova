using EuracMonitoringService.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supernova.Models.GatewayHooks;

namespace EuracMonitoringService.Controllers
{
    [Route("/Gateway")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GatewayController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IBackgroundHookQueue backgroundHookQueue;


        public GatewayController(IAuthorizationService authorizationService,
            IBackgroundHookQueue backgroundHookQueue)
        {
            this.authorizationService = authorizationService;
            this.backgroundHookQueue = backgroundHookQueue;
        }

        [HttpGet("Hooks")]
        public virtual async ValueTask<GatewayHook[]> GetHooks()
        {
            return await Task.FromResult<GatewayHook[]>([new GatewayHook()
            {
                Name = "production-data",
                Parameters = new Dictionary<string, string> {
                        ["start-date"] = "es: 2024-01-01",
                        ["end-date"] =  "es: 2024-12-01",
                        ["measure"] = "inverters",
                        ["field"] = "P_dc"
                    }
            }]);
        }

        [HttpGet("Hooks/{hookname}/executions/{execId}")]
        [ProducesResponseType<GatewayHookExecution>(200)]
        public virtual async ValueTask<ActionResult<GatewayHookExecution>> GetExecution(string hookname, string execId)
        {
            if (!Guid.TryParse(execId, out var executionId))
            {
                return NotFound();
            }

            var exec = backgroundHookQueue.GetHookExecution(executionId);

            if (exec == null)
            {
                return NotFound();
            }
            //get current status
            return Ok(exec);
        }

        [HttpGet("Hooks/{hookname}/executions/{execId}/result")]
        [ProducesResponseType<GatewayHookResult>(200)]
        public virtual async ValueTask<ActionResult<GatewayHookResult>> GetExecutionResult(string hookname, string execId)
        {
            if (!Guid.TryParse(execId, out var executionId))
            {
                return NotFound();
            }

            var exec = backgroundHookQueue.GetHookExecution(executionId);
            if (exec == null)
            {
                return NotFound();
            }
            //get current status
            return Ok(exec.Result);
        }


        [HttpPost("Hooks/{hookname}/run")]
        [ProducesResponseType<GatewayHookExecution>(200)]
        public virtual async ValueTask<ActionResult<GatewayHookExecution>> RunHook(string hookname, [FromBody] Dictionary<string, object> paramenters)
        {
            await Task.Yield();

            if (hookname != "production-data")
            {
                return BadRequest();
            }

            var execution = new GatewayHookExecution()
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.Now,
                HookName = hookname,
                Paramenters = paramenters,
                Status = HookStatus.Created
            };

            await backgroundHookQueue.QueueHookAsync(execution);

            return Ok(execution);
        }
    }
}

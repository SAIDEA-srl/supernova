using Supernova.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EuracMonitoringService.Controllers;

[Route("/Info")]
[AllowAnonymous]
public class InfoController : ControllerBase
{

    [HttpGet]
    public virtual async Task<Info> GetInfo()
    {
        return await Task.FromResult(new Info()
        {
            Name = "eurac monitoring",
            Source = "eurac-monitoring-service",
            Description = "Prototype",
            Version = "0.0.1",
            SupportedDocuments = [
                new SupportedDocument() {
                    Description = "Production data",
                    DocumentType = "text/csv"
                }
            ]
        });
    }
}

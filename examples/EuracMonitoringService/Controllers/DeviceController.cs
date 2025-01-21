using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supernova.Models;

namespace EuracMonitoringService.Controllers;


[Route("/Devices")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DeviceController : ControllerBase
{

    static readonly string[] Devices = [
        "CdTe1",
        "CdTe14_1",
        "CdTe14_2",
        "CdTe14_3",
        "CdTe15_1",
        "CdTe15_2",
        "CdTe15_3",
        "CdTe16_1",
        "CdTe16_2",
        "CdTe16_3",
        "CdTe17_1",
        "CdTe17_2",
        "CdTe17_3",
        "CdTe18_1",
        "CdTe18_2",
        "CdTe18_3",
        "CdTe19_1",
        "CdTe19_2",
        "CdTe19_3",
        "CdTe20_1",
        "CdTe20_2",
        "CdTe20_3",
        "CdTe21_1",
        "CdTe21_2",
        "CdTe21_3",
        "CdTe22_1",
        "CdTe22_2",
        "CdTe22_3",
        "CdTe23_1",
        "CdTe23_2",
        "CdTe23_3",
        "CdTe24_1",
        "CdTe24_2",
        "CdTe24_3",
        "CdTe25_1",
        "CdTe25_2",
        "CdTe25_3"
    ];

    [HttpGet]
    public virtual async Task<IEnumerable<Supernova.Models.Device>> Get()
    {
        return from device in Devices
               select new Supernova.Models.Device()
               {
                   DeviceUUID = new OrangeButton.Models.DeviceUUID()
                   {
                       Value = device
                   }
               };        
    }

    [HttpGet("{id}")]
    public virtual async ValueTask<ActionResult<Supernova.Models.Device>> GetById(string id)
    {
        var devices = from device in Devices
                      where device == id
                      select new Device()
                      {
                          DeviceUUID = new OrangeButton.Models.DeviceUUID()
                          {
                              Value = device
                          }
                      };

        var foundDevice = devices.FirstOrDefault();

        if(foundDevice != null)
        {
            return Ok(foundDevice);
        }

        return NotFound();
    }
}

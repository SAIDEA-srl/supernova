using InfluxDB.Client;
using System.Reflection.PortableExecutable;
using Supernova.Models.GatewayHooks;
using EuracMonitoringService.Services;

namespace EuracMonitoringService.Workers.Hooks
{
    public class ProductionDataHook : IHookTask
    {
        private readonly InfluxDBClient client;
        private readonly IConfiguration configuration;

        public ProductionDataHook(InfluxDBClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
        }

        public async Task<GatewayHookResult> Execute(GatewayHookExecution exec)
        {
            try
            {

                exec.Status = HookStatus.Execution;

                var query = client.GetQueryApi();

                var startDate = DateTime.Parse(exec.Paramenters["start-date"].ToString() as string);
                var endDate = DateTime.Parse(exec.Paramenters["end-date"].ToString() as string);
                var measure = exec.Paramenters["measure"].ToString() as string;
                var field = exec.Paramenters["field"].ToString() as string;

                var database = configuration.GetValue<string>("InfluxDB:Database");

                var dump = await query.QueryRawAsync(@$"from(bucket: ""{database}"")
              |> range(start: {startDate.ToString("s")}Z, stop: {endDate.ToString("s")}Z)
              |> filter(fn: (r) => r[""_measurement""] == ""{measure}"")  
              |> filter(fn: (r) => r._field == ""{field}"") 
              |> aggregateWindow(every: 1h, fn: mean, createEmpty: false)
              |> group(columns: [""dev_id"", ""_field""])   
              |> sum(column: ""_value"")");

                // trasform dump to csv and save to memory ?
                var url = ReportManager.CreateFile($"reports/{exec.Id.ToString()}.csv", dump);

                //return result
                return new GatewayHookResult()
                {
                    ExecutionId = exec.Id,
                    DateTime = DateTime.Now,
                    IsSuccessful = true,
                    ResponseUrl = url
                };

            }
            catch (Exception ex)
            {
                return new GatewayHookResult()
                {
                    ExecutionId = exec.Id,
                    DateTime = DateTime.Now,
                    IsSuccessful = false,
                    Message = ex.Message
                };
            }
        }
    }
}

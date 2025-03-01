using System.Data.Common;
using System.Text;
using System.Text.Json;
using AutoMapper;
using IceSync.AppClient;
using IceSync.DataCore;
using IceSync.DataCore.Models;
using IceSync.Services.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IceSync.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public WorkflowService(IMemoryCache memoryCache,
        IConfiguration configuration,
        ApplicationDbContext context,
        IMapper mapper)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
        _context = context;
        _mapper = mapper;
    }

    public async Task<OutputTypeModel<List<WorkflowDTO>>> GetAllAsync()
    {
        IAppHttpClient<string?, List<WorkflowDTO>> client = new AppHttpClient<string?, List<WorkflowDTO>>(_memoryCache, _configuration);
        return await client.Execute(null, GetAllAsyncBase);
    }

    public async Task<OutputTypeModel<string?>> RunAsync(int model)
    {
        IAppHttpClient<int, string?> client = new AppHttpClient<int, string?>(_memoryCache, _configuration);
        return await client.Execute(model, RunAsyncBase);
    }

    public async Task WorkflowTableSync(CancellationToken token)
    {
        var result = await GetAllAsync();

        if (result.IsSuccessfull)
        {
            var workflowsFromApi = result.Model;
            
            var workflowIds = workflowsFromApi.Select(w => w.WorkflowId).ToList();

            #region Delete records
            // removing workflows which doesnt exist in the database
            // db records
            var workflowsForDelete = _context.Workflows
                    .Where(w => !workflowIds.Contains(w.WorkflowId));

            if (workflowsForDelete.Any())
            {
                _context.Workflows.RemoveRange(workflowsForDelete);
            }
            #endregion

            #region Update records
            // db records - materialize querry
            var workflowsForUpdate = _context.Workflows
                    .Where(w => workflowIds.Contains(w.WorkflowId)).ToList();
            var workflowForUpdateIds = workflowsForUpdate.Select((w) => w.WorkflowId).ToList();      

            for (int i = 0; i < workflowsForUpdate.Count; i++)
            {
                var workflowFromApi = workflowsFromApi.First((w) => w.WorkflowId == workflowsForUpdate[i].WorkflowId);
                _mapper.Map<WorkflowDTO, Workflow>(workflowFromApi, workflowsForUpdate[i]);
            }

            #endregion

            #region Insert new records
            // db records
            var workflowsForUpdateIds = workflowsForUpdate.Select((w) => w.WorkflowId).ToList();
            var workflowsForInsert = workflowsFromApi
                    .Where(w => !workflowsForUpdateIds.Contains(w.WorkflowId)).ToList();               

            if (workflowsForInsert.Any())
            {
                var workflows = _mapper.Map<List<Workflow>>(workflowsForInsert);
                await _context.Workflows.AddRangeAsync(workflows);
            }
            #endregion

            await _context.SaveChangesAsync();
        }
    }


    #region service base methods

    private async Task<OutputTypeModel<List<WorkflowDTO>>> GetAllAsyncBase(InputTypeModel<string?> inputSetup)
    {
        var response = await inputSetup.HttpClient.GetAsync(inputSetup.Url + "/workflows");

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            if (result != null) {
                List<WorkflowDTO> serializedResult = JsonSerializer.Deserialize<List<WorkflowDTO>>(result);
                return new OutputTypeModel<List<WorkflowDTO>>(response.StatusCode)
                {
                    Model = serializedResult,
                    IsSuccessfull = true,
                };
            }
        }

        return new OutputTypeModel<List<WorkflowDTO>>(response.StatusCode);
    }

    private async Task<OutputTypeModel<string?>> RunAsyncBase(InputTypeModel<int> inputSetup)
    {
        var json = JsonSerializer.Serialize(
            new {
                WorkflowId=inputSetup.Value,
                waitOutput="true",
                decodeOutputJsonString="true",
            }
        );

        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await inputSetup.HttpClient.PostAsync(
            inputSetup.Url + string.Format("/workflows/{0}/run", inputSetup.Value),
            jsonContent
        );

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            return new OutputTypeModel<string?>(response.StatusCode)
                {
                    IsSuccessfull = true,
                };
        }

        return new OutputTypeModel<string?>(response.StatusCode);
    }

    #endregion
}

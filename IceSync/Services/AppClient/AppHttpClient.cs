
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IceSync.Services.Helpers;
using IceSync.Services.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IceSync.AppClient;

public class AppHttpClient<InputT, OutputT> : IAppHttpClient<InputT, OutputT>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private byte _maxNumberOfTriesToAuthenticate;
    private const string CacheKey = "AppCacheKeyJWT";
    
    public AppHttpClient(
        IMemoryCache memoryCache,
        IConfiguration configuration, 
        byte maxNumberOfTriesToAuthenticate = 1)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
        _maxNumberOfTriesToAuthenticate = maxNumberOfTriesToAuthenticate;
    }

    public async Task<OutputTypeModel<OutputT>> Execute(InputT inputT, Func<InputTypeModel<InputT>, Task<OutputTypeModel<OutputT>>> method, byte counter = 0)
    {
        counter++;
        try {
            using (HttpClient client = new HttpClient())
            {
                var ApiSettings = _configuration.GetSection("ApiSettings");
                string? token = await GetJwtToken();
                if (token == null)
                {
                    throw new ArgumentNullException();
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                InputTypeModel<InputT> model = new InputTypeModel<InputT>(client, ApiSettings["ApiUrl"]);
                model.Value = inputT;
                var outputModel = await method(model);

                if (outputModel.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                return outputModel;
            }
        }
        catch(UnauthorizedAccessException ex) {
            if (counter != _maxNumberOfTriesToAuthenticate) {
                return await Execute(inputT, method, counter);
            }
            return new OutputTypeModel<OutputT>(HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private async Task<string?> GetJwtToken()
    {
        var encryptionSettings = _configuration.GetSection("EncryptionSettings");
        EncryptionHelper encryptionHelper = new EncryptionHelper(encryptionSettings["SaltKey"]);
        if (_memoryCache.TryGetValue(CacheKey, out string token))
        {
            return encryptionHelper.Decrypt(token);
        } else {
            var ApiSettings = _configuration.GetSection("ApiSettings");

            using (HttpClient client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(
                    new {
                        apiCompanyId = ApiSettings["AppCompanyId"],
                        apiUserId = ApiSettings["ApiUserId"],
                        apiUserSecret = ApiSettings["ApiUserSecret"],
                    }
                );
                var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ApiSettings["ApiUrl"] + "/authenticate", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                    };

                    _memoryCache.Set(CacheKey, encryptionHelper.Encrypt(result), cacheEntryOptions);
                    return result;
                }

                return null;
            }
        }
    }
}
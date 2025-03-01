namespace IceSync.Services.Models;

public class InputTypeModel<T>
{
    private readonly HttpClient _httpClient;
    private string _url;

    public InputTypeModel(HttpClient httpClient, string url)
    {
        _httpClient = httpClient;
        _url = url;
    }

    public T? Value { get; set; }
    public HttpClient HttpClient { get { return _httpClient; } }
    public string Url { get { return _url; } }
}

using System.Net;

namespace IceSync.Services.Models;

public class OutputTypeModel<T>
{
    private HttpStatusCode _httpStatusCode;
    public OutputTypeModel(HttpStatusCode httpStatusCode)
    {
        _httpStatusCode = httpStatusCode;
    }
    public bool IsSuccessfull { get; set; } = false;
    public T? Model { get; set; }    
    public HttpStatusCode HttpStatusCode { get{ return _httpStatusCode; }}
}

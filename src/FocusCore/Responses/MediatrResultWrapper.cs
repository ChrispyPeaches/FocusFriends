using System.Net;

namespace FocusCore.Responses;

public class MediatrResultWrapper<T> where T : class
{
    public HttpStatusCode? HttpStatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
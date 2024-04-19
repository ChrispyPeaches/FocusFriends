using System.Net;

namespace FocusCore.Responses;

public class MediatrResult
{
    public HttpStatusCode? HttpStatusCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
}
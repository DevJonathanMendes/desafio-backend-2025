using System.Net;

namespace Bankount.Exceptions;

public class HttpResponseException(HttpStatusCode statusCode, object? value = null) : Exception
{
	public int StatusCode { get; } = (int)statusCode;
	public object? Value { get; } = value;
}

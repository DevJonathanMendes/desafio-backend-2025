using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bankount.Exceptions;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
	public int Order => int.MaxValue - 10;

	public void OnActionExecuting(ActionExecutingContext context) { }

	public void OnActionExecuted(ActionExecutedContext context)
	{
		if (context.Exception is HttpResponseException ex)
		{
			var result = new ObjectResult(ex.Value) { StatusCode = ex.StatusCode };

			context.Result = result;
			context.ExceptionHandled = true;
		}
	}
}

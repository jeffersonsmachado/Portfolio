using Portfolio.Domain.Shared;

namespace Portfolio.Api.Extensions;

public static class ResultExtensions
{
	public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess, int failureStatusCode = StatusCodes.Status400BadRequest)
	{
		return result.Match(
			onSuccess,
			error => Results.Problem(
				title: error.Code,
				detail: error.Description,
				statusCode: failureStatusCode
			)
		);
	}
}
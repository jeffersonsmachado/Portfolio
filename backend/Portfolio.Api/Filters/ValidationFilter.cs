using FluentValidation;

namespace Portfolio.Api.Filters;

/// <summary>
/// Action filter to validate incoming requests using FluentValidation
/// </summary>


public class ValidationFilter<T> : IEndpointFilter where T : class
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		if (context.HttpContext.RequestServices.GetService(typeof(IValidator<T>)) is IValidator<T> validator)
		{
			var entity = context.Arguments.OfType<T>().FirstOrDefault();
			if (entity != null)
			{
				var validationResult = await validator.ValidateAsync(entity);
				if (!validationResult.IsValid)
				{
					var errorsByProperty = validationResult.Errors
						.GroupBy(error => error.PropertyName)
						.ToDictionary(
							group => group.Key,
							group => group.Select(error => error.ErrorMessage).ToArray()
						);

					return Results.ValidationProblem(errorsByProperty);
				}
			}
		}

		// Proceed to the next filter or endpoint
		return await next(context);
	}
}
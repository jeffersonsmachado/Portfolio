namespace Portfolio.Domain.Shared;

public record Error(string Code, string Description)
{
	public static readonly Error None = new(string.Empty, string.Empty);
}

public class Result<T>
{
	public bool IsSuccess { get; init; }
	public bool IsFailure => !IsSuccess;
	public T? Value { get; }
	public Error? Error { get; }

	private Result(bool isSuccess, Error error, T? value = default)
	{
		if (isSuccess && error != Error.None)
			throw new InvalidOperationException("Successful result cannot have an error.");
		if (!isSuccess && error == Error.None)
			throw new InvalidOperationException("Failure result must have an error.");

		IsSuccess = isSuccess;
		Error = error;
		Value = value;
	}

	public static Result<T> Success(T value) => new(true, Error.None, value);
	public static Result<T> Failure(Error error) => new(false, error);

	public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
	{
		return IsSuccess
			? onSuccess(Value!)
			: onFailure(Error!);
	}

}
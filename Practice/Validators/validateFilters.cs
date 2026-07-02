using FluentValidation;

namespace Practice.Validators;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argToValidate = context.Arguments.FirstOrDefault(x => x is T) as T;

        if (argToValidate is null)
        {
            return TypedResults.BadRequest("Data Not Found!");
        }

        var validationResult = await _validator.ValidateAsync(argToValidate);

        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(validationResult.ToDictionary());
        }

        return await next(context);
    }
}
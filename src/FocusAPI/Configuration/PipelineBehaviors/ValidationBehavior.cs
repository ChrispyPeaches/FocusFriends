using FluentValidation;
using MediatR;
using System.Text;

namespace FocusAPI.Configuration.PipelineBehaviors;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors).Where(x => x != null).ToList();

        if (failures.Any())
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var failure in failures)
            {
                stringBuilder.AppendLine(failure.ErrorMessage);
            }
            // TODO: Figure out a better way to handle this.
            throw new Exception("Validation failed for request:\n" + stringBuilder.ToString());
        }

        return next();
    }
}
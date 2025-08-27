using EndPointEcommerce.Domain.Exceptions;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class ErrorMessage
{
    public string Message { get; }

    public ErrorMessage(string message)
    {
        Message = message;
    }

    public ErrorMessage(EntityNotFoundException ex)
    {
        Message = ex.Message;
    }
}

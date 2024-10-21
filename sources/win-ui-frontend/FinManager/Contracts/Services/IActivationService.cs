namespace FinManager.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}

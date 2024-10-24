namespace Fin_Manager_v2.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}

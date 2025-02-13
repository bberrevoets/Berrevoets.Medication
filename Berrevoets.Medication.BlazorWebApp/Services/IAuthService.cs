namespace Berrevoets.Medication.BlazorWebApp.Services;

public interface IAuthService
{
    Task<bool> Login(LoginModel model);
    Task Logout();
}
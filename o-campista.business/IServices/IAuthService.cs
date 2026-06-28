
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(
        LoginRequest request);

    Task RegistrarAsync(
        RegisterRequest request);

    Task<LoginResponse> GoogleAuthAsync(
        GoogleAuthRequest request);

    Task ForgotPasswordAsync(
        ForgotPasswordRequest request);

    Task ResetPasswordAsync(
        ResetPasswordRequest request);

    Task<LoginResponse> RefreshTokenAsync(
        string email);
}

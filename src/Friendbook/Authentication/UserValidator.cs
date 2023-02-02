using SimpleAuthentication.BasicAuthentication;

namespace Friendbook.Authentication;

public class UserValidator : IBasicAuthenticationValidator
{
    public Task<BasicAuthenticationValidationResult> ValidateAsync(string userName, string password)
    {
        if (userName == password)
        {
            return Task.FromResult(BasicAuthenticationValidationResult.Success(userName));
        }

        return Task.FromResult(BasicAuthenticationValidationResult.Fail("Invalid user"));
    }
}
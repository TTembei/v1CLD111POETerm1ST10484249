using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CLD111POETerm1ST10484249.Repository.Models;

public class UserSessionHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSessionHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User GetLoggedInUser()
    {
        var userSession = _httpContextAccessor.HttpContext?.Session.GetString("LoggedInUser");
        return userSession != null ? JsonConvert.DeserializeObject<User>(userSession) : null;
    }
}

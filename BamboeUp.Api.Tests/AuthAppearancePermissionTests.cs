using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Shell.Controllers;
using Xunit;

public sealed class AuthAppearancePermissionTests
{
    [Fact]
    public void Login_appearance_permission_is_a_read_only_admin_authorized_endpoint()
    {
        var method = typeof(AuthController).GetMethod(nameof(AuthController.LoginAppearancePermission))!;
        Assert.Equal("Admin", method.GetCustomAttribute<AuthorizeAttribute>()?.Roles);
        Assert.Equal("login-appearance-permission", method.GetCustomAttribute<HttpGetAttribute>()?.Template);
        Assert.Null(method.GetCustomAttribute<AllowAnonymousAttribute>());
    }
}

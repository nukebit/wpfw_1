using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Auth;
using PortfolioApi.Security;

namespace PortfolioApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signIn,
    PortfolioDbContext context) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = new IdentityUser { UserName = email, Email = email };

        // Save the account and its initial role together.
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var created = await users.CreateAsync(user, request.Password);
            if (!created.Succeeded)
            {
                if (created.Errors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
                {
                    return DuplicateAccount();
                }

                foreach (var error in created.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            var assigned = await users.AddToRoleAsync(user, PortfolioRoles.Guest);
            if (!assigned.Succeeded)
            {
                return Problem(title: "The initial account role could not be assigned.");
            }

            await transaction.CommitAsync(cancellationToken);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return DuplicateAccount();
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await signIn.PasswordSignInAsync(
            request.Email.Trim(), request.Password, isPersistent: false, lockoutOnFailure: true);

        return result.Succeeded
            ? NoContent()
            : Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid email or password.");
    }

    private ObjectResult DuplicateAccount() => Problem(
        statusCode: StatusCodes.Status409Conflict, title: "An account with this email already exists.");
}

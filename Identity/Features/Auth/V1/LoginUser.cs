using FluentValidation;
using Identity.Controllers;
using Identity.DTO;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Features.Auth.V1
{
    public sealed record LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public LoginUserData User { get; set; } = default!; // Cambiado a LoginUserData
    }

    public sealed record LoginUserData
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public static class LoginUser
    {
        public class Validator : AbstractValidator<LoginRequest>
        {
            public Validator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("Username is required")
                    .WithMessage("Username format is invalid");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required");
            }
        }

        public static IEndpointRouteBuilder MapLoginUser(this IEndpointRouteBuilder endpoints)
        {
            var authGroup = endpoints.MapAuthGroup();

            authGroup.MapPost("/login", HandleAsync)
                .WithName("LoginUserV1")
                .AddOpenApiOperationTransformer((operation, context, ct) =>
                {
                    operation.Summary = "Login user";
                    operation.Description = "Authenticates a user and returns a JWT access token";
                    return Task.CompletedTask;
                })
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .DisableAntiforgery()
                .AllowAnonymous();

            return endpoints;
        }

        private static async Task<IResult> HandleAsync(
            LoginRequest request,
            IUserService authService,
            IValidator<LoginRequest> validator,
            ILogger<CreateUserRequest> logger,
            CancellationToken cancellationToken = default)
        {
            // Validar request
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.ToDictionary();
                logger.LogWarning("Login validation failed for username/email: {UserName}", request.UserName);
                return Results.ValidationProblem(errors, title: "Validation failed");
            }

            // Llamar al servicio de login
            var loginResponse = await authService.LoginAsync(request.UserName, request.Password);

            if (loginResponse == null)
            {
                logger.LogWarning("Login failed for username/email: {UserName}", request.UserName);
                return Results.Problem(
                    title: "Login failed",
                    detail: "Invalid username or password",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            // Log usando User.Id
            logger.LogInformation("User successfully logged in: {UserId} - {Email}",
                loginResponse.User.Id, loginResponse.User.Email);

            return Results.Ok(loginResponse);
        }
    }
}

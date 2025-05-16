using System.Reflection;
using NetArchTest.Rules;

namespace CQRSPattern.Api.Architecture;

/// <summary>
/// Validates the architecture of the application against predefined rules
/// </summary>
public class ArchitectureValidator
{
    private readonly ILogger<ArchitectureValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchitectureValidator"/> class
    /// </summary>
    /// <param name="logger">Logger for validation results</param>
    public ArchitectureValidator(ILogger<ArchitectureValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates all architectural rules and returns the result
    /// </summary>
    /// <returns>True if all rules pass, false otherwise</returns>
    public bool ValidateArchitecture()
    {
        _logger.LogInformation("Starting architecture validation...");
        
        var apiAssembly = Assembly.GetExecutingAssembly();
        var applicationAssembly = Assembly.Load("CQRSPattern.Application");

        var allRulesPassed = true;
        
        // Validate layer dependencies
        allRulesPassed &= ValidateLayerDependencies(
            apiAssembly, 
            applicationAssembly);
        
        // Validate controllers follow naming convention
        allRulesPassed &= ValidateControllerNamingConvention(apiAssembly);
        
        // Validate handlers are in correct namespaces
        allRulesPassed &= ValidateHandlerNamespaces(applicationAssembly);

        if (allRulesPassed)
        {
            _logger.LogInformation("All architecture validation rules passed successfully");
        }
        else
        {
            _logger.LogError("Architecture validation failed! Some rules were violated");
        }

        return allRulesPassed;
    }

    private bool ValidateLayerDependencies(
        Assembly apiAssembly,
        Assembly applicationAssembly)
    {
        // Application layer should not depend on API or Infrastructure layers
        var applicationDependencyResult = Types.InAssembly(applicationAssembly)
            .Should()
            .NotHaveDependencyOn("CQRSPattern.Api")
            .And()
            .NotHaveDependencyOn("CQRSPattern.Infrastructure")
            .GetResult();

        if (!applicationDependencyResult.IsSuccessful)
        {
            _logger.LogError("Application layer has invalid dependencies on API or Infrastructure layers: {Types}",
                string.Join(", ", applicationDependencyResult.FailingTypes?.Select(t => t.FullName) ?? Array.Empty<string>()));
            return false;
        }

        // API layer can depend on Application but not on Infrastructure directly
        var apiDependencyResult = Types.InAssembly(apiAssembly)
            .That()
            .ResideInNamespace("CQRSPattern.Api")
            .Should()
            .NotHaveDependencyOn("CQRSPattern.Infrastructure.Persistence.Repositories")
            .GetResult();

        if (!apiDependencyResult.IsSuccessful)
        {
            _logger.LogError("API layer has invalid direct dependencies on Infrastructure repositories: {Types}",
                string.Join(", ", apiDependencyResult.FailingTypes?.Select(t => t.FullName) ?? Array.Empty<string>()));
            return false;
        }

        _logger.LogInformation("Layer dependency validation passed");
        return true;
    }

    private bool ValidateControllerNamingConvention(Assembly apiAssembly)
    {
        var controllerResult = Types.InAssembly(apiAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        if (!controllerResult.IsSuccessful)
        {
            _logger.LogError("Some controllers do not follow the naming convention: {Types}",
                string.Join(", ", controllerResult.FailingTypes?.Select(t => t.FullName) ?? Array.Empty<string>()));
            return false;
        }

        _logger.LogInformation("Controller naming convention validation passed");
        return true;
    }

    private bool ValidateHandlerNamespaces(Assembly applicationAssembly)
    {
        var handlerResult = Types.InAssembly(applicationAssembly)
            .That()
            .ImplementInterface(typeof(MediatR.IRequestHandler<,>))
            .Should()
            .ResideInNamespaceStartingWith("CQRSPattern.Application.Features")
            .GetResult();

        if (!handlerResult.IsSuccessful)
        {
            _logger.LogError("Some handlers are not in the correct namespace: {Types}",
                string.Join(", ", handlerResult.FailingTypes?.Select(t => t.FullName) ?? Array.Empty<string>()));
            return false;
        }

        _logger.LogInformation("Handler namespace validation passed");
        return true;
    }
}

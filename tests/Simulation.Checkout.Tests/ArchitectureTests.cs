
using NetArchTest.Rules;
using Simulation.Checkout.Application.Abstractions;
using System.Reflection;
using Xunit;

namespace Simulation.Checkout.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOn_ApplicationOrInfrastructure()
    {
        // Arrange
        var domainAssembly = Assembly.Load("Simulation.Checkout.Domain");

        // Act
        var applicationResult = Types
            .InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Simulation.Checkout.Application")
            .GetResult();

        var infrastructureResult = Types
            .InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Simulation.Checkout.Infrastructure")
            .GetResult();

        // Assert
        Assert.True(applicationResult.IsSuccessful);
        Assert.True(infrastructureResult.IsSuccessful);
    }


    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        // Arrange
        var applicationAssembly = Assembly.Load("Simulation.Checkout.Application");

        // Act
        var result = Types
            .InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Simulation.Checkout.Infrastructure")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }


    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        // Arrange
        var assemblies = new[]
        {
        Assembly.Load("Simulation.Checkout.Domain"),
        Assembly.Load("Simulation.Checkout.Application"),
        Assembly.Load("Simulation.Checkout.Infrastructure")
    };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }


    [Fact]
    public void OrderRepositoryImplementations_ShouldEndWithRepository()
    {
        // Arrange
        var assemblies = new[]
        {
        Assembly.Load("Simulation.Checkout.Domain"),
        Assembly.Load("Simulation.Checkout.Application"),
        Assembly.Load("Simulation.Checkout.Infrastructure")
    };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(IOrderRepository))
                .Should()
                .HaveNameEndingWith("Repository")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }

}

using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.System;

[Category("System")]
public class SystemTests
{
    [Test]
    public async Task AppRunningInDocker_ShouldBeHealthy()
    {
        // Arrange
        await BuildDockerImageOfAppAsync();
        var container = await StartAppInContainersAsync();
        var httpClient = new HttpClient { BaseAddress = GetAppBaseAddress(container) };

        // Act
        var healthCheckResponse = await httpClient.GetAsync("healthz");
        var appResponse = await httpClient.GetAsync("/");

        // Assert
        (string Stdout, string Stderr) logValues = await container.GetLogsAsync();
        Console.WriteLine($"Stderr:{Environment.NewLine}{logValues.Stderr}");
        Console.WriteLine($"Stdout:{Environment.NewLine}{logValues.Stdout}");
        logValues.Stdout.Should().NotContain("warn:");
        healthCheckResponse.Should().BeSuccessful();
        (await healthCheckResponse.Content.ReadAsStringAsync()).Should().Be("Healthy");
        appResponse.Should().BeSuccessful();
        (await appResponse.Content.ReadAsStringAsync()).Should().Contain("<title>This is your life</title>");
    }

    private static async Task BuildDockerImageOfAppAsync()
    {
        var rootDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent ?? throw new NullReferenceException();
        var projectFile = Path.Join(rootDirectory.FullName, "WebApp", "WebApp.csproj");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"publish {projectFile} --os linux --arch amd64 /t:PublishContainer -p:ContainerImageTags=local-system-test",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        while (!process.StandardOutput.EndOfStream)
        {
            Console.WriteLine(await process.StandardOutput.ReadLineAsync());
        }

        await process.WaitForExitAsync();
        process.ExitCode.Should().Be(0);
    }

    private static async Task<IContainer> StartAppInContainersAsync()
    {
        Console.WriteLine("Building and starting network");
        var network = new NetworkBuilder().Build();
        await network.CreateAsync();
        Console.WriteLine("Network started");
        Console.WriteLine("Building and starting app container");
        var container = BuildAppContainer(network);
        await container.StartAsync();
        Console.WriteLine("App container started");
        return container;
    }

    private static IContainer BuildAppContainer(INetwork network) =>
        new ContainerBuilder()
            .WithImage("mu88/thisisyourlife:local-system-test")
            .WithNetwork(network)
            .WithPortBinding(8080, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .Build();

    private static Uri GetAppBaseAddress(IContainer container) => new($"http://{container.Hostname}:{container.GetMappedPublicPort(8080)}/thisIsYourLife");
}
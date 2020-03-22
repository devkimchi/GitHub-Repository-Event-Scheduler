using System.Linq;

using FluentAssertions;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace EventScheduler.FunctionApp.Tests
{
    [TestClass]
    public class StartUpTests
    {
        [TestMethod]
        public void Given_Instance_When_Configure_Invoked_Then_It_Should_Have_Dependencies()
        {
            var services = new ServiceCollection();

            var builder = new Mock<IFunctionsHostBuilder>();
            builder.SetupGet(p => p.Services).Returns(services);

            var startup = new StartUp();
            startup.Configure(builder.Object);

            services.Where(p => p.ServiceType.Name.Equals("DefaultHttpClientFactory")).Count().Should().BeGreaterThan(0);
            services.Where(p => p.ServiceType.Name.Equals("JsonSerializerSettings")).Count().Should().BeGreaterThan(0);
            services.Where(p => p.ServiceType.Name.Equals("JsonMediaTypeFormatter")).Count().Should().BeGreaterThan(0);
        }
    }
}

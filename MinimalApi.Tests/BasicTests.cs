using NUnit.Framework;
using System.Threading.Tasks;

namespace MinimalApi.Tests
{
    public class BasicTests
    {
        [Test]
        public async Task GetEntryPoint_ReturnsHelloWorld()
        {
            // Arrange
            using var application = new MinimalApiTestHost();
            using var client = application.CreateClient();

            // Act
            using var response = await client.GetAsync("/");

            // Assert
            Assert.AreEqual("Hello World!", await response.Content.ReadAsStringAsync());
        }
    }


}
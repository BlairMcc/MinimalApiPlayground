using NUnit.Framework;
using System.Threading.Tasks;

namespace MinimalApi.Tests
{
    public class GetProductCountTests
    {
        [Test]
        public async Task GetCount_ReturnsZero()
        {
            // Arrange
            using var application = new MinimalApiTestHost();
            using var client = application.CreateClient();

            // Act
            using var response = await client.GetAsync("/product/count");

            // Assert
            Assert.AreEqual("0", await response.Content.ReadAsStringAsync());
        }
    }
}

using NUnit.Framework;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MinimalApi.Tests
{
    public class PostProductTests
    {
        [Test]
        public async Task PostProduct_ReturnsCreated201()
        {
            // Arrange
            using var application = new MinimalApiTestHost();
            using var client = application.CreateClient();

            var json = "{\"name\":\"VisualStudio\",\"description\":\"My Product\"}";
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            using var response = await client.PostAsync("/product", stringContent);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            var actual = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(actual.Contains("\"name\":\"VisualStudio\",\"description\":\"My Product\"}"));
        }

        [Test]
        public async Task PostProduct_WithEmptyName_ReturnsBadRequest400()
        {
            // Arrange
            using var application = new MinimalApiTestHost();
            using var client = application.CreateClient();

            var json = "{\"name\":\"\",\"description\":\"My Product\"}";
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            using var response = await client.PostAsync("/product", stringContent);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var actual = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(actual.Contains("Name must contain between 1 and 128 characters."));
        }
    }
}

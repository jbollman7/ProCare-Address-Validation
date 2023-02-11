namespace ProcareAddressValidation.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Intrinsics.X86;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Procare.AddressValidation.Tester;
    using Xunit;
    using static System.Collections.Specialized.BitVector32;

    public class AddressValidationServiceTest
    {
        [Fact]
        public async Task TestGetAddressesAsync_SuccessfulResponse()
        {
            // Arrange
            var factoryMock = new Mock<IHttpClientFactory>();
            var addressValidationBaseUrl = new Uri("https://addresses.dev-procarepay.com");
            var request = new AddressValidationRequest
            {
                Line1 = "1125 17th St Ste 1800",
                City = "Denver",
                StateCode = "CO",
                ZipCodeLeading5 = "80202"
            };
            var expectedResponse = "Successful response";
            var retries = 3;
            var addressServiceMock = new Mock<AddressValidationService>(factoryMock.Object, false, addressValidationBaseUrl, retries)
            {
                CallBase = true
            };
            addressServiceMock.Setup(x => x.GetAddressesAsync(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            addressServiceMock.Setup(x => x.Retries).Returns(retries);
            addressServiceMock.Setup(x => x.BaseUrl).Returns(addressValidationBaseUrl);

            // Act
            var response = await addressServiceMock.Object.GetAddressesAsync(request).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedResponse, response);
        }
    }
}


//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Threading.Tasks;

    internal sealed class Program
    {
        private static async Task Main()
        {
            Uri addressValidationBaseUrl = new Uri("https://addresses.dev-procarepay.com");

            using HttpClientFactory factory = new HttpClientFactory();
            using AddressValidationService addressService = new AddressValidationService(factory, false, addressValidationBaseUrl);

            // var request = new AddressValidationRequest { Line1 = "1 W Main", City = "Medford", StateCode = "OR", ZipCodeLeading5 = "97501" };
            // var request = new AddressValidationRequest();
            var request = new AddressValidationRequest { Line1 = "1125 17th St Ste 1800", City = "Denver", StateCode = "CO", ZipCodeLeading5 = "80202" };

            var response = await addressService.GetAddressesAsync(request).ConfigureAwait(false);
            Console.WriteLine(response);
        }
    }
}

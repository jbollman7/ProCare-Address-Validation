//-----------------------------------------------------------------------
// <copyright file="AddressValidationService.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class AddressValidationService : BaseHttpService
    {

        public int Retries { get; set; }

        public AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl, int retries)
            : this(httpClientFactory, disposeFactory, baseUrl, null, false)
        {
            this.Retries = retries;
        }

        protected AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl, HttpMessageHandler? httpMessageHandler, bool disposeHandler)
            : base(httpClientFactory, disposeFactory, baseUrl, httpMessageHandler, disposeHandler)
        {
        }

        public async Task<string> GetAddressesAsync(AddressValidationRequest request, CancellationToken token = default)
        {
            if (this.Retries > 0)
            {
                using var httpRequest = request.ToHttpRequest(this.BaseUrl);
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMilliseconds(700));
                try
                {
                    using var response = await this.CreateClient().SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        this.Retries--;
                        await this.GetAddressesAsync(request, token).ConfigureAwait(false);
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                         throw new HttpRequestException("Request timed out: Please check your network connection and try again later.");
                    }
                }
                catch (OperationCanceledException)
                {
                    this.Retries--;
                    await this.GetAddressesAsync(request, token).ConfigureAwait(false);
                }
            }
            throw new Exception("Unfortunately, after multiple attempts, the API request has failed. Please check your network connection and try again later.\r\n" +
                                " If the issue persists, kindly contact support with the following error code:[500]. \r\nThis will help us investigate the issue" +
                                " and resolve it as soon as possible.");
        }
    }
}

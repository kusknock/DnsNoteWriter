using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DnsNoteWriter.Models;
using DnsNoteWriter.Transport.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DnsNoteWriter.Transport
{
    public class GatewayClient
    {
        private readonly IConfiguration configuration;

        public GatewayClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<TResponse> MakeRequest<TResponse>(IRequest request) 
            where TResponse : class, IResponse
        {
            return await SendRequest(request, Activator.CreateInstance<TResponse>());
        }

        private async Task<TResponse> SendRequest<TResponse>(IRequest request, 
            TResponse response) where TResponse : class, IResponse
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(configuration["NoteServerSettings:BaseUrl"]);
                    var content = JsonContent.Create(request.Data);
                    try
                    {
                        var result = await client.PostAsync(request.ApiUrl, content);
                        Note? person = await result.Content.ReadFromJsonAsync<Note>();
                        return response.InitializeResponse(result.StatusCode, person) as TResponse;
                    }
                    catch (HttpRequestException ex) when (ex.StatusCode != HttpStatusCode.OK)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return response.InitializeResponse(HttpStatusCode.InternalServerError, ex) as TResponse;
                }
            }
        }
    }
}

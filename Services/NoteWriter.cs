using DnsNoteWriter.Models;
using DnsNoteWriter.Services.Interfaces;
using DnsNoteWriter.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DnsNoteWriter.Services
{
    public class NoteWriter : INoteWriter
    {
        private readonly GatewayClient client;
        private readonly IConfiguration configuration;
        private readonly ILogger<NoteWriter> logger;

        public NoteWriter(GatewayClient client, 
            IConfiguration configuration,
            ILogger<NoteWriter> logger)
        {
            this.client = client;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<bool> WriteNote(string text)
        {

            var note = new Note
            {
                Id = 0,
                Text = text,
                IsRead = false,
                ModifyDateTime = DateTime.Now,
                CreationDateTime = DateTime.Now,
            };

            var request = new NoteRequest()
            {
                ApiUrl = configuration["NoteServerSettings:ApiUrls:Create"],
                Data = note
            };

            try
            {
                var result = await client.MakeRequest<NoteResponse>(request);

                if(result.Code != HttpStatusCode.OK)
                    throw new InvalidOperationException("Произошла ошибка:", result.Data as Exception);

                logger.LogInformation("Заметка {0} добавлена в {1} потоке", text, Thread.CurrentThread.ManagedThreadId);

                return true;
            }
            catch(Exception ex)
            {
                logger.LogError($"{ex.Message} / {ex.InnerException?.Message}");

                return false;
            }
        }
    }
}

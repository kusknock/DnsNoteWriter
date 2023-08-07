using DnsNoteWriter.Services;
using DnsNoteWriter.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DnsNoteWriter
{
    internal class App
    {
        private readonly IConfiguration configuration;
        private readonly NoteWriterService noteWriterService;
        private readonly ILogger<App> logger;

        public App(IConfiguration configuration, 
            NoteWriterService noteWriterService, 
            ILogger<App> logger)
        {
            this.configuration = configuration;
            this.noteWriterService = noteWriterService;
            this.logger = logger;
        }

        public void Run(string[] args)
        {
            _ = int.TryParse(configuration["CountThreads"], out int countThreads);

            for (int i = 0; i < countThreads; i++)
            {
                if (!ThreadPool.QueueUserWorkItem(async stateInfo =>
                {
                    await noteWriterService.Process();
                }))
                {
                    logger.LogError("Возможно превышено предельно допустимое количество потоков");
                }
            }

            Console.ReadLine();
        }
    }
}
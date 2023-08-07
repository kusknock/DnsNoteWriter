using DnsNoteWriter.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DnsNoteWriter.Services
{
    public class NoteWriterService
    {
        private readonly INoteWriter noteWriter;

        public NoteWriterService(INoteWriter noteWriter)
        {
            this.noteWriter = noteWriter;
        }

        public async Task Process()
        {
            while (true)
            {
                var text = Guid.NewGuid().ToString();

                await noteWriter.WriteNote(text);

                Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(1,5) * 1000);
            }
        }
    }
}
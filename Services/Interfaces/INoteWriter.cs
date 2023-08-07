namespace DnsNoteWriter.Services.Interfaces
{
    public interface INoteWriter
    {
        Task<bool> WriteNote(string text);
    }
}
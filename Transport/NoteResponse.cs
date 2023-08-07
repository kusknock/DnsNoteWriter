using DnsNoteWriter.Transport.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DnsNoteWriter.Transport
{
    public class NoteResponse : IResponse
    {
        public object? Data { get; set; }

        public HttpStatusCode Code { get; set; }

        public IResponse InitializeResponse(HttpStatusCode code, object data)
        {
            Code = code;
            Data = data;

            return this;
        }
    }
}

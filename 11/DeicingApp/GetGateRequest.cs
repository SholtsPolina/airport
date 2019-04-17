using System;
using System.Collections.Generic;
using System.Text;

namespace DeicingApp
{
    class GetGateRequest
    {
        public string service { get; set; }
        public string identifier { get; set; }
        public GetGateRequest(string service, string identifier)
        {
            this.service = service;
            this.identifier = identifier;
        }
    }
}

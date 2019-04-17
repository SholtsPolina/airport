using System;
using System.Collections.Generic;
using System.Text;

namespace DeicingApp
{
    class MoveRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string service { get; set; }
        public string identifier { get; set; }
        public MoveRequest(string from, string to, string service, string identifier)
        {
            this.from = from;
            this.to = to;
            this.service = service;
            this.identifier = identifier;
        }
    }
}

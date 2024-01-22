using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop
{
    public class ConnectionCodeUpdatedMessage
    {
        public string NewConnectCode {  get; set; }
        public ConnectionCodeUpdatedMessage(string newConnectCode)
        {
            NewConnectCode = newConnectCode;
        }
    }
}

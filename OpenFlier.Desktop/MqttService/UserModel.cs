using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.Services
{
    public class User
    {
        required public string Username { get; set; }
        public string? UserId { get; set; }
        public string? CurrentClientId { get; set; }
        public bool AllowCommandInput { get; set; }
        public string? CommandInputSource { get; set; }
        public bool IsBusy { get; set; }
    }
}

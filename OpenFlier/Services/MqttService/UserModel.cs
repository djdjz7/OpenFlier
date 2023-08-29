using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Services
{
    public class User
    {
        required public string Username { get; set; }
        public string? UserId { get; set; }
        public string? CurrentClientId { get; set; }
        public int CurrentImageProvider { get; set; }
        public bool ImageProviderSwitchAllowed { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public void SwitchImageProvider()
        {
            if (ImageProviderSwitchAllowed)
            {
                int ImageProviderCount = LocalStorage.Config.ImageProviderPlugins.Count;
                CurrentImageProvider = (CurrentImageProvider + 1) % ImageProviderCount != 0 ? ImageProviderCount : 1;
            }
            else
                CurrentImageProvider = 0;
        }
    }
}

using System;
namespace OpenFlier.SpecialChannels
{
    public class User
    {
        public string? UserID;
        required public string UserName;
        public bool AllowLocalRandom = false;
        public bool AllowRemoteRandom = false;
        public bool AllowCommandInput = false;
        public SpecialChannel CurrentChannel;
        public bool IsTaskFinished = true;
        public DateTime LastUpdateTime;
    }
    public enum SpecialChannel
    {
        Screenshot = 0,
        LocalRandom = 1,
        RemoteRandom = 2,
        CommandInput = 3,
    }
}

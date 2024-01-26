using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenFlier.Desktop.MqttService
{
    public class InvokeCommandEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x == y)
                return true;
            if (x?.ToLower() == y?.ToLower())
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}

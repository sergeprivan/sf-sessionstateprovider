using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.Interfaces
{
    [Serializable]
    public class SessionKeyItemId : IFormattable, IComparable, IComparable<SessionKeyItemId>, IEquatable<SessionKeyItemId>
    {
        public SessionKeyItemId()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.Id.CompareTo(((SessionKeyItemId)obj).Id);
        }

        public int CompareTo(SessionKeyItemId other)
        {
            return this.Id.CompareTo(other.Id);
        }

        public bool Equals(SessionKeyItemId other)
        {
            return this.Id.Equals(other.Id);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.Id.ToString(format, formatProvider);
        }

        public ServicePartitionKey GetPartitionKey()
        {
            return new ServicePartitionKey(HashUtil.GetLongHashCode(this.Id.ToString()));
        }

        public static bool operator ==(SessionKeyItemId item1, SessionKeyItemId item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(SessionKeyItemId item1, SessionKeyItemId item2)
        {
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj)
        {
            return (obj is SessionKeyItemId) ? this.Id.Equals(((SessionKeyItemId)obj).Id) : false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public string ToString(string format)
        {
            return this.Id.ToString(format);
        }
    }
}

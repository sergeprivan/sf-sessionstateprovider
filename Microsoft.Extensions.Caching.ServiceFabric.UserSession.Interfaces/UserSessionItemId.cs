using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces
{
    [Serializable]
    public class UserSessionItemId : IFormattable, IComparable, IComparable<UserSessionItemId>, IEquatable<UserSessionItemId>
    {
        public UserSessionItemId()
        {
            Id = Guid.NewGuid();
        }

        public UserSessionItemId(string id)
        {
            Id = Guid.Parse(id);
        }

        public Guid Id { get; }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.Id.CompareTo(((UserSessionItemId)obj).Id);
        }

        public int CompareTo(UserSessionItemId other)
        {
            return this.Id.CompareTo(other.Id);
        }

        public bool Equals(UserSessionItemId other)
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

        public static bool operator ==(UserSessionItemId item1, UserSessionItemId item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(UserSessionItemId item1, UserSessionItemId item2)
        {
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj)
        {
            return (obj is UserSessionItemId) ? this.Id.Equals(((UserSessionItemId)obj).Id) : false;
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

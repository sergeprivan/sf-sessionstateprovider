using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFContacts.SessionKeys.Domain {
    [DataContract]
    public class SessionKeyItemId : IFormattable, IComparable, IComparable<SessionKeyItemId>, IEquatable<SessionKeyItemId> {

        [DataMember]
        private Guid _id;

        public SessionKeyItemId() {
            this._id = Guid.NewGuid();
        }

        public int CompareTo(SessionKeyItemId other) {
            return this._id.CompareTo(other._id);
        }

        public int CompareTo(object obj) {
            return this._id.CompareTo(((SessionKeyItemId)obj)._id);
        }

        public bool Equals(SessionKeyItemId other) {
            return this._id.Equals(other._id);
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            return this._id.ToString(format, formatProvider);
        }

        public static bool operator ==(SessionKeyItemId item1, SessionKeyItemId item2) {
            return item1.Equals(item2);
        }

        public static bool operator !=(SessionKeyItemId item1, SessionKeyItemId item2) {
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj) {
            return (obj is SessionKeyItemId) ? this._id.Equals(((SessionKeyItemId)obj)._id) : false;
        }

        public override int GetHashCode() {
            return _id.GetHashCode();
        }

        public override string ToString() {
            return this._id.ToString();
        }

        public string ToString(string format) {
            return this._id.ToString(format);
        }
    }
}

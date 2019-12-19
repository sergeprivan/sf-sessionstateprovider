using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.Domain {
    [Serializable]
    public class SessionKeyItem {

        public SessionKeyItem(string key, string value, Guid? id = null) {
            Id = id ?? Guid.NewGuid();
            Key = key;
            Value = value;
        }

        public Guid Id { get; }
        public string Key { get; }
        public string Value { get; }

        public override string ToString() {
            return $"Session Key: {Key} with Value: {Value} at: {DateTime.UtcNow}";
        }
    }
}

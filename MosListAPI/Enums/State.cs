using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MosListAPI.Enum{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum State {
        DRAFT,
        ACTIVE,
        EXPIRED,
        INACTIVE
    }
}

using MongoDB.Bson.Serialization.Attributes;

namespace JanDash.Data
{
    [BsonIgnoreExtraElements]
    public class MachineInfo
    {
        public ulong MemoryUsed { get; set; }
        public ulong MemoryFree { get; set; }
        public long BootTimestamp { get; set; }
        public float CpuTemperature { get; set; }
    }
}
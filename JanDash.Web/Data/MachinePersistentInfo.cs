namespace JanDash.Data
{
    /// <summary>
    /// For "presistent" information about a <see cref="Machine"/>, e.g. RAM size, OS...
    /// </summary>
    public class MachinePersistentInfo
    {
        /// <summary>
        /// Physical RAM size, in bytes
        /// </summary>
        public ulong PhysicalRam { get; set; }
    }
}
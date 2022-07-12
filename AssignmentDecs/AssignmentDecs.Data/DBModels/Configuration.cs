using AssignmentDecs.Data.Enums;

#nullable disable

namespace AssignmentDecs.Data
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConfigurationTypeIdentity Type { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; }
    }
}

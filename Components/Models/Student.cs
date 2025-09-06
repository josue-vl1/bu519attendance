using System.Text.Json.Serialization;

namespace attendance.Components.Models
{
    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public string BUID { get; set; } = string.Empty;

        [JsonIgnore]
        public bool Present { get; set; }

        [JsonIgnore]
        public bool Exempt { get; set; }
        public bool IsInactive { get; set; } = false; // New property to track active state, default to true

        [JsonIgnore]
        public bool IsEditing { get; set; } = false; // New property to track edit state, default to false
    }
}
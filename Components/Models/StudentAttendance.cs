namespace attendance.Components.Models
{
 
    public class StudentAttendance
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
        public int DaysAttended { get; set; }

    }
}
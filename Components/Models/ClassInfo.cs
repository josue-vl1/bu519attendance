namespace attendance.Components.Models
{
    public class ClassInfo
    {
        public string Schedule { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Instructors = new List<string>();

        public ClassInfo(string schedule, DateTime startDate, DateTime endDate, List<string> instructors)
        {
            this.Schedule = schedule;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Instructors = instructors;
        }
    }
}
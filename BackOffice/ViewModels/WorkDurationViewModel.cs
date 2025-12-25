namespace BackOffice.ViewModels
{
    public class WorkDuration
    {
        public int UserId { get; set; }
        public string LastName { get; set; }
        public TimeSpan TotalWorked { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public double Percentage { get; set; }

    }
}
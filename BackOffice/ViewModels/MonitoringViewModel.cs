using BackOffice.Models;
using System.Collections.Generic;

namespace BackOffice.ViewModels
{
    public class MonitoringViewModel
    {
        public List<User> Present { get; set; } = new List<User>();
        public List<User> Absent { get; set; } = new List<User>();
    }
}
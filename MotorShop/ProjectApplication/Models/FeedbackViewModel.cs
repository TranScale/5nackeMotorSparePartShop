using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    // FeedbackViewModel.cs
    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
    }

}
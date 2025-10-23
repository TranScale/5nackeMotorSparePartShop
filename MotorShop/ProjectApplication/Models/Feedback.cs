using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectApplication.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá.")]
        [StringLength(500)]
        public string Comment { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5.")]
        public int Rating { get; set; }

        public string CustomerName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property (liên kết tới sản phẩm)
        public virtual Product Product { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}

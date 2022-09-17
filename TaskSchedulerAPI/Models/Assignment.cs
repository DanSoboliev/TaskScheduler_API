using System;
using System.ComponentModel.DataAnnotations;

namespace TaskSchedulerAPI.Models {
    public class Assignment {
        [Required(ErrorMessage = "Відсутній id")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Недопустиме значення id")]
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Відсутня назва")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Некоректна довжина назви")]
        public string AssignmentName { get; set; }

        [StringLength(250, ErrorMessage = "Некоректна довжина опису")]
        public string AssignmentDescription { get; set; }

        [Required(ErrorMessage = "Відсутній час")]
        public DateTime AssignmentTime { get; set; }

        public bool? AssignmentState { get; set; }

        [Required(ErrorMessage = "Відсутній користувач")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Недопустиме значення id")]
        public int UserId { get; set; }

        public Assignment() { }
    }
}

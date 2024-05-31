using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Models
{
    public class Progress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ProgressId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Progress Name cannot exceed 50 characters")]
        public string ProgressName { get; set; }//stato di avanzamento
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ClientId { get; set; }//segnalazioniID

        //FK
        [Required]
        [ForeignKey("ClientAddByUserId")]
        public ApplicationUser ClientAddByUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AddDateTime { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }//nome

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string SurName { get; set; }//cognome

        [Required]
        [MaxLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string Phone { get; set; }//cellulare

        public bool Sale { get; set; }//Vendita
        public bool Rent { get; set; }//Affitto

        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string Address { get; set; }//indirizzo

        public int EstimatedCommitssion { get; set; }//Provvigione stimate

        //FK
        [Required]
        public int ProgressItemId { get; set; }
        [ForeignKey("ProgressItemId")]
        public Progress ProgressItem { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ProgressDateTime { get; set; }

        public string Note { get; set; }
    }
}

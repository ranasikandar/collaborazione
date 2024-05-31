using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class NewClientViewModel
    {
        [Required(ErrorMessage = "il Nome è richiesto")]
        [MaxLength(50, ErrorMessage = "Il Nome non può superare i 50 caratteri")]
        [Display(Name = "Nome")]
        public string Name { get; set; }//nome

        [Required(ErrorMessage = "il cognome è richiesto")]
        [MaxLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri")]
        [Display(Name = "Cognome")]
        public string SurName { get; set; }//cognome

        [Required(ErrorMessage = "il cellulare è richiesto")]
        [MaxLength(50, ErrorMessage = "Il cellulare non può superare i 50 caratteri")]
        [Display(Name = "Cellulare")]
        public string Phone { get; set; }//cellulare

        //[Display(Name = "Vendita")]
        //public bool Sale { get; set; }//Vendita

        //[Display(Name = "Affitto")]
        //public bool Rent { get; set; }//Affitto

        public IEnumerable<ClientFor> ForOptions { set; get; }

        [Required(ErrorMessage = "seleziona un'opzione")]
        public int SelectedFor { set; get; }

        [MaxLength(250, ErrorMessage = "Il indirizzo non può superare i 250 caratteri")]
        [Display(Name = "Indirizzo")]
        public string Address { get; set; }//indirizzo


    }

    public class ClientFor
    {
        public int ForId { set; get; }
        public string ForName { set; get; }
    }
}

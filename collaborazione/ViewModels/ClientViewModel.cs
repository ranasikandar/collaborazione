using collaborazione.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class ClientViewModel
    {
        [Required(ErrorMessage = "il ID è richiesto")]
        public string ClientIdEnc { get; set; }

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


        public IEnumerable<Client_For> ForOptions { set; get; }

        [Required(ErrorMessage = "seleziona un'opzione")]
        public int SelectedFor { set; get; }


        [MaxLength(250, ErrorMessage = "Il indirizzo non può superare i 250 caratteri")]
        [Display(Name = "Indirizzo")]
        public string Address { get; set; }//indirizzo

        [Display(Name = "Provvigione stimate")]//EstimatedCommitssion
        public int EstimatedCommitssion { get; set; }


        [Display(Name = "Progresso?")]
        public List<Progress> ProgressItems { get; set; }

        [Display(Name = "Stato di Avanzamento")]
        public int SelectedProgress { get; set; }


        [Display(Name = "aggiungere da")]//add by
        public ApplicationUser ClientAddByUser { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "data e ora di aggiunta")]
        public DateTime AddDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data e ora di avanzamento")]
        public DateTime ProgressDateTime { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }
    }

    public class Client_For
    {
        public int ForId { set; get; }
        public string ForName { set; get; }
    }
}

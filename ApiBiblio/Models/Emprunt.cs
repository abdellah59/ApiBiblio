using System.ComponentModel.DataAnnotations.Schema;

namespace ApiBiblio.Models
{
    public class Emprunt
    {
        public int Id { get; set; }
        public DateTime DateEmprunt { get; set; }
        public required string Statut {  get; set; } = "En cours"; // En cours, Terminé, En retard
        public DateTime DateRetour { get; set; }

        // Clés étrangères
        public int IdMembre { get; set; }
        public int IdEmploye { get; set; }

        [ForeignKey("IdMembre")]
        public virtual Membre Membre { get; set; }

        [ForeignKey("IdEmploye")]
        public virtual Employe Employe { get; set; }
        public virtual ICollection<EmpruntLivre> EmpruntLivres { get; set; } = new List<EmpruntLivre>();
    }
}

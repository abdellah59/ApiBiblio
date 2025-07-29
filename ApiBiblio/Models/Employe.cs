using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ApiBiblio.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public required string NomEmploye { get; set; }
        public required string PrenomEmploye { get; set; }
        public required string LoginEmploye { get; set; } // Email utilisé pour se connecter
        public required string MdpEmploye { get; set; } 
        public int IdRole { get; set; }         // Clé étrangère
        [ForeignKey("IdRole")]
        public virtual Role Role { get; set; }
        public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    }
}

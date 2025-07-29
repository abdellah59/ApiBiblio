using ApiBiblio.Models;

namespace ApiBiblio.DTOs
{
    public class EmpruntDTO
    {
        public int Id { get; set; }
        public DateTime DateEmprunt { get; set; }
        public string Statut { get; set; }
        public DateTime DateRetour { get; set; }
        public string Membre { get; set; }
        public string Employe { get; set; }
        public List<string> Livres { get; set; } = new List<string>();
    }

    public class CreateEmpruntDto
    {
        public int Id { get; set; }
        public List<int> IdLivres { get; set; } = new List<int>();
        public DateTime DateRetour { get; set; }
        public int IdMembre { get; set; }
        public int IdEmploye { get; set; }
    }
}


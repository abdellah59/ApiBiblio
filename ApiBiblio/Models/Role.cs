namespace ApiBiblio.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string NomRole { get; set; } // Ex: Admin ou Bibliothecaire
        public virtual ICollection<Employe> Employes { get; set; } = new List<Employe>();
    }
}

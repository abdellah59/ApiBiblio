namespace ApiBiblio.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public required string NomGenre { get; set; }
        public virtual ICollection<Livre> Livres { get; set; } = new List<Livre>();
    }
}

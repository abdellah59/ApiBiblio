using ApiBiblio.Models;

namespace ApiBiblio.DTOs
{
    public class MembreDTO
    {
        public class MembreDto
        {
            public int Id { get; set; }
            public required string NomMembre { get; set; }
            public required string PrenomMembre { get; set; }
            public required string AdresseMail { get; set; }
            public required string Telephone { get; set; }
            public string? AdressePostale { get; set; }
            public string? HistoriqueMembre { get; set; }

        }

        public class CreateMembreDto
        {
            public required string NomMembre { get; set; }
            public required string PrenomMembre { get; set; }
            public required string AdresseMail { get; set; }
            public required string MdpMembre { get; set; }
            public required string Telephone { get; set; }
            public string? AdressePostale { get; set; }
            public string? HistoriqueMembre { get; set; }
        }

        public class UpdateMembreDto
        {
            public required string NomMembre { get; set; }
            public required string PrenomMembre { get; set; }
            public required string AdresseMail { get; set; }
            public required string Telephone { get; set; }
            public string? AdressePostale { get; set; }
            public string? HistoriqueMembre { get; set; }
        }
    }
}

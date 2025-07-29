namespace ApiBiblio.DTOs
{
    public class AuteurDTO
    {
        public class CreateAuteurDto
        {
            public required string NomAuteur { get; set; }
            public required string PrenomAuteur { get; set; }
        }

        public class AuteurResponseDto
        {
            public int Id { get; set; }
            public string NomAuteur { get; set; }
            public string PrenomAuteur { get; set; }
            public string NomComplet => $"{PrenomAuteur} {NomAuteur}";
        }

        public class UpdateAuteurDto
        {
            public string? NomAuteur { get; set; }
            public string? PrenomAuteur { get; set; }
            
        }
    }
}
    
namespace ApiBiblio.DTOs
{
    public class CategorieDTO
    {
        public class CreateCategorieDto
        {
            public required string NomCategorie { get; set; }
        }

        public class CategorieResponseDto
        {
            public int Id { get; set; }
            public string NomCategorie { get; set; }
        }

        public class UpdateCategorieDto
        {
            public string? NomCategorie { get; set; }
        }
    }
}
  
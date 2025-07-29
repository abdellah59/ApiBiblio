namespace ApiBiblio.DTOs
{
    public class GenreDTO
    {
        public class CreateGenreDto
        {
            public required string NomGenre { get; set; }
        }

        public class GenreResponseDto
        {
            public int Id { get; set; }
            public string NomGenre { get; set; }
        }

        public class UpdateGenreDto
        {
            public string? NomGenre { get; set; }
        }
    }
}

using ApiBiblio.Models;
using Microsoft.EntityFrameworkCore;
using ApiBiblio.DTOs;

namespace ApiBiblio.Database
{
    public class BiblioDb : DbContext
    {
        public BiblioDb(DbContextOptions<BiblioDb> options) : base(options) { }

        // DbSets
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Membre> Membres { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }
        public DbSet<EmpruntLivre> EmpruntLivres { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la table de liaison EmpruntLivre
            modelBuilder.Entity<EmpruntLivre>()
                .HasKey(el => new { el.IdEmprunt, el.IdLivre });

            modelBuilder.Entity<EmpruntLivre>()
                .HasOne(el => el.Emprunt)
                .WithMany(e => e.EmpruntLivres)
                .HasForeignKey(el => el.IdEmprunt);

            modelBuilder.Entity<EmpruntLivre>()
                .HasOne(el => el.Livre)
                .WithMany(l => l.EmpruntLivres)
                .HasForeignKey(el => el.IdLivre);

            // Index unique pour l'ISBN
            modelBuilder.Entity<Livre>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            // Index unique pour l'email de l'employé
            modelBuilder.Entity<Employe>()
                .HasIndex(e => e.LoginEmploye)
                .IsUnique();

            // Index unique pour l'email du membre
            modelBuilder.Entity<Membre>()
                .HasIndex(m => m.AdresseMail)
                .IsUnique();

            // Seed data pour les rôles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, NomRole = "Administrateur" },
                new Role { Id = 2, NomRole = "Bibliothécaire" }
            );
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TFT.API.Business.Model
{
    public partial class Entities : DbContext
    {
        public Entities()
        {
        }

        public Entities(DbContextOptions<Entities> options)
            : base(options)
        {
        }

        public virtual DbSet<ActorAgreement> ActorAgreements { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<GenreMovie> GenreMovies { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Users_Actor> Users_Actors { get; set; }
        public virtual DbSet<Users_Director> Users_Directors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;attachdbfilename=C:\\Programming Projects\\qualification process\\TFT Falcon\\TFT\\TFT.Repository\\DataSource\\Production\\TFT.mdf;Database=TFT;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ActorAgreement>(entity =>
            {
                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.ActorAgreements)
                    .HasForeignKey(d => d.ActorID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActorAgreementActor");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.ActorAgreements)
                    .HasForeignKey(d => d.MovieID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActorAgreementMovie");
            });

            modelBuilder.Entity<GenreMovie>(entity =>
            {
                entity.HasKey(e => new { e.GenreID, e.MovieID });

                entity.HasOne(d => d.Genres)
                    .WithMany(p => p.GenreMovies)
                    .HasForeignKey(d => d.GenreID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GenreMovie_Genre");

                entity.HasOne(d => d.Movies)
                    .WithMany(p => p.GenreMovies)
                    .HasForeignKey(d => d.MovieID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GenreMovie_Movie");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasOne(d => d.Director)
                    .WithMany(p => p.Movies)
                    .HasForeignKey(d => d.DirectorID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DirectorMovie");
            });

            modelBuilder.Entity<Users_Actor>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.IDNavigation)
                    .WithOne(p => p.Users_Actor)
                    .HasForeignKey<Users_Actor>(d => d.ID)
                    .HasConstraintName("FK_Actor_inherits_User");
            });

            modelBuilder.Entity<Users_Director>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.IDNavigation)
                    .WithOne(p => p.Users_Director)
                    .HasForeignKey<Users_Director>(d => d.ID)
                    .HasConstraintName("FK_Director_inherits_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

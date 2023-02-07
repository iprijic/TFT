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

        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<ActorAgreement> ActorAgreements { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<GenreMovie> GenreMovies { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
   //            optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;attachdbfilename=C:\\Programming Projects\\qualification process\\TFT Falcon\\TFT\\TFT.Repository\\DataSource\\Production\\TFT.mdf;Database=TFT;Trusted_Connection=True;");
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
                    .HasConstraintName("FK_ActorActorAgreement");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.ActorAgreements)
                    .HasForeignKey(d => d.MovieID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActorAgreementMovie");
            });

            modelBuilder.Entity<GenreMovie>(entity =>
            {
                entity.HasKey(e => new { e.Genres_ID, e.Movies_ID });

                entity.HasOne(d => d.Genres)
                    .WithMany(p => p.GenreMovies)
                    .HasForeignKey(d => d.Genres_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GenreMovie_Genre");

                entity.HasOne(d => d.Movies)
                    .WithMany(p => p.GenreMovies)
                    .HasForeignKey(d => d.Movies_ID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GenreMovie_Movie");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasOne(d => d.Director)
                    .WithMany(p => p.Movies)
                    .HasForeignKey(d => d.DirectorID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MovieDirector");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

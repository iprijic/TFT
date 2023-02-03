using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Index(nameof(DirectorID), Name = "IX_FK_DirectorMovie")]
    public partial class Movie
    {
        public Movie()
        {
            ActorAgreements = new HashSet<ActorAgreement>();
            GenreMovies = new HashSet<GenreMovie>();
        }

        [Key]
        public long ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTimeOffset Duration { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartProduction { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndProduction { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Budget { get; set; }
        public long DirectorID { get; set; }

        [ForeignKey(nameof(DirectorID))]
        [InverseProperty(nameof(Users_Director.Movies))]
        public virtual Users_Director Director { get; set; }
        [InverseProperty(nameof(ActorAgreement.Movie))]
        public virtual ICollection<ActorAgreement> ActorAgreements { get; set; }
        [InverseProperty(nameof(GenreMovie.Movies))]
        public virtual ICollection<GenreMovie> GenreMovies { get; set; }
    }
}

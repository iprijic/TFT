using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Table("GenreMovie")]
    [Index(nameof(Movies_ID), Name = "IX_FK_GenreMovie_Movie")]
    public partial class GenreMovie
    {
        [Key]
        public long Genres_ID { get; set; }
        [Key]
        public long Movies_ID { get; set; }

        [ForeignKey(nameof(Genres_ID))]
        [InverseProperty(nameof(Genre.GenreMovies))]
        public virtual Genre Genres { get; set; }
        [ForeignKey(nameof(Movies_ID))]
        [InverseProperty(nameof(Movie.GenreMovies))]
        public virtual Movie Movies { get; set; }
    }
}

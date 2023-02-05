using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Table("GenreMovie")]
    [Index(nameof(MovieID), Name = "IX_FK_GenreMovie_Movie")]
    public partial class GenreMovie
    {
        [Key]
        public long GenreID { get; set; }
        [Key]
        public long MovieID { get; set; }

        [ForeignKey(nameof(GenreID))]
        [InverseProperty(nameof(Genre.GenreMovies))]
        public virtual Genre Genres { get; set; }
        [ForeignKey(nameof(MovieID))]
        [InverseProperty(nameof(Movie.GenreMovies))]
        public virtual Movie Movies { get; set; }
    }
}

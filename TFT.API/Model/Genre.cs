using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    public partial class Genre
    {
        public Genre()
        {
            GenreMovies = new HashSet<GenreMovie>();
        }

        [Key]
        public long ID { get; set; }
        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(GenreMovie.Genres))]
        public virtual ICollection<GenreMovie> GenreMovies { get; set; }
    }
}

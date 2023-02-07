using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    public partial class Director //: User
    {
        public Director()
        {
            Movies = new HashSet<Movie>();
        }

        [Key]
        public long ID { get; set; }


        [Required]
        public string DirectorID { get; set; }


        [Required]
        public string Username { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public string Salt { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Role { get; set; }

        [InverseProperty(nameof(Movie.Director))]
        public virtual ICollection<Movie> Movies { get; set; }
    }
}

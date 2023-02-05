using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Table("Users_Director")]
    public partial class Users_Director
    {
        public Users_Director()
        {
            Movies = new HashSet<Movie>();
        }

        [Required]
        public string DirectorID { get; set; }
        [Key]
        public long ID { get; set; }

        [ForeignKey(nameof(ID))]
        [InverseProperty(nameof(User.Users_Director))]
        public virtual User IDNavigation { get; set; }
        [InverseProperty(nameof(Movie.Director))]
        public virtual ICollection<Movie> Movies { get; set; }
    }
}

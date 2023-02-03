using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    public partial class User
    {
        [Key]
        public long ID { get; set; }
        [Required]
        public string PrincipalName { get; set; }

        [InverseProperty("IDNavigation")]
        public virtual Users_Actor Users_Actor { get; set; }
        [InverseProperty("IDNavigation")]
        public virtual Users_Director Users_Director { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    public partial class Actor
    {
        public Actor()
        {
            ActorAgreements = new HashSet<ActorAgreement>();
        }

        [Key]
        public long ID { get; set; }
        [Required]
        public string ActorID { get; set; }
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

        [InverseProperty(nameof(ActorAgreement.Actor))]
        public virtual ICollection<ActorAgreement> ActorAgreements { get; set; }
    }
}

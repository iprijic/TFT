using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Table("Users_Actor")]
    public partial class Users_Actor
    {
        public Users_Actor()
        {
            ActorAgreements = new HashSet<ActorAgreement>();
        }

        [Key]
        public long ID { get; set; }

        [ForeignKey(nameof(ID))]
        [InverseProperty(nameof(User.Users_Actor))]
        public virtual User IDNavigation { get; set; }
        [InverseProperty(nameof(ActorAgreement.Actor))]
        public virtual ICollection<ActorAgreement> ActorAgreements { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TFT.API.Business.Model
{
    [Index(nameof(ActorID), Name = "IX_FK_ActorActorAgreement")]
    [Index(nameof(MovieID), Name = "IX_FK_ActorAgreementMovie")]
    public partial class ActorAgreement
    {
        [Key]
        public long ID { get; set; }
        public bool IsInvited { get; set; }
        [Required]
        public string IsAccepted { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Honorarium { get; set; }
        public long MovieID { get; set; }
        public long ActorID { get; set; }

        [ForeignKey(nameof(ActorID))]
        [InverseProperty("ActorAgreements")]
        public virtual Actor Actor { get; set; }
        [ForeignKey(nameof(MovieID))]
        [InverseProperty("ActorAgreements")]
        public virtual Movie Movie { get; set; }
    }
}

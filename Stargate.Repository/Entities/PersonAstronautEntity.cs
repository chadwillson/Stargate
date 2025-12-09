using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stargate.Repository.Entities
{

    [Table("Person")]
    public class PersonAstronautEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public virtual AstronautDetailEntity? AstronautDetail { get; set; }

        public virtual ICollection<AstronautDutyEntity> AstronautDuties { get; set; } = new HashSet<AstronautDutyEntity>();

    }

    public class PersonConfiguration : IEntityTypeConfiguration<PersonAstronautEntity>
    {
        public void Configure(EntityTypeBuilder<PersonAstronautEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasOne(z => z.AstronautDetail).WithOne(z => z.Person).HasForeignKey<AstronautDetailEntity>(z => z.PersonId);
            builder.HasMany(z => z.AstronautDuties).WithOne(z => z.Person).HasForeignKey(z => z.PersonId);
        }
    }

}

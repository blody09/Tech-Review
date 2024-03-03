using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("Logging")]
    [Index(nameof(LogType))]  
    public class Logging
    {
        [Key]
        public int Id { get; set; }
        [Required]
        
        // Time permitting i would add a class for logtype instead of a string. like an enum defined elsewhere for succcessfull entries. 
        public string LogType { get; set; }

        [Required]
        public string Message { get; set; }

        public string? LogException { get; set; }

        public DateTime TimeStamp { get; set; }

    }

    public class LoggingConfiguration : IEntityTypeConfiguration<Logging>
    {
        public void Configure(EntityTypeBuilder<Logging> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }

    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.IdNotification);

        builder.Property(n => n.IdNotification)
            .HasColumnName("id_notification");

        builder.Property(n => n.IdUtilisateur)
            .HasColumnName("id_utilisateur")
            .IsRequired();

        builder.Property(n => n.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.Property(n => n.Lue)
            .HasColumnName("lue")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(n => n.DateCreation)
            .HasColumnName("date_creation")
            .IsRequired();

        builder.HasOne(n => n.Utilisateur)
            .WithMany()
            .HasForeignKey(n => n.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

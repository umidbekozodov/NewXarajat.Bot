﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Context;
public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms")
            .Property(room => room.Name)
            .IsRequired()
        .HasColumnType("nvarchar(50)");

        builder.Property(room => room.Key)
            .HasColumnType("nvarchar(20)")
        .IsRequired();

        builder.Property(room => room.Status)
            .HasDefaultValue(RoomStatus.Created)
            .IsRequired();
    }
}
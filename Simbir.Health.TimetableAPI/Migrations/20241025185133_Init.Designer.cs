﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Simbir.Health.TimetableAPI.Model.Database;

#nullable disable

namespace Simbir.Health.TimetableAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241025185133_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Simbir.Health.TimetableAPI.Model.Database.DBO.Timetable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("doctorId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("from")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("hospitalId")
                        .HasColumnType("integer");

                    b.Property<string>("room")
                        .HasColumnType("text");

                    b.Property<DateTime>("to")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("timeTableObj");
                });
#pragma warning restore 612, 618
        }
    }
}

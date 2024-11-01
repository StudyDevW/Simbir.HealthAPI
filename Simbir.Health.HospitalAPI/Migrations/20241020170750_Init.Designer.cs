﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Simbir.Health.HospitalAPI.Model;

#nullable disable

namespace Simbir.Health.HospitalAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241020170750_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Simbir.Health.HospitalAPI.Model.Database.DBO.HospitalTable", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("address")
                        .HasColumnType("text");

                    b.Property<string>("contactPhone")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<List<string>>("rooms")
                        .HasColumnType("text[]");

                    b.HasKey("id");

                    b.ToTable("hospitalTableObj");
                });
#pragma warning restore 612, 618
        }
    }
}

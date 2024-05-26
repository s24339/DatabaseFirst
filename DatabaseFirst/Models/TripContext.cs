﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.Models;

public partial class TripContext : DbContext
{
    public TripContext()
    {
    }

    public TripContext(DbContextOptions<TripContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientTrip> ClientTrips { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<CountryTrip> CountryTrips { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=APBD_DatabaseFirst;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient)
                .HasName("Client_pk");

            entity.ToTable("Client", "trip");

            entity.Property(e => e.IdClient).ValueGeneratedNever();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(e => e.Pesel)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(e => e.Telephone)
                .IsRequired()
                .HasMaxLength(120);
        });

        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdClient, e.IdTrip })
                .HasName("Client_Trip_pk");

            entity.ToTable("Client_Trip", "trip");

            entity.Property(e => e.PaymentDate).HasColumnType("datetime");

            entity.Property(e => e.RegisteredAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdClientNavigation)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Table_5_Client");

            entity.HasOne(d => d.IdTripNavigation)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdTrip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Table_5_Trip");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry)
                .HasName("Country_pk");

            entity.ToTable("Country", "trip");

            entity.Property(e => e.IdCountry).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(120);
        });

        modelBuilder.Entity<CountryTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdCountry, e.IdTrip })
                .HasName("Country_Trip_pk");

            entity.ToTable("Country_Trip", "trip"); 

            entity.HasOne(d => d.IdCountryNavigation)
                .WithMany(p => p.CountryTrips)
                .HasForeignKey(d => d.IdCountry)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Country_Trip_Country");

            entity.HasOne(d => d.IdTripNavigation)
                .WithMany(p => p.CountryTrips)
                .HasForeignKey(d => d.IdTrip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Country_Trip_Trip");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip)
                .HasName("Trip_pk");

            entity.ToTable("Trip", "trip");

            entity.Property(e => e.IdTrip).ValueGeneratedNever();

            entity.Property(e => e.DateFrom).HasColumnType("datetime");

            entity.Property(e => e.DateTo).HasColumnType("datetime");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(220);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(120);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

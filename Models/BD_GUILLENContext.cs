using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SISGUILLEN.Models
{
    public partial class BD_GUILLENContext : DbContext
    {
        public BD_GUILLENContext()
        {
        }

        public BD_GUILLENContext(DbContextOptions<BD_GUILLENContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cargo> Cargos { get; set; } = null!;
        public virtual DbSet<Perfile> Perfiles { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.ToTable("Cargo");

                entity.Property(e => e.CargoId).HasColumnName("CargoID");

                entity.Property(e => e.Descripcion).HasMaxLength(100);

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Perfile>(entity =>
            {
                entity.HasKey(e => e.PerfilId)
                    .HasName("PK__Perfiles__0C005B662C42CD51");

                entity.Property(e => e.PerfilId).HasColumnName("PerfilID");

                entity.Property(e => e.Descripcion).HasMaxLength(100);

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.Property(e => e.Apellido).HasMaxLength(50);

                entity.Property(e => e.CargoId).HasColumnName("CargoID");

                entity.Property(e => e.Direccion).HasMaxLength(100);

                entity.Property(e => e.Estado).HasDefaultValueSql("((1))");

                entity.Property(e => e.FechaNacimiento).HasColumnType("date");

                entity.Property(e => e.Nombre).HasMaxLength(50);

                entity.Property(e => e.PerfilId).HasColumnName("PerfilID");

                entity.HasOne(d => d.Cargo)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.CargoId)
                    .HasConstraintName("FK__Usuarios__CargoI__534D60F1");

                entity.HasOne(d => d.Perfil)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.PerfilId)
                    .HasConstraintName("FK__Usuarios__Perfil__52593CB8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

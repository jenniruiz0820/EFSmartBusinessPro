using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFSmartBusinessPro
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Activo { get; set; }

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
    }

    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class Proveedor
    {
        public int ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Proveedor> Proveedores { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Cadena de conexión usando tu servidor SQLEXPRESS01
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=EFSmartBusinessDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        // Datos iniciales para que los ComboBox no estén vacíos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { CategoriaId = 1, Nombre = "Electrónica" },
                new Categoria { CategoriaId = 2, Nombre = "Papeleria" },
                new Categoria { CategoriaId = 3, Nombre = "Hogar" }
            );

            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { ProveedorId = 1, Nombre = "Proveedor Global" },
                new Proveedor { ProveedorId = 2, Nombre = "Distribuidora Central" }
            );
        }
    }
}
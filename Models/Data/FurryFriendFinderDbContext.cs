using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FurryFriendFinder.Models.Data;

public partial class FurryFriendFinderDbContext : DbContext
{
    public FurryFriendFinderDbContext()
    {
    }

    public FurryFriendFinderDbContext(DbContextOptions<FurryFriendFinderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Access> Accesses { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Adoption> Adoptions { get; set; }

    public virtual DbSet<AdoptionDate> AdoptionDates { get; set; }

    public virtual DbSet<AnimalType> AnimalTypes { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentUser> AppointmentUsers { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Breed> Breeds { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Constant> Constants { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Movement> Movements { get; set; }

    public virtual DbSet<Packing> Packings { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    public virtual DbSet<Rh> Rhs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StateHealth> StateHealths { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=CadenaSQL");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Access>(entity =>
        {
            entity.HasKey(e => e.IdAccess);

            entity.ToTable("Access");

            entity.Property(e => e.IdAccess).HasColumnName("Id_Access");
            entity.Property(e => e.Email)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdRole).HasColumnName("Id_Role");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");
            entity.Property(e => e.Password)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("password");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Accesses)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Access_Role");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Accesses)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Access_User");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.IdAddress).HasName("PK_Addresss");

            entity.ToTable("Address");

            entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
            entity.Property(e => e.Address1)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Address_User");
        });

        modelBuilder.Entity<Adoption>(entity =>
        {
            entity.HasKey(e => e.IdAdoption).HasName("PK__Adoption__32EB504B80BB8931");

            entity.ToTable("Adoption");

            entity.Property(e => e.IdAdoption).HasColumnName("Id_Adoption");
            entity.Property(e => e.IdAdoptionDate).HasColumnName("Id_AdoptionDate");
            entity.Property(e => e.IdPet).HasColumnName("Id_Pet");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");

            entity.HasOne(d => d.IdAdoptionDateNavigation).WithMany(p => p.Adoptions)
                .HasForeignKey(d => d.IdAdoptionDate)
                .HasConstraintName("FK__Adoption__Id_Ado__1332DBDC");

            entity.HasOne(d => d.IdPetNavigation).WithMany(p => p.Adoptions)
                .HasForeignKey(d => d.IdPet)
                .HasConstraintName("FK_Adoption_Pet");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Adoptions)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Adoption__Id_Use__151B244E");
        });

        modelBuilder.Entity<AdoptionDate>(entity =>
        {
            entity.HasKey(e => e.IdAdoptionDate).HasName("PK__Adoption__3B42D79926BEC8D0");

            entity.ToTable("AdoptionDate");

            entity.Property(e => e.IdAdoptionDate).HasColumnName("Id_AdoptionDate");
            entity.Property(e => e.RegisterAdoption)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AnimalType>(entity =>
        {
            entity.HasKey(e => e.IdAnimalType);

            entity.ToTable("AnimalType");

            entity.Property(e => e.IdAnimalType).HasColumnName("Id_AnimalType");
            entity.Property(e => e.Type)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.IdAppointment);

            entity.ToTable("Appointment");

            entity.Property(e => e.IdAppointment).HasColumnName("Id_Appointment");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
        });

        modelBuilder.Entity<AppointmentUser>(entity =>
        {
            entity.HasKey(e => e.IdAppointmentUser);

            entity.ToTable("AppointmentUser");

            entity.Property(e => e.IdAppointmentUser).HasColumnName("Id_AppointmentUser");
            entity.Property(e => e.IdAppointment).HasColumnName("Id_Appointment");
            entity.Property(e => e.IdPet).HasColumnName("Id_Pet");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");

            entity.HasOne(d => d.IdAppointmentNavigation).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.IdAppointment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentUser_Appointment");

            entity.HasOne(d => d.IdPetNavigation).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.IdPet)
                .HasConstraintName("FK_AppointmentUser_Pet");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentUser_User");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.IdBrand);

            entity.ToTable("Brand");

            entity.Property(e => e.IdBrand).HasColumnName("Id_Brand");
            entity.Property(e => e.NameBrand)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("nameBrand");
        });

        modelBuilder.Entity<Breed>(entity =>
        {
            entity.HasKey(e => e.IdBreed);

            entity.ToTable("Breed");

            entity.Property(e => e.IdBreed).HasColumnName("Id_Breed");
            entity.Property(e => e.Breed1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Breed");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.IdComment);

            entity.ToTable("Comment");

            entity.Property(e => e.IdComment).HasColumnName("Id_Comment");
            entity.Property(e => e.Comment1)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.IdPublication).HasColumnName("Id_Publication");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");
            entity.Property(e => e.PublicationDate)
                .HasColumnType("datetime")
                .HasColumnName("publicationDate");

            entity.HasOne(d => d.IdPublicationNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdPublication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Publication");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Constant>(entity =>
        {
            entity.HasKey(e => e.IdConst);

            entity.ToTable("Constant");

            entity.Property(e => e.IdConst).HasColumnName("Id_Const");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.IdInventory);

            entity.ToTable("Inventory");

            entity.Property(e => e.IdInventory).HasColumnName("Id_Inventory");
            entity.Property(e => e.IdProduct).HasColumnName("Id_Product");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Product");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.IdMovement);

            entity.ToTable("Movement");

            entity.Property(e => e.IdMovement).HasColumnName("Id_Movement");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdInventary).HasColumnName("Id_Inventary");
            entity.Property(e => e.IdProduct).HasColumnName("Id_Product");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdInventaryNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.IdInventary)
                .HasConstraintName("FK_Movement_Inventory");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Movements)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movement_Product");
        });

        modelBuilder.Entity<Packing>(entity =>
        {
            entity.HasKey(e => e.IdPacking);

            entity.ToTable("Packing");

            entity.Property(e => e.IdPacking).HasColumnName("Id_Packing");
            entity.Property(e => e.TypePacking)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("typePacking");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.IdPet);

            entity.ToTable("Pet");

            entity.Property(e => e.IdPet).HasColumnName("Id_Pet");
            entity.Property(e => e.BirthYear).HasColumnName("birthYear");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.IdAnimalType).HasColumnName("Id_AnimalType");
            entity.Property(e => e.IdBreed).HasColumnName("Id_Breed");
            entity.Property(e => e.IdStateHealth).HasColumnName("Id_StateHealth");
            entity.Property(e => e.PerImage)
                .HasColumnType("image")
                .HasColumnName("perImage");
            entity.Property(e => e.PetName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("petName");

            entity.HasOne(d => d.IdAnimalTypeNavigation).WithMany(p => p.Pets)
                .HasForeignKey(d => d.IdAnimalType)
                .HasConstraintName("FK_Pet_AnimalType");

            entity.HasOne(d => d.IdBreedNavigation).WithMany(p => p.Pets)
                .HasForeignKey(d => d.IdBreed)
                .HasConstraintName("FK_Pet_Breed");

            entity.HasOne(d => d.IdStateHealthNavigation).WithMany(p => p.Pets)
                .HasForeignKey(d => d.IdStateHealth)
                .HasConstraintName("FK_Pet_StateHealth1");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.IdPhone);

            entity.ToTable("Phone");

            entity.Property(e => e.IdPhone).HasColumnName("Id_Phone");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");
            entity.Property(e => e.Phone1).HasColumnName("Phone");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Phones)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Phone_User");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct).HasColumnName("Id_Product");
            entity.Property(e => e.IdAnimalType).HasColumnName("Id_AnimalType");
            entity.Property(e => e.IdBrand).HasColumnName("Id_Brand");
            entity.Property(e => e.IdPacking).HasColumnName("Id_Packing");
            entity.Property(e => e.ProductName)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("productName");

            entity.HasOne(d => d.IdAnimalTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdAnimalType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_AnimalType");

            entity.HasOne(d => d.IdBrandNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdBrand)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.IdPackingNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdPacking)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Packing");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.HasKey(e => e.IdPublication);

            entity.ToTable("Publication");

            entity.Property(e => e.IdPublication).HasColumnName("Id_Publication");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IdUser).HasColumnName("Id_User");
            entity.Property(e => e.Image)
                .HasColumnType("image")
                .HasColumnName("image");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Publications)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Publication_User");
        });

        modelBuilder.Entity<Rh>(entity =>
        {
            entity.HasKey(e => e.IdRh);

            entity.ToTable("Rh");

            entity.Property(e => e.IdRh).HasColumnName("Id_Rh");
            entity.Property(e => e.RhType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.Property(e => e.IdRole).HasColumnName("Id_Role");
            entity.Property(e => e.RoleType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleType");
        });

        modelBuilder.Entity<StateHealth>(entity =>
        {
            entity.HasKey(e => e.IdStateHealth);

            entity.ToTable("StateHealth");

            entity.Property(e => e.IdStateHealth).HasColumnName("Id_StateHealth");
            entity.Property(e => e.Castrated).HasColumnName("castrated");
            entity.Property(e => e.State)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("state");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser).HasColumnName("Id_User");
            entity.Property(e => e.BirthDate)
                .HasColumnType("datetime")
                .HasColumnName("birthDate");
            entity.Property(e => e.IdRh).HasColumnName("Id_Rh");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.State).HasColumnName("state");

            entity.HasOne(d => d.IdRhNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRh)
                .HasConstraintName("FK_User_Rh");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.IdVaccine);

            entity.ToTable("Vaccine");

            entity.Property(e => e.IdVaccine).HasColumnName("Id_Vaccine");
            entity.Property(e => e.IdPet).HasColumnName("Id_Pet");
            entity.Property(e => e.TypeVaccine)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("typeVaccine");
            entity.Property(e => e.VaccinationDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdPetNavigation).WithMany(p => p.Vaccines)
                .HasForeignKey(d => d.IdPet)
                .HasConstraintName("FK_Vaccine_Pet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

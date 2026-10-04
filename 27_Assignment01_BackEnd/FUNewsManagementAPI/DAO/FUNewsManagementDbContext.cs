using FUNewsManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FUNewsManagementAPI.DAO
{
    public class FUNewsManagementDbContext : DbContext
    {
        public FUNewsManagementDbContext()
        {
        }

        public FUNewsManagementDbContext(DbContextOptions<FUNewsManagementDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<NewsArticle> NewsArticles { get; set; } = null!;
        public virtual DbSet<NewsTag> NewsTags { get; set; } = null!;
        public virtual DbSet<SystemAccount> SystemAccounts { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                var configuration = builder.Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection") 
                    ?? "Server=.;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK_Category");

                entity.Property(e => e.CategoryName).HasMaxLength(100);
                entity.Property(e => e.CategoryDesciption).HasMaxLength(250);

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("FK_Category_Category");
            });

            modelBuilder.Entity<NewsArticle>(entity =>
            {
                entity.HasKey(e => e.NewsArticleId).HasName("PK_NewsArticle");

                entity.Property(e => e.NewsArticleId).HasMaxLength(20);
                entity.Property(e => e.Headline).HasMaxLength(150);
                entity.Property(e => e.NewsContent).HasMaxLength(4000);
                entity.Property(e => e.NewsSource).HasMaxLength(400);
                entity.Property(e => e.NewsTitle).HasMaxLength(400);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.NewsArticles)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsArticle_Category");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.CreatedNewsArticles)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsArticle_SystemAccount");
            });

            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.HasKey(e => new { e.NewsArticleId, e.TagId }).HasName("PK_NewsTag");

                entity.Property(e => e.NewsArticleId).HasMaxLength(20);

                entity.HasOne(d => d.NewsArticle)
                    .WithMany(p => p.NewsTags)
                    .HasForeignKey(d => d.NewsArticleId)
                    .HasConstraintName("FK_NewsTag_NewsArticle");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.NewsTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_NewsTag_Tag");
            });

            modelBuilder.Entity<SystemAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId).HasName("PK_SystemAccount");

                entity.Property(e => e.AccountId).ValueGeneratedNever();
                entity.Property(e => e.AccountEmail).HasMaxLength(70);
                entity.Property(e => e.AccountName).HasMaxLength(100);
                entity.Property(e => e.AccountPassword).HasMaxLength(70);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.TagId).HasName("PK_HashTag");

                entity.Property(e => e.TagId).ValueGeneratedNever();
                entity.Property(e => e.Note).HasMaxLength(400);
                entity.Property(e => e.TagName).HasMaxLength(50);
            });
        }
    }
}

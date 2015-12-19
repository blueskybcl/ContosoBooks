using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using ContosoBooks.Models;

namespace ContosoBooks.Migrations
{
    [DbContext(typeof(BookContext))]
    [Migration("20151215001912_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContosoBooks.Models.Author", b =>
                {
                    b.Property<int>("AuthorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstMidName");

                    b.Property<string>("LastName");

                    b.HasKey("AuthorID");
                });

            modelBuilder.Entity("ContosoBooks.Models.Book", b =>
                {
                    b.Property<int>("BookID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthorID");

                    b.Property<string>("Genre");

                    b.Property<decimal>("Price");

                    b.Property<string>("Title");

                    b.Property<int>("Year");

                    b.HasKey("BookID");
                });

            modelBuilder.Entity("ContosoBooks.Models.Book", b =>
                {
                    b.HasOne("ContosoBooks.Models.Author")
                        .WithMany()
                        .HasForeignKey("AuthorID");
                });
        }
    }
}

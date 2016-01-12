﻿using System;
using System.Linq;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoBooks.Models
{
    public static class SampleData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<BookContext>();
            if (context.Database == null)
            {
                throw new Exception("DB is null");
            }

            if (context.Books.Any())
            {
                return;   // DB has been seeded
            }
            //if (serviceProvider.GetService<IRelationalDatabaseCreator>().Exists())
            //{
            //    if (!context.Books.Any())
            //    {
            var austen = context.Authors.Add(
        new Author { LastName = "Austen", FirstMidName = "Jane" }).Entity;
            var dickens = context.Authors.Add(
                new Author { LastName = "Dickens", FirstMidName = "Charles" }).Entity;
            var cervantes = context.Authors.Add(
                new Author { LastName = "Cervantes", FirstMidName = "Miguel" }).Entity;

            context.Books.AddRange(
                new Book()
                {
                    Title = "Pride and Prejudice",
                    Year = 1813,
                    Author = austen,
                    Price = 9.99M,
                    Genre = "Comedy of manners"
                },
                new Book()
                {
                    Title = "Northanger Abbey",
                    Year = 1817,
                    Author = austen,
                    Price = 12.95M,
                    Genre = "Gothic parody"
                },
                new Book()
                {
                    Title = "David Copperfield",
                    Year = 1850,
                    Author = dickens,
                    Price = 15,
                    Genre = "Bildungsroman"
                },
                new Book()
                {
                    Title = "Don Quixote",
                    Year = 1617,
                    Author = cervantes,
                    Price = 8.95M,
                    Genre = "Picaresque"
                }
            );
            context.SaveChanges();
        }
    }
}

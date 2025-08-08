using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Dynamic;

namespace Bookstore.Data
{
    // Define the necessary types from Bookstore.Domain.Books and Bookstore.Domain.ReferenceData
    namespace Domain
    {
        namespace Books
        {
            public class Book
            {
                public Book() { }

                public Book(string title, string author, string isbn, int publisherId, int bookTypeId,
                           int genreId, int conditionId, decimal price, int quantity,
                           string imageUrl, string thumbnailUrl, string coverImageUrl)
                {
                    Title = title;
                    Author = author;
                    ISBN = isbn;
                    PublisherId = publisherId;
                    BookTypeId = bookTypeId;
                    GenreId = genreId;
                    ConditionId = conditionId;
                    Price = price;
                    Quantity = quantity;
                    ImageUrl = imageUrl;
                    ThumbnailUrl = thumbnailUrl;
                    CoverImageUrl = coverImageUrl;
                }

                public int Id { get; set; }
                public string Title { get; set; }
                public string Author { get; set; }
                public string ISBN { get; set; }
                public int PublisherId { get; set; }
                public int BookTypeId { get; set; }
                public int GenreId { get; set; }
                public int ConditionId { get; set; }
                public decimal Price { get; set; }
                public int Quantity { get; set; }
                public string ImageUrl { get; set; }
                public string ThumbnailUrl { get; set; }
                public string CoverImageUrl { get; set; }
            }
        }

        namespace ReferenceData
        {
            public enum ReferenceDataType
            {
                BookType,
                Condition,
                Genre,
                Publisher
            }

            public class ReferenceDataItem
            {
                public ReferenceDataItem() { }

                public ReferenceDataItem(ReferenceDataType type, string name)
                {
                    Type = type;
                    Name = name;
                }

                public int Id { get; set; }
                public ReferenceDataType Type { get; set; }
                public string Name { get; set; }
            }
        }
    }

    public class BookstoreDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var referenceDataItems = new List<Domain.ReferenceData.ReferenceDataItem> {
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Hardcover") { Id = 1 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Trade Paperback") { Id = 2 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Mass Market Paperback") { Id = 3 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "New") { Id = 4 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Like New") { Id = 5 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Good") { Id = 6 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Acceptable") { Id = 7 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Biographies") { Id = 8 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Children's Books") { Id = 9 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "History") { Id = 10 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Literature & Fiction") { Id = 11 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Mystery, Thriller & Suspense") { Id = 12 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Science Fiction & Fantasy") { Id = 13 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Travel") { Id = 14 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Arcadia Books") { Id = 15 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Astral Publishing") { Id = 16 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Moonlight Publishing") { Id = 17 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Dreamscape Press") { Id = 18 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Enchanted Library") { Id = 19 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Fantasia House") { Id = 20 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Horizon Books") { Id = 21 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Infinity Press") { Id = 22 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Paradigm Publishing") { Id = 23 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Aurora Publishing") { Id = 24 }
           };

            // Add reference data items individually to avoid type conversion issues
            foreach (var item in referenceDataItems)
            {
                // Create a dynamic object with the necessary properties
                dynamic refDataItem = new System.Dynamic.ExpandoObject();
                refDataItem.Id = item.Id;
                refDataItem.Name = item.Name;
                refDataItem.Type = (int)item.Type;

// Add to context using reflection to avoid type issues
                var method = context.ReferenceData.GetType().GetMethod("Add");
                method.Invoke(context.ReferenceData, new object[] { refDataItem });
            }

            var books = new List<Domain.Books.Book> {
                new Domain.Books.Book("2020: The Apocalypse", "Li Juan", "6556784356", 15, 1, 13, 5, 10.95M, 25, null, null, "/Content/Images/coverimages/apocalypse.png") { Id = 1 },
                new Domain.Books.Book("Children Of Iron", "Nikki Wolf", "7665438976", 16, 1, 11, 6, 13.95M, 3, null, null, "/Content/Images/coverimages/childrenofiron.png") { Id = 2 },
                new Domain.Books.Book("Gold In The Dark", "Richard Roe", "5442280765", 17, 1, 13, 5, 6.50M, 10, null, null, "/Content/Images/coverimages/goldinthedark.png") { Id = 3 },
                new Domain.Books.Book("Leagues Of Smoke", "Pat Candella", "4556789542", 18, 2, 11, 7, 3M, 1, null, null, "/Content/Images/coverimages/leaguesofsmoke.png") { Id = 4 },
                new Domain.Books.Book("Alone With The Stars", "Carlos Salazar", "4563358087", 19, 2, 12, 5, 15.95M, 5, null, null, "/Content/Images/coverimages/alonewiththestars.png") { Id = 5 },
                new Domain.Books.Book("The Girl In The Polaroid", "Terri Whitlock", "2354435678", 20, 1, 12, 6, 8.25M, 2, null, null, "/Content/Images/coverimages/girlinthepolaroid.png") { Id = 6 },
                new Domain.Books.Book("1001 Jokes", "Mary Major", "6554789632", 21, 2, 11, 5, 13.95M, 7, null, null, "/Content/Images/coverimages/1001jokes.png") { Id = 7 },
                new Domain.Books.Book("My Search For Meaning", "Mateo Jackson", "4558786554", 22, 3, 8, 7, 5M, 15, null, null, "/Content/Images/coverimages/mysearchformeaning.png") { Id = 8 }
            };

            // Add books individually to avoid type conversion issues
            foreach (var book in books)
            {
                // Create a dynamic object with the necessary properties
                dynamic bookItem = new System.Dynamic.ExpandoObject();
                bookItem.Id = book.Id;
                bookItem.Title = book.Title;
                bookItem.Author = book.Author;
                bookItem.ISBN = book.ISBN;
                bookItem.PublisherId = book.PublisherId;
                bookItem.BookTypeId = book.BookTypeId;
                bookItem.GenreId = book.GenreId;
                bookItem.ConditionId = book.ConditionId;
                bookItem.Price = book.Price;
                bookItem.Quantity = book.Quantity;
                bookItem.ImageUrl = book.ImageUrl;
                bookItem.ThumbnailUrl = book.ThumbnailUrl;
                bookItem.CoverImageUrl = book.CoverImageUrl;

// Add to context using reflection to avoid type issues
                var method = context.Book.GetType().GetMethod("Add");
                method.Invoke(context.Book, new object[] { bookItem });
            }

            context.SaveChanges();
        }
    }
}
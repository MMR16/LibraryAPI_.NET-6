using LibraryAPI.DbContexts;
using LibraryAPI.Entites;
using LibraryAPI.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Servicces
{
    public class AuthorRepository : IAuthorRepository, IDisposable
    {
        private readonly CourseLibraryContexts _context;
        public AuthorRepository(CourseLibraryContexts context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public void AddAuthor(Author author)
        {
            if (author is null)
            {
                throw new ArgumentException(nameof(author));
            }
            author.Id = Guid.NewGuid();
            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }
            _context.Authors.Add(author);
        }
        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentException(nameof(authorId));
            }
            return _context.Authors.Any(e => e.Id == authorId);
        }
        public void DeleteAuthor(Author author)
        {
            if (author is null)
            {
                throw new ArgumentException(nameof(author));
            }
            _context.Authors.Remove(author);
        }
        public Author GetAuthor(Guid authorId)
        {
            // ArgumentNullException.ThrowIfNull(nameof(authorId));
            if (authorId == Guid.Empty)
            {
                throw new ArgumentException(nameof(authorId));
            }
            return _context.Authors.FirstOrDefault(e => e.Id == authorId);
        }
        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList();
            ////return _context.Authors.Include(w=>w.Courses).ToList();
        }
        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds is null)
            {
                throw new ArgumentException(nameof(authorIds));
            }
            return _context.Authors.Where(q => authorIds.Contains(q.Id))
                .OrderBy(e => e.FirstName)
                .OrderBy(e => e.LastName)
                .ToList();
        }
        public void UpdateAuthor(Author author)
        {
            if (author is null)
            {
                throw new ArgumentException(nameof(author));
            }
            _context.Authors.Update(author);
        }
        public bool save()
        {
            return (_context.SaveChanges() >= 0);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }


        //public IEnumerable<Author> GetAuthors(string mainCategory, string searchQuery)
        //{
        //    if (string.IsNullOrWhiteSpace(mainCategory) && string.IsNullOrWhiteSpace(searchQuery))
        //    {
        //        return GetAuthors();
        //    }
        //    // for defered excution we use IQueryable
        //    var collection = _context.Authors as IQueryable<Author>;

        //    if (!string.IsNullOrWhiteSpace(mainCategory))
        //    {
        //        mainCategory = mainCategory.Trim().ToLower();
        //        collection = collection.Where(e => e.MainCategory.Trim().ToLower() == mainCategory);
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchQuery))
        //    {
        //        searchQuery = searchQuery.Trim().ToLower();
        //        collection = collection.Where(
        //            e => e.MainCategory.Contains(searchQuery) ||
        //            e.FirstName.Contains(searchQuery) ||
        //            e.LastName.Contains(searchQuery));
        //    }
        //    return collection.ToList();
        //}

        public IEnumerable<Author> GetAuthors(AuthorResourceParameter authorResourceParameter)
        {

            if (string.IsNullOrWhiteSpace(authorResourceParameter.MainCategory))
            {
                return GetAuthors();
            }
            var collection = _context.Authors as IQueryable<Author>;

            if (!string.IsNullOrWhiteSpace(authorResourceParameter.MainCategory))
            {
                var mainCategory = authorResourceParameter.MainCategory.Trim().ToLower();
                collection = collection.Where(e => e.MainCategory.Trim().ToLower() == mainCategory);
            }
            if (!string.IsNullOrWhiteSpace(authorResourceParameter.searchQuery))
            {
               var searchquery = authorResourceParameter.searchQuery.Trim().ToLower();
                collection = collection.Where(
                    q => q.FirstName.Contains(searchquery) ||
                    q.LastName.Contains(searchquery) ||
                    q.MainCategory.Contains(searchquery));
            }
            return collection.ToList();
        }
    }
}

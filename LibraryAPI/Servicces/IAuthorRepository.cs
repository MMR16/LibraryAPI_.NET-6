using LibraryAPI.Entites;
using LibraryAPI.ResourceParameters;

namespace LibraryAPI.Servicces
{
    public interface IAuthorRepository
    {
        IEnumerable<Author> GetAuthors();
        //IEnumerable<Author> GetAuthors(string mainCategory,string searchQuery);
        IEnumerable<Author> GetAuthors(AuthorResourceParameter authorResourceParameter);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void UpdateAuthor(Author author);
        void DeleteAuthor(Author author);
        bool AuthorExists(Guid authorId);
        bool save();

    }
}

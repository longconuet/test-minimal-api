namespace TestMinimalAPI.Services
{
    public interface IBookService
    {
        List<Book> GetBooks();

        Book? GetBook(int id);
    }
}

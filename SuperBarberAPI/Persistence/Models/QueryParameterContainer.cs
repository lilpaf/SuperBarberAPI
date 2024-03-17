namespace Persistence.Models
{
    public class QueryParameterContainer
    {
        private const int BarberShopsPerPage = 24;

        private const int DefaultPageNumber = 0;

        private int _pageNumber = 0;

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            init
            {
                _pageNumber = ((value >= 0) ? value : DefaultPageNumber);
            }
        }

        public int SkipCount => PageNumber * BarberShopsPerPage;

        public string? City { get; init; }

        public string? Neighborhood { get; init; }

        public string? SearchName { get; init; }
    }
}

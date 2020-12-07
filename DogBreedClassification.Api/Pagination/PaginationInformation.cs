namespace DogBreedClassification.Api.Pagination
{
    public class PaginationInformation
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }

        public PaginationInformation(int count, int currentPage, int pageSize, int totalPages)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }
    }
}

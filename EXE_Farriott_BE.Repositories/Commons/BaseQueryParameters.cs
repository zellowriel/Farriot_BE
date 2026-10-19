namespace EXE_Farriott_BE.Repositories.Commons
{
    public class BaseQueryParameters
    {
        // 1. Không nhập Page -> Mặc định là trang số 1
        public int Page { get; set; } = 1;

        // 2. Không nhập Size -> Mang giá trị null (Ý nghĩa: Không phân trang, lấy tất cả)
        public int? Size { get; set; }

        // 3. Không nhập Keyword -> null (Lấy tất cả)
        public string? Keyword { get; set; }

        // 4. Không nhập SearchBy -> null (Kệ keyword là gì, bỏ qua tìm kiếm)
        public string? SearchBy { get; set; }

        public string? SortBy { get; set; }
        public string? Fields { get; set; }
        public string? Expand { get; set; }
    }
}
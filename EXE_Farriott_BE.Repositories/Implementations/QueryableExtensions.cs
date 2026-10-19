using System.Linq.Expressions;
using System.Reflection;
using EXE_Farriott_BE.Repositories.Commons;

namespace EXE_Farriott_BE.Repositories.Commons
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyDynamicSearch<T>(this IQueryable<T> query, string? searchBy, string? keyword)
        {
            // 1. Kiểm tra đầu vào, nếu rỗng thì không làm gì cả
            if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(keyword))
                return query;

            // 2. Dùng Reflection tìm cột tương ứng trong Entity (Bỏ qua viết hoa/thường)
            var property = typeof(T).GetProperty(searchBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                return query; // Tên cột không tồn tại -> Bỏ qua tìm kiếm

            // Lấy kiểu dữ liệu thực sự của cột (xử lý luôn cả kiểu Nullable như int?)
            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            // 3. Chuẩn bị biến đại diện 'x' (trong câu lệnh x => x.Property)
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            Expression filterExpression;

            keyword = keyword.Trim();

            try
            {
                // 4. TỰ ĐỘNG KIỂM TRA KIỂU DỮ LIỆU & ÉP KIỂU
                if (targetType == typeof(string))
                {
                    // Tạo logic: x.Property.ToLower().Contains(keyword.ToLower())
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

                    var keywordExpr = Expression.Constant(keyword.ToLower());
                    var propToLower = Expression.Call(propertyAccess, toLowerMethod);
                    filterExpression = Expression.Call(propToLower, containsMethod, keywordExpr);
                }
                else if (targetType == typeof(int))
                {
                    // Nếu là số, thử ép kiểu. Nếu user nhập bậy (vd: "abc") -> Bỏ qua search
                    if (!int.TryParse(keyword, out int parsedValue)) return query;

                    // Tạo logic: x.Property == parsedValue
                    var valueExpr = Expression.Constant(parsedValue, property.PropertyType);
                    filterExpression = Expression.Equal(propertyAccess, valueExpr);
                }
                else if (targetType == typeof(decimal) || targetType == typeof(double))
                {
                    if (!decimal.TryParse(keyword, out decimal parsedValue)) return query;

                    var safeValue = Convert.ChangeType(parsedValue, targetType);
                    var valueExpr = Expression.Constant(safeValue, property.PropertyType);
                    filterExpression = Expression.Equal(propertyAccess, valueExpr);
                }
                else if (targetType == typeof(bool))
                {
                    if (!bool.TryParse(keyword, out bool parsedValue)) return query;

                    var valueExpr = Expression.Constant(parsedValue, property.PropertyType);
                    filterExpression = Expression.Equal(propertyAccess, valueExpr);
                }
                else
                {
                    return query; // Chưa hỗ trợ tìm kiếm cho DateTime (có thể mở rộng sau)
                }

                // 5. Ráp thành câu lệnh LINQ Where hoàn chỉnh và nhúng vào Query
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                return query.Where(lambda);
            }
            catch
            {
                return query; // Chặn crash hệ thống nếu có lỗi ép kiểu dị biệt
            }
        }
    }
}
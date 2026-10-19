using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductResponse>> GetProductsAsync(BaseQueryParameters query)
        {
            var (items, total) = await _productRepository.GetAllAsync(query);

            var responseItems = items.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                BasePrice = p.BasePrice,
                IsCustomizable = p.IsCustomizable ?? true,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new PagedResult<ProductResponse>
            {
                Items = responseItems,
                TotalItems = total,
                Page = query.Page,
                // Nếu query.Size bị null -> Lấy total. 
                // Ép tối thiểu bằng 1 để tránh lỗi toán học (Divide by zero) khi tính TotalPages
                Size = query.Size ?? (total > 0 ? total : 1)
            };
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                IsCustomizable = product.IsCustomizable ?? true,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                BasePrice = request.BasePrice,
                IsCustomizable = request.IsCustomizable,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            return new ProductResponse
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                BasePrice = createdProduct.BasePrice,
                IsCustomizable = createdProduct.IsCustomizable ?? true,
                CreatedAt = createdProduct.CreatedAt
            };
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.Name = request.Name;
            product.Description = request.Description;
            product.BasePrice = request.BasePrice;
            product.IsCustomizable = request.IsCustomizable;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepository.DeleteAsync(product);
            return true;
        }
    }
}
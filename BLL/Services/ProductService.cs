using BLL.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductService
    {
        private ICrudRepo<Product, int> _productRepo;

        public ProductService(ICrudRepo<Product, int> _productRepo)
        {
            this._productRepo = _productRepo;
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _productRepo.GetByIdWithInclude(id, "ProductId", p => p.Carts, p => p.Category, p => p.OrderDetails) ?? new Product();
            return product;
        }

        public async Task<List<Product>> GetAllProduct()
        {
            var products = await _productRepo.GetAllWithInclude(p => p.Carts, p => p.Category, p => p.OrderDetails);
            return products.ToList();
        }

        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            if (categoryId == 0)
            {
                // get all
                var allProducts = await _productRepo.GetAllWithInclude(p => p.Carts, p => p.Category, p => p.OrderDetails);
                return allProducts.ToList();
            }
            var products = (await _productRepo.GetAllWithInclude(p => p.Carts, p => p.Category, p => p.OrderDetails));
            return products.Where(p => p.CategoryId == categoryId).ToList(); ;
        }

        public async Task<List<Product>> GetProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Product>(); // Trả về danh sách rỗng nếu không có từ khóa
            }

            var products = await _productRepo.GetAllWithInclude(p => p.Carts, p => p.Category, p => p.OrderDetails);
            return products
                .Where(p => p.ProductName != null && p.ProductName.ToLower().Contains(name.ToLower()))
                .ToList();
        }
    }
}

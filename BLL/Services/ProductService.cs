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

        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            var products = (await _productRepo.GetAllWithInclude(p => p.Carts, p => p.Category, p => p.OrderDetails)).ToList();
            products = products.Where(p => p.CategoryId == categoryId).ToList();
            return products;
        }
    }
}

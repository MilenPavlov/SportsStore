using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _repo;
        public int PageSize = 4;
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        // GET: Products
        public ViewResult List(string category, int page = 1)
        {
            var model = new ProductsListViewModel
            {
                Products = _repo.Products
                             .Where(p=> category == null || p.Category == category) 
                             .OrderBy(p => p.ProductID)
                             .Skip((page - 1) * PageSize)
                             .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                                _repo.Products.Count() :
                                _repo.Products.Count(e => e.Category == category)
                },
                CurrentCategory = category

            };

            return View(model);

        }
    }
}
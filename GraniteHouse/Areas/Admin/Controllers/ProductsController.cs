using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }


        
        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var Products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await Products.ToListAsync());
        }
        
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {                
                return View(ProductsVM);
            }

            _db.Add(ProductsVM);
            await _db.SaveChangesAsync();

            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productsFromDb = _db.Products.Find(ProductsVM.Products.Id);

            if (files.Count != 0)
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                var fileExtension = Path.GetExtension(files[0].FileName);

                
                using (var filestream = new FileStream(Path.Combine(uploadPath, ProductsVM.Products.Id + fileExtension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + fileExtension;  
            }
            else
            {
                //When Image is not uploaded
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder + @"\" +SD.DefaultProductImage);
                System.IO.File.Copy(uploadPath, webRootPath + SD.ImageFolder + @"\" + SD.DefaultProductImage);
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
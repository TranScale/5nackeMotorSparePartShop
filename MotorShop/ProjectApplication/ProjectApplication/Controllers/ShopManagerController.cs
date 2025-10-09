using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.Ajax.Utilities;
using ProjectApplication.Models;

namespace ProjectApplication.Controllers
{
    public class ShopManagerController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: ShopManager
        //public ActionResult Index(string productType)
        //{
        //    var products = db.Products.ToList();

        //    var productList = products.Select(p =>
        //    {
        //        if (p is Vehicle vehicle)
        //        {
        //            return new ProductViewModel
        //            {
        //                Id = vehicle.ProductId,
        //                Name = vehicle.vehicleName,
        //                Price = vehicle.price,
        //                number = vehicle.number,
        //                Engine = vehicle.Displacement,
        //                ProductType = vehicle.TypeProduct
        //            };

        //        }
        //        else if (p is SparePart part)
        //        {
        //            return new ProductViewModel
        //            {
        //                Id = part.ProductId,
        //                Name = part.spareName,
        //                Price = part.price,
        //                number = part.number,
        //                CompatibleModel = part.SuitableVehicles,
        //                ProductType = part.TypeProduct
        //            };
        //        }
        //        else
        //        {
        //            return new ProductViewModel
        //            {
        //                Id = 0,
        //                Name = "",
        //                Price = 0,
        //                ProductType = "Product"
        //            };
        //        }
        //    }).ToList();


        //    if (!string.IsNullOrEmpty(productType))
        //    {
        //        if (productType == "Vehicle")
        //        {
        //            productList = productList.Where(vm => vm.ProductType == "1").ToList();
        //        }
        //        else if (productType == "SparePart")
        //        {
        //            productList = productList.Where(vm => vm.ProductType == "2").ToList();
        //        }
        //    }


        //    return View(productList); // ✅ Trả về List<ProductViewModel>
        //    return View();
        //}
        public ActionResult Index(bool showProducts = false)
        {
            ViewBag.ShowProducts = showProducts;
            return View();
        }



        // GET: ShopManager/Details/5
        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);


            ProductViewModel vm;

            if (product is Vehicle vehicle)
            {
                vm = new ProductViewModel
                {
                    Id = vehicle.ProductId,
                    Name = vehicle.vehicleName,
                    Price = vehicle.price,
                    number = vehicle.number,
                    Engine = vehicle.Displacement,
                    fuelCapacity = vehicle.fuelCapacity,
                    weight = vehicle.weight,
                    Color = vehicle.Color,
                    BrandId = vehicle.BrandId,
                    description = vehicle.description,
                    ProductType = "Vehicle"
                };
            }
            else if (product is SparePart part)
            {
                vm = new ProductViewModel
                {
                    Id = part.ProductId,
                    Name = part.spareName,
                    Price = part.price,
                    number = part.number,
                    BrandId = part.BrandId,
                    description = part.spareDescription,
                    CompatibleModel = part.SuitableVehicles,
                    ProductType = "SparePart"
                };
            }
            else
            {
                return HttpNotFound(); // không tìm thấy sản phẩm
            }

            if (vm.BrandId == 1)
                vm.BrandName = "Yamaha";
            else if (vm.BrandId == 2)
                vm.BrandName = "Honda";

            return View(vm); // ✅ Truyền đúng ProductViewModel
        }


        // GET: ShopManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShopManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        public ActionResult LoadFormCreate(String productType)
        {
            if(productType == "Vehicle")
            {
                return PartialView("~/Views/ShopManager/VehicleForm.cshtml", new Vehicle());
            }
            if(productType == "SparePart")
            {
                return PartialView("~/Views/ShopManager/SparePartForm.cshtml", new SparePart());
            }
            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                vehicle.TypeProduct = "Vehicle";
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSparePart(SparePart sparePart)
        {
            if (ModelState.IsValid)
            {
                sparePart.TypeProduct = "SparePart";
                db.SpareParts.Add(sparePart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", sparePart);
        }

        // GET: ShopManager/Edit/5
        public ActionResult Edit(int? id)
        {
            var product = db.Products.Find(id);
            if (product == null) HttpNotFound();

            var viewModel = new ProductViewModel
            {
                Id = product.ProductId,
                Price = product.price,
                BrandId = product.BrandId,
                number = product.number,
            };

            var vehicle = product as Vehicle;
            if(vehicle != null)
            {
                viewModel.ProductType = "Vehicle";
                viewModel.Name = vehicle.vehicleName;
                viewModel.Engine = vehicle.Displacement;
                viewModel.fuelCapacity = vehicle.fuelCapacity;
                viewModel.weight = vehicle.weight;
                viewModel.BrandId = vehicle.BrandId;
                viewModel.Color = vehicle.Color;
                viewModel.description = vehicle.description;
            }
            var spare = product as SparePart;
            if (spare != null)
            {
                viewModel.ProductType = "SparePart";
                viewModel.Name = spare.TypeProduct;
                viewModel.BrandId = spare.BrandId;
                viewModel.spareDescription = spare.spareDescription;
                viewModel.CompatibleModel = spare.SuitableVehicles; // nếu có
            }

            return View(viewModel);
        }

        // POST: ShopManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // trả lại form nếu lỗi
            }

            var product = db.Products.Find(model.Id);
            if (product == null) return HttpNotFound();

            // cập nhật field chung
            product.BrandId = model.BrandId;
            product.price = model.Price;
            product.number = model.number;

            if (product is Vehicle vehicle && model.ProductType == "Vehicle")
            {
                vehicle.vehicleName = model.Name;
                vehicle.Displacement = model.Engine;
                vehicle.fuelCapacity = model.fuelCapacity;
                vehicle.weight = model.weight;
                vehicle.Color = model.Color;
                vehicle.description = model.description;
            }
            else if (product is SparePart spare && model.ProductType == "SparePart")
            {
                spare.TypeProduct = model.Name;
                spare.spareDescription = model.spareDescription;
                spare.SuitableVehicles = model.CompatibleModel;
            }

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("productmanage"); // hoặc Index tuỳ bạn
        }


        // GET: ShopManager/Delete/5
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Tạo ViewModel thủ công
            var viewModel = new ProductViewModel
            {
                Id = product.ProductId,
                Price = product.price,
                TypeProduct = product.TypeProduct,
                number = product.number
            };

            // Nếu là Vehicle
            var vehicle = product as Vehicle;
            if (vehicle != null)
            {
                viewModel.ProductType = "Vehicle";
                viewModel.Name = vehicle.vehicleName;
                viewModel.Engine = vehicle.Displacement;
                viewModel.fuelCapacity = vehicle.fuelCapacity;
                viewModel.weight = vehicle.weight;
                viewModel.BrandId = vehicle.BrandId;
                viewModel.Color = vehicle.Color;
                viewModel.description = vehicle.description;
            }

            // Nếu là SparePart
            var spare = product as SparePart;
            if (spare != null)
            {
                viewModel.ProductType = "SparePart";
                viewModel.Name = spare.TypeProduct;
                viewModel.BrandId = spare.BrandId;
                viewModel.spareDescription = spare.spareDescription;
                viewModel.CompatibleModel = spare.SuitableVehicles; // nếu có
            }

            return View(viewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            if (product is Vehicle vehicle)
            {
                vehicle.CompatibleSpareParts.Clear();
                db.Vehicles.Remove(vehicle);
            }
            else if (product is SparePart spare)
            {
                spare.SuitableVehicles.Clear();
                db.SpareParts.Remove(spare);
            }

            db.SaveChanges();
            return RedirectToAction("productmanage");
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ProductManage(string productType)
        {
            var products = db.Products.ToList();

            // Chuyển đổi sang ViewModel
            var productListAll = products.Select(p =>
            {
                if (p is Vehicle vehicle)
                {
                    return new ProductViewModel
                    {
                        Id = vehicle.ProductId,
                        Name = vehicle.vehicleName,
                        Price = vehicle.price,
                        number = vehicle.number,
                        Engine = vehicle.Displacement,
                        ProductType = "Vehicle"
                    };
                }
                else if (p is SparePart part)
                {
                    return new ProductViewModel
                    {
                        Id = part.ProductId,
                        Name = part.spareName,
                        Price = part.price,
                        CompatibleModel = part.SuitableVehicles,
                        ProductType = "SparePart"
                    };
                }
                else
                {
                    return new ProductViewModel
                    {
                        Id = 0,
                        Name = "",
                        Price = 0,
                        ProductType = "Product"
                    };
                }
            }).ToList();

            var productList = productListAll.ToList();
            if (!string.IsNullOrEmpty(productType))
            {
                if (productType == "Vehicle")
                {
                    productList = productListAll.Where(vm => vm.ProductType == "Vehicle").ToList();
                }
                else if (productType == "SparePart")
                {
                    productList = productListAll.Where(vm => vm.ProductType == "SparePart").ToList();
                }
                else if(productType == "All")
                {
                    productList = productListAll;
                }

            }

            // Trả về productList (danh sách đã lọc) thay vì productListAll
            return View("productmanage", productList);
        }


    }
}

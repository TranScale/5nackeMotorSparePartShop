using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectApplication.Service
{
    public class ProductViewService
    {
        //------------------ALL-------------------------------------------------
        public static ProductViewDetail ReturnVehicle(Vehicle vehicle, ProductViewDetail viewModel)
        {
            viewModel.Engine = vehicle.Engine;
            viewModel.FuelCapacity = vehicle.FuelCapacity;
            viewModel.Color = vehicle.Color;
            viewModel.VehicleType = vehicle.VehicleType;

            return viewModel;
        }

        public static ProductViewDetail ReturnSparePart(SparePart part, ProductViewDetail viewModel) 
        {
            return viewModel;
        }

        //------------------INDEX-------------------------------------------------
        //Xuất danh sách các sản phẩm (Index)
        public static List<ProductViewIndex> GetListIndex(List<Product> product)
        {
            var list = new List<ProductViewIndex>();
            foreach (Product item in product)
            {
                list.Add(new ProductViewIndex
                {
                    ImagePath = item.ImagePath,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.Price,
                    ProductQuantity = item.Quantity
                });
            }
            return list;
        }

        //Xuất 1 sản phẩm (Index)
        public static ProductViewIndex GetIndexView(Product product)
        {
            return new ProductViewIndex
            {
                ImagePath = product.ImagePath,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.Price,
                ProductQuantity = product.Quantity
            };
        }
        //------------------DETAIL-------------------------------------------------
        //Xuất 1 sản phẩm (Detail)
        public static ProductViewDetail GetDetail(Product product)
        {
            var viewModel = new ProductViewDetail
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.Price,
                ProductQuantity = product.Quantity,
                ProductDescription = product.ProductDescription,
                ProductType = product.ProductType,
                BrandId = product.BrandId,
                BrandName = product.brand != null ? product.brand.BrandName : "Unknown"

            };
            if (product is Vehicle vehicle)
            {
                ReturnVehicle(vehicle, viewModel);
                return viewModel;
            }
            else if (product is SparePart sparepart)
            {
                ReturnSparePart(sparepart, viewModel);
                return viewModel;
            }
            else
                throw new Exception("Product not found");
        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine;
using Southwind.Entities;
using Signum.Utilities;
using Signum.Entities;
using Signum.Services;
using System.Globalization;

namespace Southwind.Load
{
    internal static class ProductLoader
    {
#pragma warning disable 0649
        public class SupplierFaxCSV
        {
            public int SupplierID;
            public string Fax; 
        }
#pragma warning restore 0649
        public static void LoadSuppliers()
        {
            using (NorthwindDataContext db = new NorthwindDataContext())
            {
                List<SupplierFaxCSV> faxes = Csv.ReadFile<SupplierFaxCSV>("SupplierFaxes.csv", culture: CultureInfo.GetCultureInfo("es"));

                var faxDic = faxes.ToDictionary(r => r.SupplierID, r => r.Fax); 

                Administrator.SaveListDisableIdentity(db.Suppliers.Select(s =>
                    new SupplierDN
                    {
                        CompanyName = s.CompanyName,
                        ContactName = s.ContactName,
                        ContactTitle = s.ContactTitle,
                        Phone = s.Phone.Replace(".", " "),
                        Fax = faxDic[s.SupplierID].Replace(".", " "),
                        Address = new AddressDN
                        {
                            Address = s.Address,
                            City = s.City,
                            Region = s.Region,
                            PostalCode = s.PostalCode,
                            Country = s.Country
                        },
                    }.SetId(s.SupplierID)));
            }
        }

        public static void LoadCategories()
        {
            using (NorthwindDataContext db = new NorthwindDataContext())
            {
                Administrator.SaveListDisableIdentity(db.Categories.Select(s =>
                    new CategoryDN
                    {
                        CategoryName = s.CategoryName,
                        Description = s.Description,
                        Picture = s.Picture.ToArray(),
                    }.SetId(s.CategoryID)));
            }
        }

        public static void LoadProducts()
        {
            using (NorthwindDataContext db = new NorthwindDataContext())
            {
                Administrator.SaveListDisableIdentity(db.Products.Select(s =>
                    new ProductDN
                    {
                        ProductName = s.ProductName,
                        Supplier =  Lite.Create<SupplierDN>(s.SupplierID.Value),
                        Category = Lite.Create<CategoryDN>(s.CategoryID.Value),
                        QuantityPerUnit = s.QuantityPerUnit,
                        UnitPrice = s.UnitPrice.Value,
                        UnitsInStock = s.UnitsInStock.Value,
                        ReorderLevel = s.ReorderLevel.Value,
                        Discontinued = s.Discontinued,
                    }.SetId(s.ProductID)));
            }
        }
    }
}

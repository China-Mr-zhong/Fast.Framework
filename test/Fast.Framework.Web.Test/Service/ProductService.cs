using Fast.Framework.Extensions;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.Test.Models;
using Fast.Framework.Utils;
using Fast.Framework.Web.Test.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Service
{


    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductService
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContext db;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="db">数据库上下文</param>
        public ProductService(IDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="product">产品</param>
        /// <returns></returns>
        public int AddProduct(Product product)
        {
            return db.Insert(product).Exceute();
        }

        /// <summary>
        /// 添加产品列表
        /// </summary>
        /// <param name="products">产品</param>
        /// <returns></returns>
        public int AddCategoryList(List<Product> products)
        {
            return db.Insert(products).Exceute();
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="product">产品</param>
        /// <returns></returns>
        public int DeleteProduct(Product product)
        {
            return db.Delete(product).Exceute();
        }

        /// <summary>
        /// 更新产品
        /// </summary>
        /// <param name="product">产品</param>
        /// <returns></returns>
        public int UpdateProduct(Product product)
        {
            return db.Update(product).Exceute();
        }

        /// <summary>
        /// 更新产品列表
        /// </summary>
        /// <param name="products">产品</param>
        /// <returns></returns>
        public int UpdateProductList(List<Product> products)
        {
            return db.Update(products).Exceute();
        }

        /// <summary>
        /// 导入产品列表
        /// </summary>
        /// <param name="products">产品</param>
        /// <returns></returns>
        public void ImportProductList(List<Product> products)
        {
            db.Fast<Product>().BulkCopy(products);
        }

        /// <summary>
        /// 导入产品列表
        /// </summary>
        /// <param name="dataTable">数据表格</param>
        /// <returns></returns>
        public void ImportProductList(DataTable dataTable)
        {
            db.Fast<Product>().BulkCopy(dataTable);
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Product GetProduct(ReqProduct req)
        {
            return db.Query<Product>().Where(w => w.ProductId == req.ProductId).First();
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Product> GetProductList(ReqProductList req)
        {
            var ex = DynamicWhereExp.Create<Product>().AndIF(req.ProductIds.Count > 0, (a) => SqlFunc.In(a.ProductId, req.ProductIds));//动态条件表达式
            return db.Query<Product>().Where(ex.Build()).ToList();
        }

        /// <summary>
        /// 获取产品分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<Product> GetProductPageList(ReqProductList req)
        {
            var ex = DynamicWhereExp.Create<Product>().AndIF(req.ProductIds.Count > 0, (a) => SqlFunc.In(a.ProductId, req.ProductIds));//动态条件表达式

            var count = 0;

            var data = db.Query<Product>().Where(ex.Build()).ToPageList(req.Pagination.Page, req.Pagination.PageSize, ref count);
            req.Pagination.Count = count;//回写总数
            return data;
        }
    }
}

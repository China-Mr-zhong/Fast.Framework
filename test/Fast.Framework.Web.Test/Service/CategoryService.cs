using Fast.Framework.Extensions;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.Test.Models;
using Fast.Framework.Utils;
using Fast.Framework.Web.Test.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Service
{


    /// <summary>
    /// 类别服务类
    /// </summary>
    public class CategoryService
    {

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContext db;

        /// <summary>
        /// 构造放方法
        /// </summary>
        /// <param name="db">数据库上下文</param>
        public CategoryService(IDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// 添加类别
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public int AddCategory(Category category)
        {
            return db.Insert(category).Exceute();
        }

        /// <summary>
        /// 添加类别列表
        /// </summary>
        /// <param name="categorys">类别</param>
        /// <returns></returns>
        public int AddCategoryList(List<Category> categorys)
        {
            return db.Insert(categorys).Exceute();
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public int DeleteCategory(Category category)
        {
            return db.Delete(category).Exceute();
        }

        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public int UpdateCategory(Category category)
        {
            return db.Update(category).Exceute();
        }

        /// <summary>
        /// 更新类别列表
        /// </summary>
        /// <param name="categorys">类别</param>
        /// <returns></returns>
        public int UpdateCategoryList(List<Category> categorys)
        {
            return db.Update(categorys).Exceute();
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Category GetCategory(ReqCategory req)
        {
            return db.Query<Category>().Where(w => w.CategoryId == req.CategoryId).First();
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<Category> GetCategoryList(ReqCategoryList req)
        {
            var ex = DynamicWhereExp.Create<Category>().AndIF(req.CategoryIds.Count > 0, (a) => SqlFunc.In(a.CategoryId, req.CategoryIds));//动态条件表达式
            return db.Query<Category>().Where(ex.Build()).ToList();
        }

        /// <summary>
        /// 获取分类分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<Category> GetCategoryPageList(ReqCategoryList req)
        {
            var ex = DynamicWhereExp.Create<Category>().AndIF(req.CategoryIds.Count > 0, (a) => SqlFunc.In(a.CategoryId, req.CategoryIds));//动态条件表达式
            var count = 0;
            var data = db.Query<Category>().Where(ex.Build()).ToPageList(req.Pagination.Page, req.Pagination.PageSize, ref count);
            req.Pagination.Count = count;//回写总数
            return data;
        }
    }
}

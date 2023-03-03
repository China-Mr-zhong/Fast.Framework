using Fast.Framework.Interfaces;
using Fast.Framework.Test.Models;

namespace Fast.Framework.Web.Test.Service
{

    /// <summary>
    /// 工作单元测试服务
    /// </summary>
    public class UnitOfWorkTestService
    {

        /// <summary>
        /// 工作单元
        /// </summary>
        private readonly IUnitOfWork unitOfWork;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="unitOfWork">工作单元</param>
        public UnitOfWorkTestService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public string Test()
        {
            //unitOfWork 对象无需显示使用using
            var result1 = unitOfWork.Db.Insert(new Category()
            {
                CategoryName = "类别1"
            }).ExceuteReturnIdentity();

            var result2 = unitOfWork.Db.Insert(new Product()
            {
                CategoryId = result1,
                ProductCode = "1001",
                ProductName = "测试产品1"
            }).Exceute();

            unitOfWork.Commit();

            return "工作单元执行完成...";
        }
    }
}

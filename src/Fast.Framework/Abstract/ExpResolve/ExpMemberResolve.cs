using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 成员解析
    /// </summary>
    public abstract class ExpMemberResolve
    {
        /// <summary>
        /// 解析名称
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public abstract string ResolveName(string memberName);
    }
}

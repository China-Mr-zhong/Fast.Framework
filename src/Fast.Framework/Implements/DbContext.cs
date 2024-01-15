using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Options;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库上下文实现类
    /// </summary>
    public class DbContext : IDbContext
    {
        /// <summary>
        /// 上下文ID
        /// </summary>
        public Guid ContextId { get; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        public List<DbOptions> DbOptions { get; }

        /// <summary>
        /// Ado字段
        /// </summary>
        private IAdo ado;

        /// <summary>
        /// ado锁
        /// </summary>
        private readonly object ado_lock = new object();

        /// <summary>
        /// Ado属性
        /// </summary>
        public IAdo Ado
        {
            get
            {
                if (ado == null)
                {
                    lock (ado_lock)
                    {
                        if (ado == null)
                        {
                            var option = DbOptions.FirstOrDefault(f => f.IsDefault, DbOptions[0]);
                            ado = GetAdo(option.DbId);
                        }
                    }
                }
                return ado;
            }
            private set
            {
                ado = value;
            }
        }

        /// <summary>
        /// Aop
        /// </summary>
        public AopProvider Aop { get; }

        /// <summary>
        /// ado缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, Lazy<IAdo>> adoCache;

        /// <summary>
        /// 是否事务
        /// </summary>
        private bool isTran;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项</param>
        public DbContext(IOptionsSnapshot<List<DbOptions>> options) : this(options.Value)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项</param>
        public DbContext(List<DbOptions> options)
        {
            if (options == null || options.Count == 0)
            {
                throw new ArgumentException($"{nameof(options)}不包含任何元素.");
            }

            var list = options.GroupBy(g => g.DbId).Where(a => a.Count() > 1);

            if (list.Any())
            {
                throw new FastException($"数据库ID {string.Join(",", list.Select(s => s.Key))} 重复.");
            }

            adoCache = new ConcurrentDictionary<string, Lazy<IAdo>>();

            ContextId = Guid.NewGuid();
            DbOptions = options;

            Aop = new AopProvider(this);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual IDbContext Clone()
        {
            var dbContext = new DbContext(DbOptions)
            {
                isTran = this.isTran
            };
            return dbContext;
        }

        #region 私有方法
        /// <summary>
        /// Ado循环
        /// </summary>
        /// <param name="action">委托</param>
        private void AdoFeach(Action<IAdo> action)
        {
            foreach (var item in adoCache)
            {
                action.Invoke(item.Value.Value);
            }
        }

        /// <summary>
        /// Ado循环异步
        /// </summary>
        /// <param name="action">委托</param>
        private async Task AdoFeachAsync(Func<IAdo, Task> action)
        {
            foreach (var item in adoCache)
            {
                await action.Invoke(item.Value.Value);
            }
        }
        #endregion

        #region 多租户接口实现

        /// <summary>
        /// 获取Ado
        /// </summary>
        /// <param name="dbId">数据库ID</param>
        /// <returns></returns>
        public IAdo GetAdo(string dbId)
        {
            return adoCache.GetOrAdd(dbId, k => new Lazy<IAdo>(() =>
            {
                var option = DbOptions.FirstOrDefault(f => f.DbId == dbId) ?? throw new FastException($"DbId {dbId} 不存在.");
                var adoProvider = ProviderFactory.CreateAdoProvider(option);
                if (isTran)
                {
                    adoProvider.BeginTran();
                }
                if (ado != null)
                {
                    adoProvider.DbLog = Ado.DbLog;
                    adoProvider.SlaveDbFault = Ado.SlaveDbFault;
                }
                return adoProvider;
            })).Value;
        }

        /// <summary>
        /// 获取Ado含属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAdo GetAdoWithAttr<T>() where T : class, new()
        {
            var entityInfo = typeof(T).GetEntityInfo();
            if (string.IsNullOrWhiteSpace(entityInfo.TenantId))
            {
                throw new FastException("未获取到TenantId.");
            }
            return GetAdo(entityInfo.TenantId);
        }

        /// <summary>
        /// 改变数据库
        /// </summary>
        /// <param name="dbId">数据库ID</param>
        /// <returns></returns>
        public void ChangeDb(string dbId)
        {
            if (Ado.DbOptions.DbId != dbId)
            {
                Ado = GetAdo(dbId);
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            isTran = true;
            AdoFeach(ado =>
           {
               try
               {
                   ado.BeginTran();
               }
               catch
               {
                   try
                   {
                       Retry.Execute(() =>
                       {
                           ado.BeginTran();
                       }, TimeSpan.FromSeconds(3));
                   }
                   catch
                   {
                       throw;
                   }
               }
           });
        }

        /// <summary>
        /// 开启事务异步
        /// </summary>
        public Task BeginTranAsync()
        {
            isTran = true;
            return AdoFeachAsync(async ado =>
            {
                try
                {
                    await ado.BeginTranAsync();
                }
                catch
                {
                    try
                    {
                        await Retry.Execute(async () =>
                        {
                            await ado.BeginTranAsync();
                        }, TimeSpan.FromSeconds(3));
                    }
                    catch
                    {
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            isTran = false;
            AdoFeach(ado =>
           {
               try
               {
                   ado.CommitTran();
               }
               catch
               {
                   try
                   {
                       Retry.Execute(() =>
                       {
                           ado.CommitTran();
                       }, TimeSpan.FromSeconds(3));
                   }
                   catch
                   {
                       throw;
                   }
               }
           });
        }

        /// <summary>
        /// 提交事务异步
        /// </summary>
        public Task CommitTranAsync()
        {
            isTran = false;
            return AdoFeachAsync(async ado =>
            {
                try
                {
                    await ado.CommitTranAsync();
                }
                catch
                {
                    try
                    {
                        await Retry.Execute(async () =>
                        {
                            await ado.CommitTranAsync();
                        }, TimeSpan.FromSeconds(3));
                    }
                    catch
                    {
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public void RollbackTran()
        {
            isTran = false;
            AdoFeach(ado =>
           {
               try
               {
                   ado.RollbackTran();
               }
               catch
               {
                   Retry.Execute(() =>
                   {
                       ado.RollbackTran();
                   }, TimeSpan.FromSeconds(3));
               }
           });
        }

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        /// <returns></returns>
        public Task RollbackTranAsync()
        {
            isTran = false;
            return AdoFeachAsync(async ado =>
            {
                try
                {
                    await ado.RollbackTranAsync();
                }
                catch
                {
                    await Retry.Execute(async () =>
                    {
                        await ado.RollbackTranAsync();
                    }, TimeSpan.FromSeconds(3));
                }
            });
        }
        #endregion

        #region 增 删 改

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IInsert<T> Insert<T>(T entity, IAdo ado) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var type = entity.GetType();
            var insertBuilder = BuilderFactory.CreateInsertBuilder<T>(ado.DbOptions.DbType);

            if (type.Name.StartsWith("Dictionary`"))
            {
                if (entity is not Dictionary<string, object> dictionary)
                {
                    throw new FastException("字典插入请使用Dictionary<string, object>类型.");
                }
                insertBuilder.EntityInfo.ColumnInfos.AddRange(dictionary.Select(s =>
                {
                    var obj = new ColumnInfo()
                    {
                        ColumnName = s.Key,
                        ParameterName = s.Key
                    };

                    if (s.Value == null)
                    {
                        obj.MemberType = typeof(object);
                    }
                    else
                    {
                        var type = s.Value.GetType();
                        obj.MemberType = type;
                        obj.UnderlyingType = type.GetUnderlyingType();
                        if (type.IsClass())
                        {
                            obj.IsJson = true;
                        }
                    }

                    return obj;
                }));
                insertBuilder.DbParameters.AddRange(dictionary.Select(s =>
                {
                    var value = s.Value;
                    if (value != null)
                    {
                        var type = value.GetType();
                        if (type.IsClass())
                        {
                            value = Json.Serialize(value);
                        }
                    }
                    var parameter = new FastParameter(s.Key, value);
                    return parameter;
                }));
            }
            else
            {
                insertBuilder.EntityInfo = type.GetEntityInfo();

                insertBuilder.ComputedValues.AddRange(insertBuilder.EntityInfo.ColumnInfos.ComputedValues(entity));

                insertBuilder.DbParameters = insertBuilder.EntityInfo.ColumnInfos
                    .Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate)
                    .ToList()
                    .GenerateDbParameters(entity);
            }

            insertBuilder.EntityInfo.TargetObj = entity;

            var insertProvider = new InsertProvider<T>(ado, insertBuilder);
            return insertProvider;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IInsert<T> Insert<T>(List<T> list, IAdo ado) where T : class
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (list.Count == 0)
            {
                throw new ArgumentException($"{nameof(list)}元素不能为0.");
            }

            var type = list[0].GetType();
            var insertBuilder = BuilderFactory.CreateInsertBuilder<T>(ado.DbOptions.DbType);

            if (type.Name.StartsWith("Dictionary`"))
            {
                var dictionary = list[0] as Dictionary<string, object> ?? throw new FastException("字典插入请使用Dictionary<string, object>类型.");
                insertBuilder.EntityInfo.ColumnInfos.AddRange(dictionary.Select(s => new ColumnInfo()
                {
                    ColumnName = s.Key
                }));
            }
            else
            {
                insertBuilder.EntityInfo = type.GetEntityInfo();
                insertBuilder.ComputedValues.AddRange(insertBuilder.EntityInfo.ColumnInfos.ComputedValues(list));
            }

            insertBuilder.EntityInfo.TargetObj = list;

            insertBuilder.IsInsertList = true;
            insertBuilder.InsertList = list;

            var insertProvider = new InsertProvider<T>(ado, insertBuilder);
            return insertProvider;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IInsert<T> Insert<T>(T entity) where T : class
        {
            return Insert(entity, Ado);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IInsert<T> Insert<T>(List<T> list) where T : class
        {
            return Insert(list, Ado);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IInsert<T> InsertWithAttr<T>(T entity) where T : class, new()
        {
            return Insert(entity, GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IInsert<T> InsertWithAttr<T>(List<T> list) where T : class, new()
        {
            return Insert(list, GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IDelete<T> Delete<T>(IAdo ado) where T : class
        {
            var type = typeof(T);
            var entityInfo = type.GetEntityInfo();

            var deleteBuilder = BuilderFactory.CreateDeleteBuilder<T>(ado.DbOptions.DbType);
            deleteBuilder.EntityInfo = entityInfo;

            var deleteProvider = new DeleteProvider<T>(ado, deleteBuilder);
            return deleteProvider;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IDelete<T> Delete<T>(T entity, IAdo ado) where T : class
        {
            var type = entity.GetType();

            var entityInfo = type.GetEntityInfo();
            entityInfo.TargetObj = entity;

            var columnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.IsPrimaryKey) ?? throw new ArgumentNullException(nameof(entity), "未获取到标记为KeyAttribute的属性.");

            var deleteBuilder = BuilderFactory.CreateDeleteBuilder<T>(ado.DbOptions.DbType);
            deleteBuilder.EntityInfo = entityInfo;

            var identifier = ado.DbOptions.DbType.GetIdentifier();

            deleteBuilder.Where.Add($"{identifier.Insert(1, columnInfo.ColumnName)} = {ado.DbOptions.DbType.GetSymbol()}{columnInfo.ColumnName}");

            var parameter = new FastParameter(columnInfo.ColumnName, columnInfo.PropertyInfo.GetValue(entity));
            if (columnInfo.DbParameterType != null)
            {
                parameter.DbType = columnInfo.DbParameterType.Value;
            }
            deleteBuilder.DbParameters.Add(parameter);

            var deleteProvider = new DeleteProvider<T>(ado, deleteBuilder);
            return deleteProvider;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IDelete<T> Delete<T>(List<T> list, IAdo ado) where T : class
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (list.Count == 0)
            {
                throw new ArgumentException($"{nameof(list)}元素不能为0.");
            }

            var type = list[0].GetType();

            var entityInfo = type.GetEntityInfo();
            entityInfo.TargetObj = list;

            var deleteBuilder = BuilderFactory.CreateDeleteBuilder<T>(ado.DbOptions.DbType);
            deleteBuilder.EntityInfo = entityInfo;
            deleteBuilder.IsDeleteList = true;
            deleteBuilder.DeleteList = list;

            var deleteProvider = new DeleteProvider<T>(ado, deleteBuilder);
            return deleteProvider;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDelete<T> Delete<T>() where T : class
        {
            return Delete<T>(Ado);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IDelete<T> Delete<T>(T entity) where T : class
        {
            return Delete(entity, Ado);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IDelete<T> Delete<T>(List<T> list) where T : class
        {
            return Delete(list, Ado);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDelete<T> DeleteWithAttr<T>() where T : class, new()
        {
            return Delete<T>(GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IDelete<T> DeleteWithAttr<T>(T entity) where T : class, new()
        {
            return Delete(entity, GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IDelete<T> DeleteWithAttr<T>(List<T> list) where T : class, new()
        {
            return Delete(list, GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IUpdate<T> Update<T>(IAdo ado) where T : class
        {
            var type = typeof(T);
            var updateBuilder = BuilderFactory.CreateUpdateBuilder<T>(ado.DbOptions.DbType);
            updateBuilder.EntityInfo = type.GetEntityInfo();

            var updateProvider = new UpdateProvider<T>(ado, updateBuilder);
            return updateProvider;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IUpdate<T> Update<T>(T entity, IAdo ado) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var type = entity.GetType();
            var updateBuilder = BuilderFactory.CreateUpdateBuilder<T>(ado.DbOptions.DbType);

            if (type.Name.StartsWith("Dictionary`"))
            {
                var dictionary = entity as Dictionary<string, object> ?? throw new FastException("字典更新请使用Dictionary<string, object>类型.");
                updateBuilder.EntityInfo.ColumnInfos.AddRange(dictionary.Select(s =>
                {
                    var obj = new ColumnInfo()
                    {
                        ColumnName = s.Key,
                        ParameterName = s.Key
                    };
                    if (s.Value == null)
                    {
                        obj.MemberType = typeof(object);
                    }
                    else
                    {
                        var type = s.Value.GetType();
                        obj.MemberType = type;
                        obj.UnderlyingType = type.GetUnderlyingType();

                        if (type.IsClass())
                        {
                            obj.IsJson = true;
                        }
                    }
                    return obj;
                }));
                updateBuilder.DbParameters.AddRange(dictionary.Select(s =>
                {
                    var value = s.Value;
                    if (value != null)
                    {
                        var type = value.GetType();
                        if (type.IsClass())
                        {
                            value = Json.Serialize(value);
                        }
                    }
                    var parameter = new FastParameter(s.Key, value);
                    return parameter;
                }));
            }
            else
            {
                updateBuilder.EntityInfo = type.GetEntityInfo();
                updateBuilder.DbParameters = updateBuilder.EntityInfo.ColumnInfos
                    .Where(w => !w.IsNotMapped && !w.IsNavigate)
                    .ToList()
                    .GenerateDbParameters(entity);
            }

            updateBuilder.EntityInfo.TargetObj = entity;

            var updateProvider = new UpdateProvider<T>(ado, updateBuilder);
            return updateProvider;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        private static IUpdate<T> Update<T>(List<T> list, IAdo ado) where T : class
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (list.Count == 0)
            {
                throw new ArgumentException($"{nameof(list)}元素不能为0.");
            }

            var type = list[0].GetType();
            var updateBuilder = BuilderFactory.CreateUpdateBuilder<T>(ado.DbOptions.DbType);

            if (type.Name.StartsWith("Dictionary`"))
            {
                var dictionary = list[0] as Dictionary<string, object> ?? throw new FastException("字典更新请使用Dictionary<string, object>类型.");
                updateBuilder.EntityInfo.ColumnInfos.AddRange(dictionary.Select(s => new ColumnInfo()
                {
                    ColumnName = s.Key
                }));
            }
            else
            {
                updateBuilder.EntityInfo = type.GetEntityInfo();
            }

            updateBuilder.EntityInfo.TargetObj = list;
            updateBuilder.IsUpdateList = true;
            updateBuilder.UpdateList = list;

            var updateProvider = new UpdateProvider<T>(ado, updateBuilder);
            return updateProvider;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IUpdate<T> Update<T>() where T : class
        {
            return Update<T>(Ado);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IUpdate<T> Update<T>(T entity) where T : class
        {
            return Update(entity, Ado);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IUpdate<T> Update<T>(List<T> list) where T : class
        {
            return Update(list, Ado);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IUpdate<T> UpdateWithAttr<T>() where T : class, new()
        {
            return Update<T>(GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IUpdate<T> UpdateWithAttr<T>(T entity) where T : class, new()
        {
            return Update(entity, GetAdoWithAttr<T>());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IUpdate<T> UpdateWithAttr<T>(List<T> list) where T : class, new()
        {
            return Update(list, GetAdoWithAttr<T>());
        }
        #endregion

        #region 查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ado">Ado</param>
        /// <param name="subQuery">子查询</param>
        /// <returns></returns>
        private static IQuery<T> Query<T>(IAdo ado, IQuery subQuery = null) where T : class
        {
            var type = typeof(T);
            var queryBuilder = BuilderFactory.CreateQueryBuilder(ado.DbOptions.DbType);

            var entityInfo = type.GetEntityInfo();
            queryBuilder.EntityInfo = entityInfo;

            if (subQuery != null)
            {
                queryBuilder.IsFromQuery = true;
                queryBuilder.FromQuerySql = subQuery.QueryBuilder.ToSqlString();
                queryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
                queryBuilder.SelectValue = "*";
            }

            return new QueryProvider<T>(ado, queryBuilder);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQuery<T> Query<T>() where T : class
        {
            return Query<T>(Ado);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="subQuery">子查询</param>
        /// <returns></returns>
        public IQuery<T> Query<T>(IQuery<T> subQuery) where T : class
        {
            return Query<T>(Ado, subQuery);
        }

        /// <summary>
        /// 联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isAll">是否全</param>
        /// <param name="querys">查询集合</param>
        /// <returns></returns>
        private IQuery<T> Union<T>(bool isAll, List<IQuery<T>> querys)
        {
            if (querys.Count < 2)
            {
                throw new FastException($"{nameof(querys)} 元素个数必须大于或等于2.");
            }
            var queryBuilder = BuilderFactory.CreateQueryBuilder(Ado.DbOptions.DbType);
            var sqlList = new List<string>();
            foreach (var item in querys)
            {
                sqlList.Add(item.QueryBuilder.ToSqlString());
                queryBuilder.DbParameters.AddRange(item.QueryBuilder.DbParameters.Where(w => !queryBuilder.DbParameters.Exists(e => e.ParameterName == w.ParameterName)));
            }
            queryBuilder.IsUnion = true;
            queryBuilder.SelectValue = "*";
            queryBuilder.Union = string.Join($"\r\n{(isAll ? "UNION ALL" : "UNION")}\r\n", sqlList);
            queryBuilder.EntityInfo.TableName = $"{(isAll ? "UNION_ALL" : "UNION")}_{querys.Count}";
            queryBuilder.EntityInfo.Alias = "p1";
            var queryProvider = new QueryProvider<T>(Ado, queryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象数组</param>
        /// <returns></returns>
        public IQuery<T> Union<T>(params IQuery<T>[] querys)
        {
            return Union(querys.ToList());
        }

        /// <summary>
        /// 联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象列表</param>
        /// <returns></returns>
        public IQuery<T> Union<T>(List<IQuery<T>> querys)
        {
            return Union(false, querys);
        }

        /// <summary>
        /// 全联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象数组</param>
        /// <returns></returns>
        public IQuery<T> UnionAll<T>(params IQuery<T>[] querys)
        {
            return UnionAll(querys.ToList());
        }

        /// <summary>
        /// 全联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象列表</param>
        /// <returns></returns>
        public IQuery<T> UnionAll<T>(List<IQuery<T>> querys)
        {
            return Union(true, querys);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQuery<T> QueryWithAttr<T>() where T : class, new()
        {
            return Query<T>(GetAdoWithAttr<T>());
        }
        #endregion

        /// <summary>
        /// 快速
        /// </summary>
        /// <returns></returns>
        public IFast<T> Fast<T>() where T : class
        {
            return ProviderFactory.CreateFastProvider<T>(Ado);
        }

        #region Dispose实现
        /// <summary>
        /// 释放标识
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">托管对象释放标识</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var item in adoCache)
                    {
                        item.Value.Value.Dispose();
                    }
                }
                disposed = true;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

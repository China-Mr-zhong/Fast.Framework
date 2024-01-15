##  Fast Framework

作者 Mr-zhong

代码改变世界....

##### 一、前言

Fast Framework 基于NET6.0 封装的轻量级 ORM 框架 支持多种数据库 SqlServer Oracle MySql PostgreSql Sqlite

优点： 体积小、原生支持微软特性、流畅API、使用简单、性能高、模型数据绑定采用 Expression、强大的表达式解析、支持多种子查询可实现较为复杂查询、源代码可读性强、支持AOT 编译。

缺点：目前仅支持Db Frist



##### 二、项目明细

| 名称                               | 说明                                   |
| :--------------------------------- | :------------------------------------- |
| Fast.Framework                     | ORM                                    |
| Fast.Framework.Logging             | 文件日志 (扩展项目可不使用)            |
| Fast.Framework.DependencyInjection | 依赖注入 (扩展项目可不使用)  不支持AOT |
| Fast.Framework.Test                | 控制台测试项目                         |
| Fast.Framework.UnitTest            | 单元测试项目                           |
| Fast.Framework.Web.Test            | Web测试项目                            |

##### 三、核心对象

- Ado

  ```C#
                  IAdo ado = new AdoProvider(new DbOptions()
                  {
                      DbId = "1",
                      DbType = DbType.MySQL,
                      ConnectionStrings = "server=localhost;database=Test;user=root;pwd=123456789;port=3306;min pool size=3;max pool size=100;connect timeout=30;"
                  });
  ```
  
- DbContext  支持多租户

  ```C#
                  IDbContext db = new DbContext(new List<DbOptions>() {
                  new DbOptions()
                  {
                      DbId = "1",
                      DbType = DbType.MySQL,
                      ConnectionStrings = "server=localhost;database=Test;user=root;pwd=123456789;port=3306;min pool size=3;max pool size=100;connect timeout=30;"
                  }});
  ```
  
- DbOptions  Json文件配置格式

  ```C#
  "DbOptions": [
      {
        "DbId": 1,
        "DbType": "SQLServer",
        "ConnectionStrings": "server=localhost;database=Test;user=sa;pwd=123456789;min pool size=3;max pool size=100;connect timeout=120;"
      }]
  ```
  
- 主从分离（读写分离）配置

  ```C#
    "DbOptions": [
      {
        "DbId": 2,
        "DbType": "MySQL",
        "IsDefault": true,
        "ConnectionStrings": "server=localhost;database=Test;user=root;pwd=123456789;port=3306;min pool size=0;max pool size=100;connect timeout=120;AllowLoadLocalInfile=true;",
        "UseMasterSlaveSeparation": true,//使用主从分离 注意所有事务将强制走主库
        "SlaveItems": [
          {
            "Weight": 80,
            "ConnectionStrings": "server=localhost;database=Test1;user=root;pwd=123456789;port=3306;min pool size=0;max pool size=100;connect timeout=120;AllowLoadLocalInfile=true;",
            "Description": "从库连接配置"
          },
          {
            "Weight": 20,
            "ConnectionStrings": "server=localhost;database=Test2;user=root;pwd=123456789;port=3306;min pool size=0;max pool size=100;connect timeout=120;AllowLoadLocalInfile=true;",
            "Description": "从库连接配置"
          }
        ],
        "Description": "主库连接配置"
      }
    ]
  ```
  
  
  
- Asp.net Core 依赖注入

  ```C#
  // 注册服务 
  var builder = WebApplication.CreateBuilder(args);
  
  // 添加数据库上下文
  builder.Services.AddFastDbContext();
  
  // 从Json配置文件加载数据库选项
  builder.Services.Configure<List<DbOptions>>(builder.Configuration.GetSection("DbOptions"));
  
  // 产品服务类 通过构造方法注入
  public class ProductService
  {
      /// <summary>
      /// 数据库
      /// </summary>
      private readonly IDbContext db;
  
      /// <summary>
      /// 构造方法
      /// </summary>
      /// <param name="db">数据库</param>
      public ProductService(IDbContext db)
      {
          this.db = db;
      }
  }
  ```

##### 四、插入

- 实体对象插入

  ```c#
              var product = new Product()
              {
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              var result = db.Insert(product).Exceute();
  ```
  
- 实体对象插入并返回自增ID 仅支持 SQLServer MySQL SQLite

  ```C#
              var product = new Product()
              {
                  ProductCode = "1001",
                  ProductName = "测试产品1"
              };
              var result = db.Insert(product).ExceuteReturnIdentity();
  ```
  
- 实体对象列表插入

  ```c#
              var list = new List<Product>();
              for (int i = 0; i < 2100; i++)
              {
                  list.Add(new Product()
                  {
                      ProductCode = $"编号{i + 1}",
                      ProductName = $"名称{i + 1}"
                  });
              }
              var result = db.Insert(list).Exceute();
  ```
  
- 匿名对象插入

  ```C#
              var obj = new
              {
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              //注意:需要使用As方法显示指定表名称
              var result = db.Insert(obj).As("Product").Exceute();
  ```
  
- 匿名对象列表插入

  ```C#
              var list = new List<object>();
              for (int i = 0; i < 2100; i++)
              {
                  list.Add(new
                  {
                      ProductCode = $"编号{i + 1}",
                      ProductName = $"名称{i + 1}"
                  });
              }
              //注意:需要使用As方法显示指定表名称
              var result = db.Insert(list).As("Product").Exceute();
  ```
  
- 字典插入

  ```c#
              var product = new Dictionary<string, object>()
              {
                  {"ProductCode","1001"},
                  { "ProductName","测试商品1"}
              };
              var result = db.Insert(product).As("Product").Exceute();
  ```
  
- 字典列表插入

  ```c#
              var list = new List<Dictionary<string, object>>();
              for (int i = 0; i < 2100; i++)
              {
                  list.Add(new Dictionary<string, object>()
                  {
                      {"ProductCode","1001"},
                      { "ProductName","测试商品1"}
                   });
              }
              var result = db.Insert(list).As("Product").Exceute();
  ```

##### 五、删除

- 实体对象删除

  ```C#
              var product = new Product()
              {
                  ProductId = 1,
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              var result = db.Delete(product).Exceute();
  ```
  
- 实体对象列表删除

  ```c#
              var list = new List<Product>();
              for (int i = 0; i < 2100; i++)
              {
                  list.Add(new Product()
                  {
                      ProductCode = $"编号{i + 1}",
                      ProductName = $"名称{i + 1}"
                  });
              }
              var result = db.Delete(list).Exceute();
  ```
  
  
  
- 无条件删除

  ```C#
              var result = db.Delete<Product>().Exceute();
  ```
  
- 条件删除

  ```C#
              var result = await db.Delete<Product>().Where(w => w.ProductId == 1).ExceuteAsync();
  ```

- 逻辑删除

  ```c#
                 /*     
                  可用特性标记逻辑删除列 仅支持 int bool datetime 类型,其它类型不合适
  
                  /// <summary>
                  /// 删除标记
                  /// </summary>
                  [Logic]
                  public bool DeleteMark { get; set; }
  
                  */
  
  				//额外设置其它属性值,使用SetColumns方法前需先使用IsLogic方法
  				//类型逻辑删除
                  var result1 = db.Delete<Product>().IsLogic().SetColumns(c => new Product()
                  {
                      ModifyTime = DateTime.Now
                  }).Where(w => w.ProductId == 1).Exceute();
  
                  //对象逻辑删除
                  var result2 = db.Delete(new Product() { ProductId = 1 }).IsLogic().SetColumns(c => new Product()
                  {
                      ModifyTime = DateTime.Now
                  }).Exceute();
  
                  //特殊逻辑删除（不想建实体类可以用该方式）
                  var result3 = db.Delete<object>().As("Product").IsLogic().SetColumns(c => new 
                  {
                      ModifyTime = DateTime.Now
                  }).Exceute();
  ```

  

- 特殊删除

  ```c#
  			//特殊用法 如需单个条件或多个可搭配 Where或WhereColumns方法
              var result = await db.Delete<object>().As("Product").ExceuteAsync();
              Console.WriteLine($"无实体删除 受影响行数 {result}");
  ```
  

##### 六、更新

- 实体对象更新

  ```c#
              var product = new Product()
              {
                  ProductId = 1,
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              //注意:标记KeyAuttribute特性属性或使用Where条件，为了安全起见全表更新将必须使用Where方法
              var result = db.Update(product).Exceute();
  ```
  
- 指定列更新

  ```c#
              var result = db.Update<Product>(new Product() { ProductCode = "1001", ProductName = "1002" }).UpdateColumns(c=> new { c.ProductCode , c.ProductName }).Exceute();
  			// 推荐使用表达式 c=>new {} 好处更改属性名称可以同步修改
  ```
  
- 忽略列更新

  ```c#
              var result = db.Update<Product>(new Product() { ProductCode = "1001", ProductName = "1002" }).IgnoreColumns(c=> new { c.Custom1 }).Exceute();
              // 同上使用方法一样
  ```
  
- 实体对象列表更新

  ```c#
              var list = new List<Product>();
              for (int i = 0; i < 2022; i++)
              {
                  list.Add(new Product()
                  {
                      ProductCode = $"编号{i + 1}",
                      ProductName = $"名称{i + 1}"
                  });
              }
              //注意:标记KeyAuttribute特性属性或使用WhereColumns方法指定更新条件列
              var result = db.Update(list).Exceute();
  ```
  
- 匿名对象更新

  ```C#
              var obj = new
              {
                  ProductId = 1,
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              //注意:需要显示指定表名称 以及更新条件 使用 Where或者WhereColumns方法均可
              var result = db.Update(obj).As("product").WhereColumns("ProductId").Exceute();
  ```
  
- 匿名对象列表更新

  ```c#
              var list = new List<object>();
              for (int i = 0; i < 2022; i++)
              {
                  list.Add(new
                  {
                      ProductId = i + 1,
                      ProductCode = $"编号{i + 1}",
                      ProductName = $"名称{i + 1}"
                  });
              }
              //由于是匿名对象需要显示指定表名称,使用WhereColumns方法指定更新条件列
              var result = db.Update(list).As("Product").WhereColumns("ProductId").Exceute();
  ```
  
- 字典更新

  ```c#
              var product = new Dictionary<string, object>()
              {
                  { "ProductId",1},
                  {"ProductCode","1001"},
                  { "ProductName","测试商品1"}
              };
              var result = db.Update(product).As("Product").WhereColumns("ProductId").Exceute();
  ```
  
- 字典列表更新

  ```c#
              var list = new List<Dictionary<string, object>>();
              for (int i = 0; i < 2022; i++)
              {
                  list.Add(new Dictionary<string, object>()
                  {
                      { "ProductId",i+1},
                      {"ProductCode",$"更新编号:{i+1}"},
                      { "ProductName",$"更新商品:{i + 1}"}
                  });
              }
              var result = db.Update(list).As("Product").WhereColumns("ProductId").Exceute();
  ```
  
- 设置列更新

  ```c#
              // 设置列更新
  			db.Update<Product>().SetColumns(c => new Product()
              {
                  ProductCode = "1001",
                  ProductName = "测试产品1"
              }).Where(w => w.ProductId == 1).Exceute();
  ```
  
- 指定条件更新

  ```C#
              var product = new Product()
              {
                  ProductId = 1,
                  ProductCode = "1001",
                  ProductName = "测试商品1"
              };
              var result = db.Update(product).Where(p => p.ProductId == 100).Exceute();
              Console.WriteLine($"表达式更新 受影响行数 {result}");
  ```
  
- 并发更新  乐观锁-版本控制 

  ```c#
              //注意:仅支持非列表更新 版本列数据类型仅支持 object、string、Guid 时间类型存在精度丢失所以不做支持
  			var obj = db.Query<Product>().Where(w => w.ProductId == 1).Frist();
              obj.Custom1 = "测试版本控制修改";
  			//参数为 true 更新失败将抛出异常
              var result = db.Update(obj).ExceuteWithOptLock(true);
  ```

  


##### 七、查询

- 单一查询

  ```C#
              var data = db.Query<Product>().First();
  ```

- 列表查询

  ```C#
              var data = db.Query<Product>().ToList();
  ```

- 返回单个字典

  ```c#
              var data = db.Query<Product>().ToDictionary();
  ```

- 返回字典列表

  ```c#
              var data = db.Query<Product>().ToDictionaryList();
  ```

- 分页查询

  ```C#
              //分页查询不返回总数
              var data = db.Query<Product>().ToPageList(1,100);
              //分页查询返回总数
  			var total = 0;//定义总数变量
  			var data = db.Query<Product>().ToPageList(1, 1, ref total);
              Console.WriteLine($"总数：{total}");
  ```
  
- 计数查询

  ```C#
              var data = db.Query<Product>().Count();
  ```
  
- 任何查询

  ```c#
              var data = db.Query<Product>().Any();
  ```

- 条件查询

  ```c#
              var data = db.Query<Product>().Where(w => w.ProductId == 1).ToList;
  ```
  
- Like 查询

  ```C#
              var data = db.Query<Product>().Where(w => w.ProductName.StartsWith("左模糊") || w.ProductName.EndsWith("右模糊") || w.ProductName.Contains("全模糊")).ToList();
  ```

- Not Like查询

  ```C#
  var data = db.Query<Product>().Where(w => !w.ProductName.StartsWith("左模糊") || !w.ProductName.EndsWith("右模糊") || !w.ProductName.Contains("全模糊")).ToList();
  ```
  
- Select查询 (选择字段)

  ```C#
              var data = db.Query<Product>().Select(s => new
              {
                  s.ProductId,
                  s.ProductName
              }).ToList();
  ```

- Select查询 (Case When)

  ```C#
                  var data = db.Query<Product>().Select(s => new
                  {
                      CaseTest1 = SqlFunc.Case(s.Custom1).When("1").Then("xx1").When("2").Then("xx2").Else("xx3").End(),
                      CaseTest2 = SqlFunc.CaseWhen<string>(s.Custom1 == "1").Then("xx1").When(s.Custom1 == "2").Then("xx2").Else("xx3").End()
                  }).ToList();
  ```

- 分组查询

  ```C#
              var data = db.Query<Product>().GroupBy(s => new
              {
                  s.ProductId,
                  s.ProductName
              }).ToList();
  ```

- 分组聚合查询

  ```c#
              var sql = db.Query<Order>().InnerJoin<OrderDetail>((a, b) => a.OrderId == b.OrderId).GroupBy((a, b) => new
              {
                  a.OrderCode
              }).Select((a, b) => new
              {
                  a.OrderCode,
                  Sum_Qty = SqlFunc.Sum(b.Qty)//支持嵌套
              }).ToList();
  ```
  
- 排序查询

  ```c#
              var data = db.Query<Product>().OrderBy(s => new
              {
                  s.CreateTime
              }).ToList();
              //这是多个字段排序使用方法 还有其它重载方法
  ```

- Having查询

  ```C#
              var data = db.Query<Product>().GroupBy(s => new
              {
                  s.ProductId,
                  s.ProductName
              }).Having(s => SqlFunc.Count(s.ProductId) > 1).ToList();
              //必须先使用GroupBy方法 懂得都懂
  ```

- 联表查询

  ```C#
              var data = db.Query<Product>().LeftJoin<Class1>((a, b) => a.ProductId == b.ProductId).ToList();
              // 右连接 RightJoin 内连接 InnerJoin 全连接 FullJoin
  ```
  
- 联合查询

  ```c#
              var query1 = db.Query<Product>();
              var query2 = db.Query<Product>();
              db.Union(query1, query2);//联合
              db.UnionAll(query1, query2);//全联合
              //执行查询调用Toxx方法
  ```
  
- 导航查询 (支持无限层级)

  ```c#
                  /// <summary>
                  /// 类别
                  /// </summary>
                  public class Category
                  {
                      /// <summary>
                      /// 类别ID
                      /// </summary>
                      [Key]
                      public int CategoryId { get; set; }
                  
                      /// <summary>
                      /// 类别名称
                      /// </summary>
                      public string CategoryName { get; set; }
                  
                      /// <summary>
                      /// 产品 Navigate MainName和ChildName 可不显示指定，会自动查找主键匹配或ID为结尾的属性
                      /// </summary>
                      [Navigate(MainName = nameof(CategoryId), ChildName = nameof(Product.CategoryId))]
                      public IEnumerable<Product> Products { get; set; }
                  
                  }
  
                  var data = db.Query<Category>()
                      .Include(i => i.Products)
                      .ToList();
  ```
  
  
  
- 查询并插入  仅支持同实例的数据库 跨库 个人还是建议 用事务分开写查询和插入

  ```c#
                  //方式1
                  var result1 = db.Query<Product>().Where(w => w.ProductId == 1489087).Select(s => new
                  {
                      s.ProductCode,
                      s.ProductName
                  }).Insert<Product>(p => new
                  {
                      p.ProductCode,
                      p.ProductName
                  });
  
                  //方式2
                  var result2 = db.Query<Product>().Where(w => w.ProductId == 1489087).Select(s => new
                  {
                      s.ProductCode,
                      s.ProductName
                  }).Insert("表名称 同实例不同库 可以使用 db.数据库名称.表名称 ", "列名称1", "列名称2");
  
                  //方式3 需要注意同方式2 一样
                  var result3 = db.Query<Product>().Where(w => w.ProductId == 1489087).Select(s => new
                  {
                      s.ProductCode,
                      s.ProductName
                  }).Insert("表名称 同实例不同库 可以使用 db.数据库名称.表名称 ", new List<string>() { "列名称1" });
  ```

- In查询

  ```c#
                  var data1 = db.Query<Product>().Where(w => new List<string>(){"1001", "1002"}.Contains(w.ProductCode)).ToList();
  ```
  
- Select嵌套查询和子查询

  ```c#
                  var data1 = db.Query<Product>().Select(s => new
                  {
                      XX = db.Query<Product>().Select(s => 1).First()//需调用返回结果的方法 否则无法解析
                  }).First();
  
  				//进价用法，下面示例方法的重载均支持
                  var count = 0;
                  var refAsync = new RefAsync<int>();
  
                  var query = db.Query<Product>().Select(s => new
                  {
                      WithAttr_First = db.QueryWithAttr<Product>().First(),
                      WithAttr_FirstAsync = db.QueryWithAttr<Product>().FirstAsync(),
                      WithAttr_ToList = db.QueryWithAttr<Product>().ToList(),
                      WithAttr_ToListAsync = db.QueryWithAttr<Product>().ToListAsync(),
                      First_1 = db.Query<Category>().Select(s => 1).First(),//解析成Sql
                      First = db.Query<Category>().First(),
                      FirstAsync = db.Query<Category>().FirstAsync(),
                      ToArray = db.Query<Category>().ToArray(),
                      ToArrayAsync = db.Query<Category>().ToArrayAsync(),
                      ToList = db.Query<Category>().ToList(),
                      ToListAsync = db.Query<Category>().ToListAsync(),
                      ToPageList = db.Query<Category>().ToPageList(1, 10),
                      ToPageListAsync = db.Query<Category>().ToPageListAsync(1, 10),
                      ToPageList_Count = db.Query<Category>().ToPageList(1, 10, ref count),
                      ToPageListAsync_Count = db.Query<Category>().ToPageListAsync(1, 10, refAsync),
                      ToDictionary = db.Query<Category>().ToDictionary(),
                      ToDictionaryAsync = db.Query<Category>().ToDictionaryAsync(),
                      ToDictionaryList = db.Query<Category>().ToDictionaryList(),
                      ToDictionaryListAsync = db.Query<Category>().ToDictionaryListAsync(),
                      ToDictionaryPageList = db.Query<Category>().ToDictionaryPageList(1, 10),
                      ToDictionaryPageListAsync = db.Query<Category>().ToDictionaryPageListAsync(1, 10),
                      ToDictionaryPageList_Count = db.Query<Category>().ToDictionaryPageList(1, 10, ref count),
                      ToDictionaryPageListAsync_Count = db.Query<Category>().ToDictionaryPageListAsync(1, 10, refAsync),
                      ToDataTable = db.Query<Category>().ToDataTable(),
                      ToDataTableAsync = db.Query<Category>().ToDataTableAsync(),
                      Max = db.Query<Category>().Max(a => a.CategoryId),//解析成Sql
                      MaxAsync = db.Query<Category>().MaxAsync(a => a.CategoryId),
                      Min = db.Query<Category>().Min(a => a.CategoryId),//解析成Sql
                      MinAsync = db.Query<Category>().MinAsync(a => a.CategoryId),
                      Count = db.Query<Category>().Count(),//解析成Sql
                      CountAsync = db.Query<Category>().CountAsync(),
                      Sum = db.Query<Category>().Sum(s => s.CategoryId),//解析成Sql
                      SumAsync = db.Query<Category>().SumAsync(s => s.CategoryId),
                      Avg = db.Query<Category>().Avg(s => s.CategoryId),//解析成Sql
                      AvgAsync = db.Query<Category>().AvgAsync(s => s.CategoryId)
                  });
  				var data2= query.First();
  ```
  
- From子查询

  ```C#
                  var subQuery2 = db.Query<Product>().Select(s=>new
                  {
                      s.ProductId,
                      s.CategoryId,
                      s.ProductCode,
                      s.ProductName,
                      s.DeleteMark
                  });
                  var data = db.Query(subQuery2).ToList();
  ```
  
- Join子查询

  ```C#
                  var subQuery1 = db.Query<Product>().Select(s => new
                  {
                      s.ProductId,
                      s.CategoryId,
                      s.ProductCode,
                      s.ProductName,
                      s.DeleteMark
                  });
                  var data = db.Query<Category>().InnerJoin(subQuery1, (a, b) => a.CategoryId == b.CategoryId).ToList();
  ```
  
- Include查询

  ```c#
                  // 联表条件 默认优先匹配主键 其次带有ID结尾的名称
                  var data = db.Query<Category>().Include(i => i.Products).ToList();
  ```
  
- Exists查询

  ```c#
                  var data = db.Query<Product>()
                      .Where(w => db.Query<Product>().WhereIF(!string.IsNullOrWhiteSpace("测试"), a => a.ProductId == 1).Select(s => 1).Any())
                      .Select(s => new
                      {
                          s.ProductId,
                          s.ProductCode
                      }).ToList();
  ```

- 查询绑定字段（注意 字段必须是公开的，否则绑定外部无法访问，没有意义）

  ```c#
                  //当某些字段需要参与计算并且不返回前端时推荐用字段绑定，无需从A实体转换到B实体，强烈推荐此方式
  				var data = db.Query<Product>().Select(s => new Product()
                  {
                      _xx = s.ProductName
                  }).First();
  ```

##### 八、Lambda表达式

- 动态表达式

  ```c#
                  var ex = DynamicWhereExp.Create<Product>().AndIF(1 == 1, a => a.DeleteMark == true).Build();
                  var data = db.Query<Product>().Where(ex).ToList();
  ```

- Sql函数    自定义函数  使用SqlFunc类

  - SqlServer

    - 类型转换

      |  方法名称  |           解析示例值            |     说明      | 自定义函数 |
      | :--------: | :-----------------------------: | :-----------: | :--------: |
      |  ToString  |   CONVERT( VARCHAR(255),123)    | 转换 VARCHAR  |     否     |
      | ToDateTime | CONVERT( DATETIME,‘2022-09-16’) | 转换 DATETIME |     否     |
      | ToDecimal  |  CONVERT( DECIMAL(10,6),‘123’)  | 转换 DECIMAL  |     否     |
      |  ToDouble  |  CONVERT( NUMERIC(10,6),‘123’)  | 转换 NUMERIC  |     否     |
      |  ToSingle  |      CONVERT( FLOAT,‘123’)      |  转换 FLOAT   |     否     |
      |  ToInt32   |       CONVERT( INT,‘123’)       |   转换 INT    |     否     |
      |  ToInt64   |     CONVERT( BIGINT,‘123’)      |  转换 BIGINT  |     否     |
      | ToBoolean  |        CONVERT( BIT,‘1’)        |   转换 BIT    |     否     |
      |   ToChar   |      CONVERT( CHAR(2),'x')      |   转换 CHAR   |     否     |

    - 聚合函数

      | 方法名称 |   解析示例值    |  说明  | 自定义函数 |
      | :------: | :-------------: | :----: | :--------: |
      |   Max    |  MAX( a.[xx] )  | 最大值 |     是     |
      |   Min    |  MIN( a.[xx] )  | 最小值 |     是     |
      |  Count   | COUNT( a.[xx] ) |  计数  |     是     |
      |   Sum    |  SUM( a.[xx] )  |  合计  |     是     |
      |   Avg    |  AVG( a.[xx] )  |  平均  |     是     |

    - 数学函数

      | 方法名称 |     解析示例值     |   说明   | 自定义函数 |
      | :------: | :----------------: | :------: | :--------: |
      |   Abs    |   ABS( a.[xx] )    |  绝对值  |     是     |
      |  Round   | ROUND( a.[xx] ,2 ) | 四舍五入 |     是     |

    - 字符串函数

      |  方法名称  |        解析示例值         |            说明            | 自定义函数 |
      | :--------: | :-----------------------: | :------------------------: | :--------: |
      | StartsWith |       LIKE 'xx'+‘%’       |           左模糊           |     否     |
      |  EndsWith  |       LIKE ‘%’+'xx'       |           右模糊           |     否     |
      |  Contains  |     LIKE ‘%’+'xx'+‘%’     |           全模糊           |     否     |
      | SubString  | SUBSTRING( 'xxxxxx' ,1,3) |            截取            |     否     |
      |  Replace   |  REPLACE( 'xxx','x','y')  |            替换            |     否     |
      |    Len     |       LEN( 'xxx' )        |            长度            |     是     |
      | TrimStart  |      LTRIM( ' xx ' )      |        修剪起始空格        |     否     |
      |  TrimEnd   |      RTRIM( ' xx ' )      |        修剪末尾空格        |     否     |
      |  ToUpper   |       UPPER( 'xx' )       |            大写            |     否     |
      |  ToLower   |       LOWER( 'xx' )       |            小写            |     否     |
      |   Concat   |  CONCAT(a.[xx1],a.[xx2])  |         字符串拼接         |     否     |
      | Operation  | [CreateTime]  >=  @Now_1  | 日期、数值、字符串范围比较 |     是     |
  
    - 日期函数
  
      |    方法名称     |           解析示例值            |   说明   | 自定义函数 |
      | :-------------: | :-----------------------------: | :------: | :--------: |
      |    DateDiff     |  DATEDIFF( DAY ,a.[xx],b.[xx])  | 日期相差 |     是     |
      |    AddYears     |    DATEADD( YEAR,a.[xx],1 )     | 添加年份 |     否     |
      |    AddMonths    |    DATEADD( MONTH,a.[xx],1 )    | 添加月份 |     否     |
      |     AddDays     |     DATEADD( DAY,a.[xx],1 )     | 添加天数 |     否     |
      |    AddHours     |    DATEADD( HOUR,a.[xx],1 )     |  添加时  |     否     |
      |   AddMinutes    |   DATEADD( MINUTE,a.[xx],1 )    |  添加分  |     否     |
      |   AddSeconds    |   DATEADD( SECOND,a.[xx],1 )    |  添加秒  |     否     |
      | AddMilliseconds | DATEADD( MILLISECOND,a.[xx],1 ) | 添加毫秒 |     否     |
      |      Year       |         YEAR( a.[xx] )          | 获取年份 |     是     |
      |      Month      |         MONTH( a.[xx] )         | 获取月份 |     是     |
      |       Day       |          DAY( a.[xx] )          | 获取天数 |     是     |
  
    - 其它函数
  
      | 方法名称 |       解析示例值        |   说明   | 自定义函数 |
      | :------: | :---------------------: | :------: | :--------: |
      | NewGuid  |         NEWID()         | 获取GUID |     否     |
      |  Equals  | p.[ProductCode] = '123' |   比较   |     否     |
      |  IsNull  |    ISNULL(a.[xx],0)     | 是否为空 |     是     |
      |   Case   |          CASE           |   case   |     是     |
      |   When   |          WHEN           |   when   |     是     |
      |   Then   |          THEN           |   then   |     是     |
      |   Else   |          ELSE           |   else   |     是     |
      |   End    |           END           |   end    |     是     |
    
  - MySql
    
    - 类型转换
      |  方法名称  |            解析示例值             |        说明        | 自定义函数 |
      | :--------: | :-------------------------------: | :----------------: | :--------: |
      |  ToString  |   CAST( a.\`xx\` AS CHAR(510) )   |   转换 CHAR(510)   |     否     |
      | ToDateTime |   CAST( a.\`xx\` AS DATETIME )    |   转换 DATETIME    |     否     |
      | ToDecimal  | CAST( a.\`xx\` AS DECIMAL(10,6) ) | 转换 DECIMAL(10,6) |     否     |
      |  ToDouble  | CAST( a.\`xx\` AS DECIMAL(10,6) ) | 转换 DECIMAL(10,6) |     否     |
      |  ToInt32   |  CAST( a.\`xx\` AS DECIMAL(10) )  |  转换 DECIMAL(10)  |     否     |
      |  ToInt64   |  CAST( a.\`xx\` AS DECIMAL(19) )  |  转换 DECIMAL(19)  |     否     |
      | ToBoolean  |   CAST( a.\`xx\`  AS UNSIGNED )   |   转换 UNSIGNED    |     否     |
      |   ToChar   |    CAST( a.\`xx\` AS CHAR(2) )    |    转换 CHAR(2)    |     否     |
    
    - 聚合函数
      | 方法名称 |    解析示例值     |  说明  | 自定义函数 |
      | :------: | :---------------: | :----: | :--------: |
      |   Max    |  MAX( a.\`xx\` )  | 最大值 |     是     |
      |   Min    |  MIN( a.\`xx\` )  | 最小值 |     是     |
      |  Count   | COUNT( a.\`xx\` ) |  计数  |     是     |
      |   Sum    |  SUM( a.\`xx\` )  |  合计  |     是     |
      |   Avg    |  AVG( a.\`xx\` )  |  平均  |     是     |
    
    - 数学函数
      | 方法名称 |      解析示例值      |   说明   | 自定义函数 |
      | :------: | :------------------: | :------: | :--------: |
      |   Abs    |   ABS( a.\`xx\` )    |  绝对值  |     是     |
      |  Round   | ROUND( a.\`xx\` ,2 ) | 四舍五入 |     是     |
    
    - 字符串函数
      |  方法名称  |         解析示例值          |            说明            | 自定义函数 |
      | :--------: | :-------------------------: | :------------------------: | :--------: |
      | StartsWith |   LIKE CONCAT( 'xx','%' )   |           左模糊           |     否     |
      |  EndsWith  |   LIKE CONCAT( '%','xx' )   |           右模糊           |     否     |
      |  Contains  | LIKE CONCAT( '%','xx','%' ) |           全模糊           |     否     |
      | SubString  | SUBSTRING(  'xxxxxx' ,1,3 ) |            截取            |     否     |
      |  Replace   |  REPLACE( 'xxx','x','y' )   |            替换            |     否     |
      |    Len     |        LEN( 'xxx' )         |            长度            |     是     |
      |    Trim    |       TRIM( ' xx ' )        |          修剪空格          |     否     |
      | TrimStart  |       LTRIM( ' xx ' )       |        修剪起始空格        |     否     |
      |  TrimEnd   |       RTRIM( ' xx ' )       |        修剪末尾空格        |     否     |
      |  ToUpper   |        UPPER( 'xx' )        |            大写            |     否     |
      |  ToLower   |        LOWER( 'xx' )        |            小写            |     否     |
      |   Concat   | CONCAT(a.\`xx1\`,a.\`xx2\`) |         字符串拼接         |     否     |
      | Operation  |  \`CreateTime\` >=  @Now_1  | 日期、数值、字符串范围比较 |     是     |
      
    - 日期函数
      |    方法名称     |                  解析示例值                   |         说明          | 自定义函数 |
      | :-------------: | :-------------------------------------------: | :-------------------: | :--------: |
      |    DateDiff     |         DATEDIFF( a.\`xx\`,b.\`xx\` )         | 日期相差 返回相差天数 |     是     |
      |  TimestampDiff  |    TIMESTAMPDIFF( DAY,a.\`xx\`,b.\`xx\` )     | 日期相差 指定时间单位 |     是     |
      |    AddYears     |     DATE_ADD( a.\`xx\`,INTERVAL 1 YEAR )      |       添加年份        |     否     |
      |    AddMonths    |     DATE_ADD( a.\`xx\`,INTERVAL 1 MONTH )     |       添加月份        |     否     |
      |     AddDays     |      DATE_ADD( a.\`xx\`,INTERVAL 1 DAY )      |       添加天数        |     否     |
      |    AddHours     |     DATE_ADD( a.\`xx\`,INTERVAL 1 HOUR )      |        添加时         |     否     |
      |   AddMinutes    |    DATE_ADD( a.\`xx\`,INTERVAL 1 MINUTE )     |        添加分         |     否     |
      |   AddSeconds    |    DATE_ADD( a.\`xx\`,INTERVAL 1 SECOND )     |        添加秒         |     否     |
      | AddMilliseconds | DATE_ADD( a.\`xx\`,INTERVAL 1 MINUTE_SECOND ) |       添加毫秒        |     否     |
      |      Year       |               YEAR( a.\`xx\` )                |       获取年份        |     是     |
      |      Month      |               MONTH( a.\`xx\` )               |       获取月份        |     是     |
      |       Day       |                DAY( a.\`xx\` )                |       获取天数        |     是     |
    
    - 其它函数
      | 方法名称 |        解析示例值         |   说明   | 自定义函数 |
      | :------: | :-----------------------: | :------: | :--------: |
      | NewGuid | UUID() | 获取GUID | 否 |
      |  Equals  | p.\`ProductCode\` = '123' |   比较   |     否     |
      |  IfNull  |   IFNULL( a.\`xx\`,0 )    | 如果为空 |     是     |
      |   Case   |          CASE           |   case   |     是     |
      |   When   |          WHEN           |   when   |     是     |
      |   Then   |          THEN       |   then   |     是     |
      |   Else   |          ELSE           |   else   |     是     |
      |   End    |           END           |   end    |     是     |
    
  - Oracle
    
    - 类型转换
    
      |  方法名称  |                   解析示例值                    |     说明      | 自定义函数 |
      | :--------: | :---------------------------------------------: | :-----------: | :--------: |
      |  ToString  |         CAST( a."xx" AS VARCHAR(255) )          | 转换 VARCHAR  |     否     |
      | ToDateTime | TO_TIMESTAMP( a."xx" ,'yyyy-MM-dd hh:mi:ss.ff') | 转换 DATETIME |     否     |
      | ToDecimal  |         CAST( a."xx" AS DECIMAL(10,6) )         | 转换 DECIMAL  |     否     |
      |  ToDouble  |            CAST( a."xx" AS NUMBER )             |  转换 NUMBER  |     否     |
      |  ToSingle  |             CAST( a."xx" AS FLOAT )             |  转换 FLOAT   |     否     |
      |  ToInt32   |              CAST( a."xx" AS INT )              |   转换 INT    |     否     |
      |  ToInt64   |            CAST( a."xx" AS NUMBER )             |  转换 NUMBER  |     否     |
      | ToBoolean  |            CAST( a."xx" AS CHAR(1) )            |   转换 CHAR   |     否     |
      |   ToChar   |            CAST( a."xx" AS CHAR(2) )            |   转换 CHAR   |     否     |
  
    - 聚合函数
      | 方法名称 |   解析示例值    |  说明  | 自定义函数 |
      | :------: | :-------------: | :----: | :--------: |
      |   Max    |  MAX( a."xx" )  | 最大值 |     是     |
      |   Min    |  MIN( a."xx" )  | 最小值 |     是     |
      |  Count   | COUNT( a."xx" ) |  计数  |     是     |
      |   Sum    |  SUM( a."xx" )  |  合计  |     是     |
      |   Avg    |  AVG( a."xx" )  |  平均  |     是     |
    
    - 数学函数
      | 方法名称 |     解析示例值     |   说明   | 自定义函数 |
      | :------: | :----------------: | :------: | :--------: |
      |   Abs    |   ABS( a."xx" )    |  绝对值  |     是     |
      |  Round   | ROUND( a."xx" ,2 ) | 四舍五入 |     是     |
    
    - 字符串函数
      |  方法名称  |         解析示例值          |            说明            | 自定义函数 |
      | :--------: | :-------------------------: | :------------------------: | :--------: |
      | StartsWith |   LIKE CONCAT( 'xx','%' )   |           左模糊           |     否     |
      |  EndsWith  |   LIKE CONCAT( '%','xx' )   |           右模糊           |     否     |
      |  Contains  | LIKE CONCAT( '%','xx','%' ) |           全模糊           |     否     |
      | SubString  |  SUBSTRING( 'xxxxxx' ,1,3)  |            截取            |     否     |
      |  Replace   |   REPLACE( 'xxx','x','y')   |            替换            |     否     |
      |   Length   |       LENGTH( 'xxx' )       |            长度            |     是     |
      | TrimStart  |       LTRIM( ' xx ' )       |        修剪起始空格        |     否     |
      |  TrimEnd   |       RTRIM( ' xx ' )       |        修剪末尾空格        |     否     |
      |  ToUpper   |        UPPER( 'xx' )        |            大写            |     否     |
      |  ToLower   |        LOWER( 'xx' )        |            小写            |     否     |
      |   Concat   |   CONCAT(a."xx1",a."xx2")   |         字符串拼接         |     否     |
      | Operation  |  ”CreateTime“  >=  @Now_1   | 日期、数值、字符串范围比较 |     是     |
      
    - 日期函数
      | 方法名称 |          解析示例值          |   说明   | 自定义函数 |
      | :------: | :--------------------------: | :------: | :--------: |
      |   Year   | EXTRACT( YEAR FROM a."xx" )  | 获取年份 |     是     |
      |  Month   | EXTRACT( MONTH FROM a."xx" ) | 获取月份 |     是     |
      |   Day    |  EXTRACT( DAY FROM a."xx" )  | 获取天数 |     是     |
      
    - 其它函数
      | 方法名称 |       解析示例值        |  说明   | 自定义函数 |
      | :------: | :---------------------: | :-----: | :--------: |
      |  Equals  | p."ProductCode" = '123' |  比较   |     否     |
      |   Nvl    |     NVL( a."xx",0 )     | 空,默认 |     是     |
      |   Case   |          CASE           |   case   |     是     |
      |   When   |          WHEN           |   when   |     是     |
      |   Then   |          THEN       |   then   |     是     |
      |   Else   |          ELSE           |   else   |     是     |
      |   End    |           END           |   end    |     是     |
    
  - PostgreSql
  
    - 类型转换
      |  方法名称  |      解析示例值       |      说明      | 自定义函数 |
      | :--------: | :-------------------: | :------------: | :--------: |
      |  ToString  | a."xx"::VARCHAR(255)  |  转换 VARCHAR  |     否     |
      | ToDateTime |   a."xx"::TIMESTAMP   | 转换 TIMESTAMP |     否     |
      | ToDecimal  | a."xx"::DECIMAL(10,6) |  转换 DECIMAL  |     否     |
      |  ToDouble  | a."xx"::NUMERIC(10,6) |  转换 NUMERIC  |     否     |
      |  ToSingle  |     a."xx"::REAL      |   转换 REAL    |     否     |
      |  ToInt32   |    a."xx"::INTEGER    |    转换 INT    |     否     |
      |  ToInt64   |    a."xx"::BIGINT     |  转换 BIGINT   |     否     |
      | ToBoolean  |    a."xx"::BOOLEAN    |  转换 BOOLEAN  |     否     |
      |   ToChar   |    a."xx"::CHAR(2)    |   转换 CHAR    |     否     |
      
    - 聚合函数
      | 方法名称 |   解析示例值    |  说明  | 自定义函数 |
      | :------: | :-------------: | :----: | :--------: |
      |   Max    |  MAX( a."xx" )  | 最大值 |     是     |
      |   Min    |  MIN( a."xx" )  | 最小值 |     是     |
      |  Count   | COUNT( a."xx" ) |  计数  |     是     |
      |   Sum    |  SUM( a."xx" )  |  合计  |     是     |
      |   Avg    |  AVG( a."xx" )  |  平均  |     是     |
  
    - 数学函数
      | 方法名称 |     解析示例值     |   说明   | 自定义函数 |
      | :------: | :----------------: | :------: | :--------: |
      |   Abs    |   ABS( a."xx" )    |  绝对值  |     是     |
      |  Round   | ROUND( a."xx" ,2 ) | 四舍五入 |     是     |
  
    - 字符串函数
      |  方法名称  |         解析示例值          |            说明            | 自定义函数 |
      | :--------: | :-------------------------: | :------------------------: | :--------: |
      | StartsWith |   LIKE CONCAT( '%','xx' )   |           左模糊           |     否     |
      |  EndsWith  |   LIKE CONCAT( 'xx','%' )   |           右模糊           |     否     |
      |  Contains  | LIKE CONCAT( '%','xx','%' ) |           全模糊           |     否     |
      | SubString  | SUBSTRING(  'xxxxxx' ,1,3 ) |            截取            |     否     |
      |  Replace   |  REPLACE( 'xxx','x','y' )   |            替换            |     否     |
      |   Length   |       LENGTH( 'xxx' )       |            长度            |     是     |
      |    Trim    |       TRIM( ' xx ' )        |          修剪空格          |     否     |
      | TrimStart  |       LTRIM( ' xx ' )       |        修剪起始空格        |     否     |
      |  TrimEnd   |       RTRIM( ' xx ' )       |        修剪末尾空格        |     否     |
      |  ToUpper   |        UPPER( 'xx' )        |            大写            |     否     |
      |  ToLower   |        LOWER( 'xx' )        |            小写            |     否     |
      |   Concat   |   CONCAT(a."xx1",a."xx2")   |         字符串拼接         |     否     |
      | Operation  |   ”CreateTime“ >=  @Now_1   | 日期、数值、字符串范围比较 |     是     |
      
    - 日期函数
      |    方法名称     |             解析示例值              |   说明   | 自定义函数 |
      | :-------------: | :---------------------------------: | :------: | :--------: |
      |    AddYears     |     a."xx" + INTERVAL '1 YEAR'      | 添加年份 |     否     |
      |    AddMonths    |     a."xx" + INTERVAL '1 MONTH'     | 添加月份 |     否     |
      |     AddDays     |      a."xx" + INTERVAL '1 DAY'      | 添加天数 |     否     |
      |    AddHours     |     a."xx" + INTERVAL '1 HOUR'      |  添加时  |     否     |
      |   AddMinutes    |    a."xx" + INTERVAL '1 MINUTE'     |  添加分  |     否     |
      |   AddSeconds    |    a."xx" + INTERVAL '1 SECOND'     |  添加秒  |     否     |
      | AddMilliseconds | a."xx" + INTERVAL '1 MINUTE_SECOND' | 添加毫秒 |     否     |
      |      Year       |           YEAR( a."xx" )            | 获取年份 |     是     |
      |      Month      |           MONTH( a."xx" )           | 获取月份 |     是     |
      |       Day       |            DAY( a."xx" )            | 获取天数 |     是     |
  
    - 查询函数
      | 方法名称 |             解析示例值             |    说明    | 自定义函数 |
      | :------: | :--------------------------------: | :--------: | :--------: |
      |    In    |   IN  ( a."xx" ,'x1','x2','x3' )   |   In查询   |     是     |
      |  NotIn   | NOT IN  ( a."xx" ,'x1','x2','x3' ) | Not In查询 |     是     |
  
    - 其它函数
      | 方法名称 |       解析示例值        | 说明 | 自定义函数 |
      | :------: | :---------------------: | :--: | :--------: |
      |  Equals  | p.”ProductCode“ = '123' | 比较 |     否     |
      |   Case   |          CASE           |   case   |     是     |
      |   When   |          WHEN           |   when   |     是     |
      |   Then   |          THEN       |   then   |     是     |
      |   Else   |          ELSE           |   else   |     是     |
      |   End    |           END           |   end    |     是     |
  
  - Sqlite
  
    - 类型转换
    
      |  方法名称  |           解析示例值           |     说明      | 自定义函数 |
      | :--------: | :----------------------------: | :-----------: | :--------: |
      |  ToString  |     CAST(a.[xx] AS TEXT )      |   转换 TEXT   |     否     |
      | ToDateTime |       DATETIME( a.[xx] )       | 转换 DateTime |     否     |
      | ToDecimal  | CAST(a.[xx] AS DECIMAL(10,6) ) | 转换 DECIMAL  |     否     |
      |  ToDouble  | CAST(a.[xx] AS NUMERIC(10,6) ) | 转换 NUMERIC  |     否     |
      |  ToSingle  |     CAST(a.[xx] AS FLOAT )     |  转换 FLOAT   |     否     |
      |  ToInt32   |    CAST(a.[xx] AS INTEGER )    | 转换 INTEGER  |     否     |
      |  ToInt64   |    CAST(a.[xx] AS BIGINT )     |  转换 BIGINT  |     否     |
      | ToBoolean  |    CAST(a.[xx] AS CHAR(1) )    |   转换 CHAR   |     否     |
      |   ToChar   |    CAST(a.[xx] AS CHAR(2) )    |   转换 CHAR   |     否     |
      
    - 聚合函数
      | 方法名称 |   解析示例值    |  说明  | 自定义函数 |
      | :------: | :-------------: | :----: | :--------: |
      |   Max    |  MAX( a.[xx] )  | 最大值 |     是     |
      |   Min    |  MIN( a.[xx] )  | 最小值 |     是     |
      |  Count   | COUNT( a.[xx] ) |  计数  |     是     |
      |   Sum    |  SUM( a.[xx] )  |  合计  |     是     |
      |   Avg    |  AVG( a.[xx] )  |  平均  |     是     |
    
    - 数学函数
      | 方法名称 |     解析示例值     |   说明   | 自定义函数 |
      | :------: | :----------------: | :------: | :--------: |
      |   Abs    |   ABS( a.[xx] )    |  绝对值  |     是     |
      |  Round   | ROUND( a.[xx] ,2 ) | 四舍五入 |     是     |
    
    - 字符串函数
      |  方法名称  |         解析示例值          |            说明            | 自定义函数 |
      | :--------: | :-------------------------: | :------------------------: | :--------: |
      | StartsWith |      LIKE 'xx'\|\|'%'       |           左模糊           |     否     |
      |  EndsWith  |      LIKE '%'\|\|'xx'       |                            |     否     |
      |  Contains  |   LIKE '%'\|\|'xx'\|\|'%'   |           全模糊           |     否     |
      | SubString  | SUBSTRING(  'xxxxxx' ,1,3 ) |            截取            |     否     |
      |  Replace   |  REPLACE( 'xxx','x','y' )   |            替换            |     否     |
      |   Length   |       LENGTH( 'xxx' )       |            长度            |     是     |
      |    Trim    |       TRIM( ' xx ' )        |          修剪空格          |     否     |
      | TrimStart  |       LTRIM( ' xx ' )       |        修剪起始空格        |     否     |
      |  TrimEnd   |       RTRIM( ' xx ' )       |        修剪末尾空格        |     否     |
      |  ToUpper   |        UPPER( 'xx' )        |            大写            |     否     |
      |  ToLower   |        LOWER( 'xx' )        |            小写            |     否     |
      | Operation  |  [CreateTime]  >=  @Now_1   | 日期、数值、字符串范围比较 |     是     |
      
    - 日期函数
      |    方法名称     |          解析示例值           |   说明   | 自定义函数 |
      | :-------------: | :---------------------------: | :------: | :--------: |
      |    AddYears     |  DATETIME( a.[xx],'1 YEAR' )  | 添加年份 |     否     |
      |    AddMonths    | DATETIME( a.[xx],'1 MONTH' )  | 添加月份 |     否     |
      |     AddDays     |  DATETIME( a.[xx],'1 DAY' )   | 添加天数 |     否     |
      |    AddHours     |  DATETIME( a.[xx],'1 HOUR' )  |  添加时  |     否     |
      |   AddMinutes    | DATETIME( a.[xx],'1 MINUTE' ) |  添加分  |     否     |
      |   AddSeconds    | DATETIME( a.[xx],'1 SECOND' ) |  添加秒  |     否     |
      | AddMilliseconds |  DATETIME( a.[xx],'1 YEAR' )  | 添加毫秒 |     否     |
      |      Year       |   STRFTIME( '%Y', a.[xx] )    | 获取年份 |     是     |
      |      Month      |   STRFTIME( '%m', a.[xx] )    | 获取月份 |     是     |
      |       Day       |   STRFTIME( '%j', a.[xx] )    | 获取天数 |     是     |
    
    - 其它函数
      | 方法名称 |       解析示例值        | 说明 | 自定义函数 |
      | :------: | :---------------------: | :--: | :--------: |
      |  Equals  | p.”ProductCode“ = '123' | 比较 |     否     |
      |   Case   |          CASE           |   case   |     是     |
      |   When   |          WHEN          |   when   |     是     |
      |   Then   |          THEN          |   then   |     是     |
      |   Else   |          ELSE           |   else   |     是     |
      |   End    |           END           |   end    |     是     |
      
  
- 添加自定义函数解析

  ```C#
                  //注意:只能扩展未实现的方法名称 不能覆盖原有的实现
                  Models.DbType.MySQL.AddSqlFunc("方法名称", (visit, method, sqlStack) =>
                  {
                      //解析逻辑
                  });
  ```


##### 九、数据库日志

```C#
				        db.Aop.DbLog = (sql, dp) =>
                {
                    Console.WriteLine($"执行Sql:{sql}");
                    if (dp != null)
                    {
                        foreach (var item in dp)
                        {
                            Console.WriteLine($"参数名称:{item.ParameterName} 参数值:{item.ParameterValue}");
                        }
                    }
                };
```

##### 十、事务

- 普通事务

  ```c#
                try
                {
                    db.Ado.BeginTran();//开启事务

                    // 执行 CRUD

                    db.Ado.CommitTran();//提交事务
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    db.Ado.RollbackTran();//回滚事务
                }
  ```

- 更大范围的事务

  ```c#
                  try
                  {
                      db.BeginTran();//开启事务
  
                    	// 执行 CRUD
  
                      db.CommitTran();//提交事务
                  }
                  catch (Exception ex)
                  {
                      db.RollbackTran();//回滚事务
                      Console.WriteLine(ex.Message);
                  }
  ```

#####  十一、多租户

- 改变数据库

  ```c#
                  //数据库配置可从Json配置文件加载
  				IDbContext db = new DbContext(new List<DbOptions>() {
                  new DbOptions()
                  {
                      DbId = "1",
                      DbType = Models.DbType.SQLServer,
                      ConnectionStrings = "server=localhost;database=Test;user=sa;pwd=123456789;min pool size=0;max pool size=100;connect timeout=120;"
                  },
                  new DbOptions()
                  {
                      DbId = "2",
                      DbType = Models.DbType.MySQL,
                      ConnectionStrings = "server=localhost;database=Test;user=root;pwd=123456789;port=3306;min pool size=0;max pool size=100;connect timeout=120;"
                  }});
                  db.ChangeDb("2");//切换到MySQL
  ```

##### 十二、原生特性支持

```C#
    /// <summary>
    /// 产品
    /// </summary>
    [Table("ProductMain")]
    public class Product
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [Column("ProductCode")]//不标记默认取当前属性名称
        public string ProductCode { get; set; }

        /// <summary>
        /// 自定义1
        /// </summary>
        [NotMapped]
        public string Custom1 { get; set; }
        
        /// <summary>
        /// 自定义2
        /// </summary>
        [Column(TypeName="Json")]//类型标记为Json格式对象 匿名对象属性以及字典值类型 IsClass&&!type.Equals(typeof(string)) 将自动序列化成Json格式
        public Category Custom2 { get; set; }
    }
```

##### 十三、原生Ado

```C#
                // 原始起步
                // var conn = db.Ado.DbProviderFactory.CreateConnection();
                // var cmd = conn.CreateCommand();

                // 封装的方法分别以Execute和Create开头以及预处理 PrepareCommand 方法
                // 该方法可以自动帮你处理执行的预操作，主要作用是代码复用。

                // 当有非常复杂的查询 ORM不能满足需求的时候可以使用原生Ado满足业务需求

                // 构建数据集核心扩展方法 分别有 FirstBuild ListBuild DictionaryBuild DictionaryListBuild
                var data = db.Ado.ExecuteReader(CommandType.Text, "select * from product", null).ListBuild<Product>();
```

##### 十四、工作单元

- 注册数据库上下文和工作单元服务

  ```c#
  var builder = WebApplication.CreateBuilder(args);
  
  var configuration = builder.Configuration;
  
  // 添加数据库上下文服务
  builder.Services.AddFastDbContext();
  // 添加工作单元服务
  builder.Services.AddUnitOfWork();
  
  // 加载数据库配置
  builder.Services.Configure<List<DbOptions>>(configuration.GetSection("DbOptions"));
  ```

- 实际应用

  ```C#
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
                  CategoryName = "类别3"
              }).Exceute();
  
              var result2 = unitOfWork.Db.Insert(new Product()
              {
                  ProductCode = "测试工作单元",
              }).Exceute();
  
              unitOfWork.Commit();
  
              return "工作单元执行完成...";
          }
  ```


##### 十五、大数据导入

- 批复制 仅支持SqlServer Oracle MySql PostgreSql

  ```c#
                  var list = new List<Product>();
                  for (int j = 1; j <= 100000; j++)
                  {
                      list.Add(new Product()
                      {
                          CategoryId = 1,
                          ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          CreateTime = DateTime.Now,
                          Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{j}",
                          Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{j}",
                      });
                  }
                  db.Fast<Product>().BulkCopy(list);
  ```

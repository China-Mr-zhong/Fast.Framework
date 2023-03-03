using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.Framework;
using Fast.Framework.Extensions;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.Utils;
using Fast.Framework.Web.Test;
using Fast.Framework.Web.Test.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.AddFileLog();

//加载配置选项
builder.Services.Configure<List<DbOptions>>(builder.Configuration.GetSection("DbOptions"));

//注册数据库上下文
builder.Services.AddFastDbContext();

//注册工作单元
builder.Services.AddFastUnitOfWork();

//注册Http上下文
builder.Services.AddHttpContextAccessor();

//#region 注册数据访问和业务逻辑服务
//foreach (var section in builder.Configuration.GetSection("DependencyInjection").GetChildren())
//{
//    var serviceDll = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, section["ServiceDll"]));
//    var ImplementationDll = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, section["ImplementationDll"]));
//    var types = section.GetSection("Types").GetChildren();
//    foreach (var type in types)
//    {
//        var serviceType = serviceDll.GetType(type["ServiceType"]);
//        if (serviceType == null)
//        {
//            throw new Exception($"服务类型:{type["ServiceType"]} 不存在");
//        }
//        var implementationType = ImplementationDll.GetType(type["ImplementationType"]);
//        if (implementationType == null)
//        {
//            throw new Exception($"实现类型:{type["ImplementationType"]} 不存在");
//        }
//        builder.Services.AddTransient(serviceType, implementationType);
//    }
//}
//#endregion


// 添加测试服务
builder.Services.AddScoped<UnitOfWorkTestService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();


builder.Services.AddControllers(c =>
{
    //c.Filters.Add(typeof(CustomAuthorizeFilter));
}).AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    o.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    o.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());
});

builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = action =>
    {
        return new JsonResult(new
        {
            Code = ApiCodes.ArgumentError,
            Message = action.ModelState.Values.FirstOrDefault()?.Errors[0].ErrorMessage
        });
    };
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis:ConnectionStrings").Value;
    options.InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value;
});

builder.Services.AddTransient<IClientErrorFactory, ClientErrorFactory>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = Token.tokenValidationParameters;
    options.Events = new JwtBearerEvents()
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            return context.Response.WriteAsync($"{{\"Code\":{ApiCodes.TokenError},\"Message\":\"Token Error\"}}");
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.Urls.Add("http://*.*.*:5000");
}

app.UseMiddleware<ExceptionMiddleware>();

//app.UseMiddleware<BodyCacheMiddleware>();

app.UseCors(configurePolicy =>
{
    configurePolicy.AllowAnyHeader();
    configurePolicy.AllowAnyMethod();
    configurePolicy.AllowAnyOrigin();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapEntitys(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fast.Framework.Test.Models.dll"));

app.Run();
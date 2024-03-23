using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.Framework;
using Fast.Framework.Web.Test;
using Fast.Framework.Web.Test.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fast.Framework.Logging;
using Microsoft.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFileLog();

//��������ѡ��
builder.Services.Configure<List<DbOptions>>(builder.Configuration.GetSection(nameof(DbOptions)));

//ע�����ݿ�������
builder.Services.AddFastDbContext();

//ע�Ṥ����Ԫ
builder.Services.AddUnitOfWork();

////����JWT����
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

//ע��Http������
builder.Services.AddHttpContextAccessor();

//ע�����
//var injectDll = builder.Configuration.GetSection("DependencyInjection").Get<List<InjectDll>>();
//builder.Services.RegisterServices(injectDll);


// ��Ӳ��Է���
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
            Code = -1002,
            Message = action.ModelState.Values.FirstOrDefault()?.Errors[0].ErrorMessage
        });
    };
});

builder.Services.AddTransient<IClientErrorFactory, ClientErrorFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fast.Framework.Web.Test.xml"), true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();

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

//app.UseAuthentication();

//app.UseAuthorization();

app.MapControllers();

//��ʾ�첽����ʵ�建�棬�ְ汾������ʵ��ע��
//app.UseEntityCache(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fast.Framework.Test.Models.dll"));

app.Run();
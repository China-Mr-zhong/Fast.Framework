using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Fast.Framework.Utils
{

    #region 自定义转换类
    /// <summary>
    /// 日期时间转换
    /// </summary>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dateTimeFormat"></param>
        public DateTimeConverter(string dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff")
        {
            DateTimeFormat = dateTimeFormat;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader">读取</param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(reader.GetString()))
            {
                return DateTime.Now;
            }
            return Convert.ToDateTime(reader.GetString());
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer">写入</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormat));
        }
    }

    /// <summary>
    /// 日期时间可空转换
    /// </summary>
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dateTimeFormat"></param>
        public DateTimeNullableConverter(string dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff")
        {
            DateTimeFormat = dateTimeFormat;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader">读取</param>
        /// <param name="typeToConvert">转换类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(reader.GetString()))
            {
                return null;
            }
            return Convert.ToDateTime(reader.GetString());
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer">写入</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Value.ToString(DateTimeFormat));
            }
        }
    }
    #endregion

    /// <summary>
    /// Json工具类
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        private readonly static JsonSerializerOptions defaultOptions;

        /// <summary>
        /// 构造方法
        /// </summary>
        static Json()
        {
            defaultOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            defaultOptions.Converters.Add(new DateTimeConverter());
            defaultOptions.Converters.Add(new DateTimeNullableConverter());
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string Serialize<T>(T value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(value, options ?? defaultOptions);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">Json</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options ?? defaultOptions);
        }
    }
}

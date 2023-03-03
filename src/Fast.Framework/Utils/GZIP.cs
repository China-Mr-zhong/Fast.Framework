using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fast.Framework.Utils
{

    /// <summary>
    /// GZIP工具类
    /// </summary>
    public static class GZIP
    {

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns></returns>
        public static async Task<byte[]> ZIPAsync(byte[] bytes, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzip = new GZipStream(memoryStream, compressionLevel))
                {
                    await gzip.WriteAsync(bytes);
                }
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns></returns>
        public static async Task<byte[]> UZIPAsync(byte[] bytes, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzip = new GZipStream(memoryStream, compressionLevel))
                {
                    await gzip.ReadAsync(bytes);
                }
                return memoryStream.ToArray();
            }
        }
    }
}

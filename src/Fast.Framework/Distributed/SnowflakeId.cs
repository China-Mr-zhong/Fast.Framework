using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Models;
using Fast.Framework.Utils;
using Microsoft.Extensions.Options;

namespace Fast.Framework.Snowflake
{

    /// <summary>
    /// 雪花ID
    /// </summary>
    public class SnowflakeId
    {
        /// <summary>
        /// 基准时间
        /// </summary>
        private const long twepoch = 1288834974657L;

        /// <summary>
        /// 机器标识位数
        /// </summary>
        private const int workerIdBits = 5;

        /// <summary>
        /// 数据中心ID位数
        /// </summary>
        private const int dataCenterIdBits = 5;

        /// <summary>
        /// 序列号识位数
        /// </summary>
        private const int sequenceBits = 12;

        /// <summary>
        /// 机器ID最大值
        /// </summary>
        private const long maxWorkerId = -1L ^ -1L << workerIdBits;

        /// <summary>
        /// 最大数据中心ID
        /// </summary>
        private const long maxDataCenterId = -1L ^ -1L << dataCenterIdBits;

        /// <summary>
        /// 序列号ID最大值
        /// </summary>
        private const long sequenceMask = -1L ^ -1L << sequenceBits;

        /// <summary>
        /// 工作ID偏左移12位
        /// </summary>
        private const int workerIdShift = sequenceBits;

        /// <summary>
        /// 数据中心ID偏左移17位
        /// </summary>
        private const int dataCenterIdShift = sequenceBits + workerIdBits;

        /// <summary>
        /// 时间毫秒左移22位
        /// </summary>
        public const int timestampLeftShift = sequenceBits + workerIdBits + dataCenterIdBits;

        /// <summary>
        /// 最后时间戳
        /// </summary>
        private long lastTimestamp = -1L;

        /// <summary>
        /// 选项
        /// </summary>
        private readonly SnowflakeIdOptions options;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项</param>
        public SnowflakeId(IOptionsSnapshot<SnowflakeIdOptions> options) : this(options.Value)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项</param>s
        public SnowflakeId(SnowflakeIdOptions options)
        {
            if (options.WorkerId > maxWorkerId || options.WorkerId < 0)
            {
                throw new ArgumentException($"{nameof(options.WorkerId)}必须大于0,且不能大于MaxWorkerId:{maxWorkerId}.");
            }

            if (options.DataCenterId > maxDataCenterId || options.DataCenterId < 0)
            {
                throw new ArgumentException($"{nameof(options.DataCenterId)}必须大于0,且不能大于DataCenterId:{maxDataCenterId}.");
            }
            this.options = options;
        }

        /// <summary>
        /// 下一个ID
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = Timestamp.CurrentTimestampMilliseconds();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception($"时间戳必须大于上一次生成ID的时间戳,拒绝为{lastTimestamp - timestamp}毫秒生成Id.");
                }

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (lastTimestamp == timestamp)
                {
                    //sequence自增和sequenceMask相与一下去掉高位
                    options.Sequence = options.Sequence + 1 & sequenceMask;
                    //判断是否溢出也就是每毫秒内超过1024当为1024时与sequenceMask相与sequence就等于0
                    if (options.Sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    //如果和上次生成时间不同重置sequence就是下一毫秒开始sequence计数重新从0开始累加
                    //为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    options.Sequence = 0;
                }

                lastTimestamp = timestamp;
                return timestamp - twepoch << timestampLeftShift | options.DataCenterId << dataCenterIdShift | options.WorkerId << workerIdShift | options.Sequence;
            }
        }

        /// <summary>
        /// 等待下一毫秒 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        /// </summary>
        /// <param name="lastTimestamp">最后时间戳</param>
        /// <returns></returns>
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = Timestamp.CurrentTimestampMilliseconds();
            while (timestamp <= lastTimestamp)
            {
                timestamp = Timestamp.CurrentTimestampMilliseconds();
            }
            return timestamp;
        }
    }
}

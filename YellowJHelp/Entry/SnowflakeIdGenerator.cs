using System;
using System.Collections.Generic;
using System.Text;

namespace YellowJHelp.Entry
{
    /// <summary>
    /// 雪花算法ID生成器
    /// </summary>
    public class SnowflakeIdGenerator
    {
        // 时间戳基准（2020-01-01）
        private const long Epoch = 1577836800000L;

        // 各部分位数
        private const int TimestampBits = 41;
        private const int WorkerIdBits = 10;
        private const int SequenceBits = 12;

        private readonly long _workerId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;
        // 最大取值范围
        private const long MaxWorkerId = (1L << WorkerIdBits) - 1;
        private const long MaxSequence = (1L << SequenceBits) - 1;

        // 移位偏移量
        private const int TimestampShift = WorkerIdBits + SequenceBits;
        private const int WorkerIdShift = SequenceBits;

        public SnowflakeIdGenerator(long workerId)
        {
            if (workerId < 0 || workerId > MaxWorkerId)
                throw new ArgumentException($"Worker ID must be between 0 and {MaxWorkerId}");
            _workerId = workerId;
        }

        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public long NextId()
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Clock moved backwards");

            int sequence;
            if (timestamp == _lastTimestamp)
            {
                sequence = (int)(Interlocked.Increment(ref _sequence) & MaxSequence);
                if (sequence == 0)
                    timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                sequence = 0;
                Interlocked.Exchange(ref _sequence, 0);
            }

            Interlocked.Exchange(ref _lastTimestamp, timestamp);

            return (timestamp - Epoch) << TimestampShift
                   | _workerId << WorkerIdShift
                   | sequence;
        }

        private long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private long WaitNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                Thread.Sleep(1);
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }
}

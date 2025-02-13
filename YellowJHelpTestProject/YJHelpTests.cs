using Microsoft.AspNetCore.Http;
using YellowJHelp.Entry;

namespace YellowJHelp.Tests
{
    public class YJHelpTests
    {
        private readonly YJHelp _yjHelp;

        public YJHelpTests()
        {
            _yjHelp = new YJHelp();
        }

        [Fact]
        public void YAlloctionlist_ShouldReturnExpectedResult()
        {

            // Arrange
            var yAllocations = new List<YAllocationInfo>();
            var yAllocations1 = new List<YAllocationInfo>();

            for (int i = 0; i < 10000000; i++)
            {
                Random random = new Random();
                decimal randomDoublenumber = Convert.ToDecimal(random.NextDouble());
                decimal randomDouble =Convert.ToDecimal(random.NextDouble()); // 生成一个 [0.0, 1.0) 范围内的随机小数
                YAllocationInfo yAllocationInfo = new YAllocationInfo
                {
                    Number = new List<string> { "YellowJ" + randomDoublenumber },
                    Key = "key" + i,
                    Qty = randomDouble,
                    RemQty = randomDouble
                };
                yAllocations.Add(yAllocationInfo);
            }
            for (int i = 0; i < 10000000; i++)
            {
                Random random = new Random();
                decimal randomDoublenumber = Convert.ToDecimal(random.NextDouble());
                decimal randomDouble = Convert.ToDecimal(random.NextDouble()); // 生成一个 [0.0, 1.0) 范围内的随机小数
                YAllocationInfo yAllocationInfo = new YAllocationInfo
                {
                    Number = new List<string> { "YellowJ" + randomDoublenumber },
                    Key = "key" + i,
                    Qty = randomDouble
                };
                yAllocations1.Add(yAllocationInfo);
            }

            // Act
            var result = _yjHelp.YAlloctionlist(yAllocations, yAllocations1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void YAlloctionlistThred_ShouldReturnExpectedResult()
        {
            // Arrange
            var yAllocations = new List<YAllocationInfo>();
            var yAllocations1 = new List<YAllocationInfo>();

            for (int i = 0; i < 10000000; i++)
            {
                Random random = new Random();
                decimal randomDoublenumber = Convert.ToDecimal(random.NextDouble());
                decimal randomDouble = Convert.ToDecimal(random.NextDouble()); // 生成一个 [0.0, 1.0) 范围内的随机小数
                YAllocationInfo yAllocationInfo = new YAllocationInfo
                {
                    Number =new List<string> { "YellowJ" + randomDoublenumber },
                    Key = "key" + i,
                    Qty = randomDouble,
                    RemQty = randomDouble
                };
                yAllocations.Add(yAllocationInfo);
            }
            for (int i = 0; i < 10000000; i++)
            {
                Random random = new Random();
                decimal randomDoublenumber = Convert.ToDecimal(random.NextDouble());
                decimal randomDouble = Convert.ToDecimal(random.NextDouble()); // 生成一个 [0.0, 1.0) 范围内的随机小数
                YAllocationInfo yAllocationInfo = new YAllocationInfo
                {
                    Number = new List<string> { "YellowJ" + randomDoublenumber },
                    Key = "key" + i,
                    Qty = randomDouble
                };
                yAllocations1.Add(yAllocationInfo);
            }

            // Act
            var result = _yjHelp.YAlloctionlistThred(yAllocations, yAllocations1);

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void YAlloctionlistThred_Copy()
        {
            // Arrange
            List<YAllocationInfo> yAllocations = new List<YAllocationInfo>();
            foreach (var i in Enumerable.Range(0, 10000000))
            {
                Random random = new Random();
                decimal randomDoublenumber = Convert.ToDecimal(random.NextDouble());
                decimal randomDouble = Convert.ToDecimal(random.NextDouble()); // 生成一个 [0.0, 1.0) 范围内的随机小数
                YAllocationInfo yAllocationInfo = new YAllocationInfo
                {
                    Number = new List<string> { "YellowJ" + randomDoublenumber },
                    Key = "key" + i,
                    Qty = randomDouble,
                    RemQty = randomDouble
                };
                yAllocations.Add(yAllocationInfo);
            }
            
            YJHelpT<List<YAllocationInfo>> yJHelpT = new YJHelpT<List<YAllocationInfo>>();
            var result = yJHelpT.Copy(yAllocations);
            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void YAlloctionlistThred_Nextid()
        {
            // Arrange
            var result = _yjHelp.NextId(100);
            // Assert
            Assert.NotNull(result);
        }
    }
}

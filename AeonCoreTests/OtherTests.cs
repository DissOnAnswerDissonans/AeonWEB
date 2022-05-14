using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Core.Tests;

public class OtherTests
{
	[Theory]
	[InlineData(100.0, 100)]
	[InlineData(12.3, 12)]
	[InlineData(12.5, 12)]
	[InlineData(12.7, 12)]
	public void RoundTest(decimal input, int output) => input.TRound().Should().Be(output);

	[Theory]
	[InlineData(1.1, 0, 1)]
	[InlineData(1.1, 1, 1.1)]
	[InlineData(1.1, 2, 1.21)]
	[InlineData(1.25, -3, .512)]
	[InlineData(2, 31, (long)int.MaxValue + 1)]
	public void DecPowerTest(decimal input, int pow, decimal output) => input.Power(pow).Should().Be(output);
}

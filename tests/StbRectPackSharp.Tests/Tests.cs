using NUnit.Framework;
using StbRectPackSharp;

namespace StbImageSharp.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void SimplePacking()
		{
			// Pack 3 rects
			var packer = new Packer();

			packer.PackRect(10, 10, "a");
			packer.PackRect(15, 10, "b");
			packer.PackRect(10, 20, "c");

			Assert.AreEqual(packer.PackRectangles.Count, 3);

			var rect = packer.PackRectangles[0];
			Assert.AreEqual(rect.Rectangle.Width, 10);
			Assert.AreEqual(rect.Rectangle.Height, 10);
			Assert.AreEqual(rect.Data, "a");

			rect = packer.PackRectangles[1];
			Assert.AreEqual(rect.Rectangle.Width, 15);
			Assert.AreEqual(rect.Rectangle.Height, 10);
			Assert.AreEqual(rect.Data, "b");

			rect = packer.PackRectangles[2];
			Assert.AreEqual(rect.Rectangle.Width, 10);
			Assert.AreEqual(rect.Rectangle.Height, 20);
			Assert.AreEqual(rect.Data, "c");
		}

		/// <summary>
		/// This test forces pack buffer to grow a few times
		/// </summary>
		[Test]
		public void ForceGrow()
		{
			var packer = new Packer(128, 128);

			packer.PackRect(10, 10, "a");
			packer.PackRect(15, 10, "b");
			packer.PackRect(800, 600, "c");

			Assert.AreEqual(packer.PackRectangles.Count, 3);

			var rect = packer.PackRectangles[0];
			Assert.AreEqual(rect.Rectangle.Width, 10);
			Assert.AreEqual(rect.Rectangle.Height, 10);
			Assert.AreEqual(rect.Data, "a");

			rect = packer.PackRectangles[1];
			Assert.AreEqual(rect.Rectangle.Width, 15);
			Assert.AreEqual(rect.Rectangle.Height, 10);
			Assert.AreEqual(rect.Data, "b");

			rect = packer.PackRectangles[2];
			Assert.AreEqual(rect.Rectangle.Width, 800);
			Assert.AreEqual(rect.Rectangle.Height, 600);
			Assert.AreEqual(rect.Data, "c");
		}
	}
}

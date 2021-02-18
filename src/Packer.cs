using System;
using System.Collections.Generic;
using System.Drawing;
using static StbRectPackSharp.StbRectPack;

namespace StbRectPackSharp
{
	public class PackerRectangle
	{
		public Rectangle Rectangle { get; private set; }

		public object Data { get; private set; }

		public PackerRectangle(Rectangle rect, object data)
		{
			Rectangle = rect;
			Data = data;
		}
	}

	/// <summary>
	/// Simple Packer class that doubles size of the atlas if the place runs out
	/// </summary>
	public unsafe class Packer: IDisposable
	{
		private stbrp_context _context;
		private List<PackerRectangle> _rectangles = new List<PackerRectangle>();

		public int Width => _context.width;
		public int Height => _context.height;

		public List<PackerRectangle> PackRectangles => _rectangles;


		public Packer(int initialWidth = 256, int initialHeight = 256)
		{
			InitContext(initialWidth, initialHeight);
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		private void InitContext(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			// Dispose old context
			_context.Dispose();

			// Create new one
			var num_nodes = width;
			stbrp_context newContext = new stbrp_context(num_nodes);

			stbrp_init_target(&newContext, width, height, newContext.all_nodes, num_nodes);

			_context = newContext;
			_rectangles = new List<PackerRectangle>();
		}

		public PackerRectangle PackRect(int width, int height, object data)
		{
			var rect = new stbrp_rect
			{
				id = _rectangles.Count,
				w = width,
				h = height
			};

			int result;
			fixed (stbrp_context* contextPtr = &_context)
			{
				result = stbrp_pack_rects(contextPtr, &rect, 1);
			}

			if (result == 0)
			{
				var oldRectangles = _rectangles;

				// Can't fit
				// Create new context two times bigger than existing
				InitContext(_context.width * 2, _context.height * 2);

				// Place old rectangles
				foreach(var r in oldRectangles)
				{
					PackRect(r.Rectangle.Width, r.Rectangle.Height, r.Data);
				}
			}

			var packRectangle = new PackerRectangle(new Rectangle(rect.x, rect.y, rect.w, rect.h), data);

			_rectangles.Add(packRectangle);

			return packRectangle;
		}
	}
}

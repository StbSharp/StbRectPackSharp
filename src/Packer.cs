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
	public unsafe class Packer
	{
		private stbrp_context _context;
		private int _padding;
		private List<PackerRectangle> _rectangles = new List<PackerRectangle>();

		public int Width => _context.width;
		public int Height => _context.height;
		public int Padding => _padding;

		public List<PackerRectangle> PackRectangles => _rectangles;


		public Packer(int initialWidth = 256, int initialHeight = 256, int padding = 0)
		{
			InitContext(initialWidth, initialHeight, padding);

		}

		private void InitContext(int width, int height, int padding)
		{
			var num_nodes = width - padding;
			var nodes = new stbrp_node[num_nodes];

			stbrp_context newContext = new stbrp_context();

			// Allocate extras
			newContext.extra = (stbrp_node*)CRuntime.malloc(sizeof(stbrp_node) * 2);
			fixed (stbrp_node* nodesPtr = nodes)
				stbrp_init_target(&newContext, width, height, nodesPtr, num_nodes);

			_context = newContext;
			_padding = padding;
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
				InitContext(_context.width * 2, _context.height * 2, _padding);

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

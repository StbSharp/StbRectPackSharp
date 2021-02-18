using System;

namespace StbRectPackSharp
{
	public static unsafe partial class StbRectPack
	{
		public struct stbrp_context: IDisposable
		{
			public int width;
			public int height;
			public int align;
			public int init_mode;
			public int heuristic;
			public int num_nodes;
			public stbrp_node* active_head;
			public stbrp_node* free_head;
			public stbrp_node* extra;

			public void Dispose()
			{
				if (extra != null)
				{
					CRuntime.free(extra);
					extra = null;
				}
			}
		}
	}
}

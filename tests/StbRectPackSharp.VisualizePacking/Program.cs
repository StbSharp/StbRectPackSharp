using System;

namespace StbImageSharp.Samples.MonoGame
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				using (var game = new VisualizerGame())
					game.Run();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
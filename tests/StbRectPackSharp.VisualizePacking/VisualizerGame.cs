using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StbRectPackSharp;

namespace StbImageSharp.Samples.MonoGame
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class VisualizerGame : Game
	{
		private const int RectangleAddDelayInMs = 100;
		private const int MaximumRectangleSize = 50;
		private const int BorderThickness = 2;

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D _white;
		private readonly Random _random = new Random();
		private Packer _packer = new Packer(128, 128);
		private DateTime? _addedLast;
		private FontSystem _fontSystem;

		public VisualizerGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1400,
				PreferredBackBufferHeight = 960
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}
		
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// White texture
			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			// Font
			var data = File.ReadAllBytes("Fonts/DroidSans.ttf");
			_fontSystem = FontSystemFactory.Create(GraphicsDevice);
			_fontSystem.AddFont(data);
		}

		private void GrowContext()
		{
			var oldPacker = _packer;

			// Create new packer two times bigger than existing
			_packer = new Packer(oldPacker.Width * 2, oldPacker.Height * 2);

			// Place old rectangles
			foreach (var r in oldPacker.PackRectangles)
			{
				_packer.PackRect(r.Width, r.Height, r.Data);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (_addedLast == null || (DateTime.Now - _addedLast.Value).TotalMilliseconds >= RectangleAddDelayInMs)
			{
				var color = new Color(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));
				var width = _random.Next(1, MaximumRectangleSize);
				var height = _random.Next(1, MaximumRectangleSize);

				var result = _packer.PackRect(width, height, color);
				while(result == null)
				{
					// Grow our atlas size until the new rectangle can be packed
					GrowContext();
					result = _packer.PackRect(width, height, color);
				}

				_addedLast = DateTime.Now;
			}
		}

		/// <summary>
		/// Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="texture"></param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="thickness">The thickness of the lines</param>
		public static void DrawRectangle(SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color color, float thickness = 1f)
		{
			var t = (int)thickness;

			// Top
			spriteBatch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, t), null, color);

			// Bottom
			spriteBatch.Draw(texture, new Rectangle(rectangle.X, rectangle.Bottom - t, rectangle.Width, t), null, color);

			// Left
			spriteBatch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, t, rectangle.Height), null, color);

			// Right
			spriteBatch.Draw(texture, new Rectangle(rectangle.Right - t, rectangle.Y, t, rectangle.Height), null, color);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin();

			DrawRectangle(_spriteBatch, _white, new Rectangle(0, 0, _packer.Width + BorderThickness * 2, _packer.Height + +BorderThickness * 2), Color.White, BorderThickness);


			foreach(var rect in _packer.PackRectangles)
			{
				_spriteBatch.Draw(_white, new Rectangle(rect.X + BorderThickness, rect.Y + BorderThickness, rect.Width, rect.Height), (Color)rect.Data);
			}

			var font = _fontSystem.GetFont(32);

			font.DrawText(_spriteBatch, "Rectangles: " + _packer.PackRectangles.Count, new Vector2(BorderThickness, BorderThickness), Color.White);

			var text = _packer.Width + "x" + _packer.Height;
			var size = font.MeasureString(text);

			font.DrawText(_spriteBatch, text, new Vector2((_packer.Width - size.X) / 2 + BorderThickness, (_packer.Height - size.Y) / 2 + BorderThickness), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
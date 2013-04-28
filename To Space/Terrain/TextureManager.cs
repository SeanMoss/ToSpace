using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using To_Space.Terrain;
using To_Space.Terrain.Blocks;
using To_Space.Terrain.Backgrounds;

namespace To_Space.Terrain
{
	public static class TextureManager
	{
		public static readonly int TEXTURE_MAP_DIM = 16;

		private static bool _init = false;

		private static Dictionary<string, Texture2D> _textureList = new Dictionary<string, Texture2D>();

		private static GraphicsDevice _device;

		public static void InitializeTextures(ContentManager manager, GraphicsDevice device)
		{
			if (!_init)
			{
				_device = device;

				TSDebug.WriteLine("TEXTURE: Loading block textures.");

				List<string> toLoad = new List<string>();
				List<string> files = new List<string>();

				foreach (Block b in Block.BlockList)
				{
					if (b.TextureIndex >= 0)
					{
						string load = b.GetTexturePath() + b.GetTextureFile();

						if (!toLoad.Contains(load))
						{
							toLoad.Add(load);
							files.Add(b.GetTextureFile());
						} 
					}
				}

				foreach (Background b in Background.BackgroundList)
				{
					if (b.TextureIndex >= 0)
					{
						string load = b.GetTexturePath() + b.GetTextureFile();

						if (!toLoad.Contains(load))
						{
							toLoad.Add(load);
							files.Add(b.GetTextureFile());
						}
					}
				}

				for (int i = 0; i < toLoad.Count; i++)
				{
					if (!_textureList.ContainsKey(files[i]))
					{
						Texture2D tex = manager.Load<Texture2D>(@toLoad[i]);

						if (!TextureManager.HasMultipleOfTwoDimention(tex))
						{
							throw new Exception("Could not load the texture file " + files[i] + " because of improper dimensions." + 
								" Please check that the texture file's dimentions are a multiple of two, and is square.");
						}

						_textureList.Add(files[i], manager.Load<Texture2D>(@toLoad[i]));
					}
				}

				TSDebug.WriteLine("TEXTURE: Loaded block textures.");

				_init = true;
			}
		}

		public static Texture2D GetTexture(string name)
		{
			if (!_init)
				throw new Exception("The texture mananger has not been initialized.");

			if (!_textureList.ContainsKey(name))
				throw new Exception("There is no block texture map with the name \"" + name + "\" loaded into memory.");

			return _textureList[name];
		}

		public static Texture2D GetTextureAtIndex(int index, Texture2D tex)
		{
			if (index >= 0)
			{
				Rectangle source = GetSourceForIndex(index, tex);
				Texture2D texture = new Texture2D(_device, source.Width, source.Height);

				Color[] colors = new Color[tex.Width * tex.Height];
				tex.GetData<Color>(colors);
				Color[] colorData = new Color[source.Width * source.Height];

				for (int x = 0; x < source.Width; x++)
				{
					for (int y = 0; y < source.Height; y++)
					{
						colorData[x + y * source.Width] = colors[x + source.X + (y + source.Y) * tex.Width];
					}
				}

				texture.SetData<Color>(colorData);
				return texture; 
			}

			return null;
		}

		public static Texture2D GetTextureAtIndex(int index, string name)
		{
			if (index >= 0)
			{
				Texture2D tex = GetTexture(name);
				Rectangle source = GetSourceForIndex(index, name);
				Texture2D texture = new Texture2D(_device, source.Width, source.Height);

				Color[] colors = new Color[tex.Width * tex.Height];
				tex.GetData<Color>(colors);
				Color[] colorData = new Color[source.Width * source.Height];

				for (int x = 0; x < source.Width; x++)
				{
					for (int y = 0; y < source.Height; y++)
					{
						colorData[x + y * source.Width] = colors[x + source.X + (y + source.Y) * tex.Width];
					}
				}

				texture.SetData<Color>(colorData);
				return texture; 
			}

			return null;
		}

		public static Rectangle GetSourceForIndex(int index, Texture2D tex)
		{
			int dim = tex.Width / TEXTURE_MAP_DIM;

			int startx = index % TEXTURE_MAP_DIM;
			int starty = index / TEXTURE_MAP_DIM;

			return new Rectangle(startx * dim, starty * dim, dim, dim);
		}

		public static Rectangle GetSourceForIndex(int index, string name)
		{
			if(!_init)
				throw new Exception("The texture mananger has not been initialized.");

			if (!_textureList.ContainsKey(name))
				throw new Exception("There is no block texture map with the name \"" + name + "\" loaded into memory.");

			int dim =  _textureList[name].Width / TEXTURE_MAP_DIM;

			int startx = index % TEXTURE_MAP_DIM;
			int starty = index / TEXTURE_MAP_DIM;

			return new Rectangle(startx, starty, dim, dim);
		}

		public static bool HasMultipleOfTwoDimention(Texture2D tex)
		{
			int width = tex.Width;
			int height = tex.Height;

			if (width != height)
			{
				return false;
			}

			return (width != 0) && (height != 0) && ((width & (width - 1)) == 0) && ((height & (height - 1)) == 0);
		}
	}
}
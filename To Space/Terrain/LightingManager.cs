using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace To_Space.Terrain
{
	//Lighting one region only takes 4 kilobytes of memory
	public class LightingManager
	{
		public static readonly float SUN_FALLOFF = .1f;
		public static readonly Color SUN_COLOR = new Color(255, 255, 255);

		//Holds the red light for the area
		private byte[,] _rMap;
		public byte[,] RMap { get { return _rMap; } }

		//Holds the green light for the area
		private byte[,] _gMap;
		public byte[,] GMap { get { return _gMap; } }

		//Holds the blue light for the area
		private byte[,] _bMap;
		public byte[,] BMap { get { return _bMap; } }

		//Depth of the first solid blocks
		private short[] _lightDepth;
		public short[] LightDepth { get { return _lightDepth; } }

		//The region being tracked
		private Region _region;
		public Region Region { get { return _region; } }

		public LightingManager(Region region)
		{
			_region = region;

			_rMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
			_gMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
			_bMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
		}

		public void Unload()
		{
			_rMap = null;
			_gMap = null;
			_bMap = null;
		}

		public void Reinitialize()
		{
			_rMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
			_gMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
			_bMap = new byte[Region.REGION_DIM, Region.REGION_DIM];
		}
		#region Light Distribution
		
		#endregion

		#region Setters
		public void SetRedLightAt(int x, int y, byte b)
		{
			if (InRange(x, y))
			{
				_rMap[x, y] = b;
			}
		}

		public void SetGreenLightAt(int x, int y, byte b)
		{
			if (InRange(x, y))
			{
				_gMap[x, y] = b;
			}
		}

		public void SetBlueLightAt(int x, int y, byte b)
		{
			if (InRange(x, y))
			{
				_bMap[x, y] = b;
			}
		}
		#endregion

		#region Accessors
		public byte GetRedLightAt(int x, int y)
		{
			if (InRange(x, y))
			{
				return _rMap[x, y]; 
			}

			return 0;
		}

		public byte GetGreenLightAt(int x, int y)
		{
			if (InRange(x, y))
			{
				return _gMap[x, y];
			}

			return 0;
		}

		public byte GetBlueLightAt(int x, int y)
		{
			if (InRange(x, y))
			{
				return _bMap[x, y];
			}

			return 0;
		}

		public Color GetLightAt(int x, int y)
		{
			float r = LightingManager.ColorByteToFloat(_rMap[x, y]); //Red light
			float g = LightingManager.ColorByteToFloat(_gMap[x, y]); //Green light
			float b = LightingManager.ColorByteToFloat(_bMap[x, y]); //Blue light

			return new Color(r, g, b, 1);
		}
		#endregion

		#region Static Methods
		//If the provided coordinates is in range
		public static bool InRange(int x, int y)
		{
			return !(x < 0 || y < 0 || x >= Region.REGION_DIM || y >= Region.REGION_DIM);
		}
		//Converts a byte to a color float
		private static float ColorByteToFloat(byte color)
		{
			return (float)color / 255f;
		}
		//Dedused the light by a set amount
		private static byte AttenuateLight(byte light, byte amt)
		{
			return (byte)(light * (1 - amt));
		}
		#endregion
	}
}
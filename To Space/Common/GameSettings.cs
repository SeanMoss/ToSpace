using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace To_Space.Common
{
	public static class GameSettings
	{
		//If the settings have been initialized
		private static bool _init = false;
		public static bool Initialized { get { return _init; } }

		//The main path to the non-program files for this game
		public static readonly string GamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ToSpace\";

		//The name of the settings file
		private static readonly string _settingsFileName = "GameSettings.tss";
		private static readonly string _finalPath = GamePath + _settingsFileName;

		public static void Initialize()
		{
			if (!_init)
			{
				ResetToDefaults();

				Load();
				Save();

				_init = true;
			}

			CheckDirectories();
		}

		#region File Access
		public static void Load()
		{
			try
			{
				FileStream stream = File.OpenRead(_finalPath);
				BinaryReader reader = new BinaryReader(stream);

				TSDebug.WriteLine("SETTINGS: Loading from settings file...");

				FullScreen = reader.ReadBoolean();
				VSync = reader.ReadBoolean();
				ScreenWidth = reader.ReadInt32();
				ScreenHeight = reader.ReadInt32();

				reader.Close();
				stream.Close();
			}
			catch (IOException)
			{
				TSDebug.WriteLine("SETTINGS: Could not load settings from the settings file. Reverting to defaults.");
				ResetToDefaults();
				Save();
			}
		}

		public static void Save()
		{
			try
			{
				//Flush the existing data
				FileStream clearStream = File.Open(_finalPath, FileMode.Create);
				clearStream.SetLength(0);
				clearStream.Flush();
				clearStream.Close();

				FileStream stream = File.Open(_finalPath, FileMode.OpenOrCreate);
				BinaryWriter writer = new BinaryWriter(stream);

				TSDebug.WriteLine("SETTINGS: Saving to settings file...");

				writer.Write(FullScreen);
				writer.Write(VSync);
				writer.Write(ScreenWidth);
				writer.Write(ScreenHeight);

				writer.Flush();
				writer.Close();
				stream.Close();
			}
			catch (Exception)
			{
				TSDebug.WriteLine("SETTINGS: Could not save settings.");
			}
		}
		#endregion

		public static void ResetToDefaults()
		{
			TSDebug.WriteLine("SETTINGS: Resetting settings to defaults.");

			FullScreen = _defaultFullScreen;
			VSync = _defaultVSync;
			ScreenWidth = _defaultScreenWidth;
			ScreenHeight = _defualtScreenHeight;
		}

		private static void CheckDirectories()
		{
			if (!Directory.Exists(GamePath + @"Saves"))
			{
				TSDebug.WriteLine("SETTINGS: Creating save path.");
				Directory.CreateDirectory(GamePath + @"Saves");
			}
		}

		#region Current Settings
		//If the game is full screen
		public static bool FullScreen;
		
		//If vsync is enabled
		public static bool VSync;

		//The width of the screen
		public static int ScreenWidth;

		//The height of the screen
		public static int ScreenHeight;
		#endregion

		#region Defaults
		//full screen
		private static readonly bool _defaultFullScreen = false;

		//vsync
		private static readonly bool _defaultVSync = false;

		//screen width
		private static readonly int _defaultScreenWidth = 800;

		//screen height
		private static readonly int _defualtScreenHeight = 600;
		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using To_Space.Terrain;

namespace To_Space.Entities
{
	//Represents another player connected over the server
	public class ServerPlayer : Entity
	{
		public ServerPlayer(Vector2 pos, Texture2D tex, World w)
			: base(pos, tex, w)
		{

		}

		//All updating done by server
		public override void Update() { }
	}
}
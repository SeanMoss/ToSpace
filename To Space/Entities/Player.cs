using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using To_Space.Terrain;

namespace To_Space.Entities
{
	public class Player : Entity
	{
		private Texture2D _crouch;
		private Texture2D _oldTexture;

		//If the player is crouching
		private bool _crouching = false;
		public bool Crouching { get { return _crouching; } }

		//Other speeds
		public float CrouchSpeed = 1.5f;
		public float RunSpeed = 5f;
		public float SprintSpeed = 8f;

		public Player(Vector2 pos, Texture2D tex, World w)
			: base(pos, tex, w)
		{
			this.JumpStrength = 8f;
			this.WalkSpeed = 2.5f;
			_crouch = _world.Game.Content.Load<Texture2D>(@"Entites\PlayerCrouch");
			_oldTexture = tex;
		}

		public override void Update()
		{
			//crouching
			if (_world.Game.InputManager.IsKeyPressed(Keys.LeftControl) && this.OnGround)
			{
				if (this._texture == _oldTexture)
				{
					this._crouching = true;
					this.SetTexture(_crouch);
					this.JumpStrength = 4f;
				}
				else
				{
					if (this.canStand())
					{
						this._crouching = false;
						this.SetTexture(_oldTexture);
						this.JumpStrength = 8f;
					}
				}
			}

			//jumping
			if (_world.Game.InputManager.IsKeyPressed(Keys.Space))
			{
				this.TryJump();
			}

			if (_world.Game.InputManager.IsKeyDown(Keys.D))
			{
				if (!this._crouching)
				{
					this.Velocity.X = this.WalkSpeed;
				}
				else
				{
					this.Velocity.X = this.CrouchSpeed;
				}
			}
			else
			{
				this.Velocity.X = 0;

				if (_world.Game.InputManager.IsKeyDown(Keys.A))
				{
					if (!this._crouching)
					{
						this.Velocity.X = -this.WalkSpeed;
					}
					else
					{
						this.Velocity.X = -this.CrouchSpeed;
					}
				}
				else
				{
					this.Velocity.X = 0;
				}
			}

			if (_world.Game.InputManager.IsKeyPressed(Keys.R))
			{
				this.Position = new Vector2(72, 2000);
				this.Velocity = new Vector2(0);
			}

			base.Update();
		}

		private bool canStand()
		{
			Texture2D old = this._texture;
			Texture2D temp = new Texture2D(_world.Game.GraphicsDevice, _oldTexture.Width, _crouch.Height);
			this.SetTexture(temp);

			float? dist = this.CheckNearestCeiling();
			float diff = _oldTexture.Height - _crouch.Height;

			this.SetTexture(old);

			if (dist == null || (dist != null && dist > diff))
			{
				return true;
			}

			return false;
		}
	}
}
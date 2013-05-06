using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using To_Space.Common;
using To_Space.Terrain;
using To_Space.Terrain.Blocks;

namespace To_Space.Entities
{
	public class Entity
	{
		#region Static Entites
		public const float STANDARD_GRAVITY = .22f;

		public const float TERMINAL_VELOCITY = 8f;
		#endregion

		#region Properties
		#region Fields
		//Dimensions of the entity
		public int Width { get { return _texture.Width; } }
		public int Height { get { return _texture.Height; } }

		//Block dimensions of the entity
		public int MaxBlockWidth { get { return (Width % 32 == 0) ? ((Width / 32) + 1) : (((Width + 32) / 32) + 1); } }
		public int MaxBlockHeight { get { return (Height % 32 == 0) ? ((Height / 32) + 1) : (((Height + 32) / 32) + 1); } }

		//The texture of the entity
		protected Texture2D _texture;
		public Texture2D Texture { get { return _texture; } }

		//Bounding box for the entity
		private BoundingPolygon _boundingBox;
		public BoundingPolygon BoundingBox { get { return _boundingBox; } }

		//The world the entity is in
		protected World _world;
		public World World { get { return _world; } }

		//If this entity is the focus of a world camera
		private bool _focus = false;
		public bool Focus { get { return _focus; } }

		//If the entity if falling
		protected bool _onGround = true;
		public bool OnGround { get { return _onGround; } }

		//Other position properties
		public Vector2 EntityCenter { get { return new Vector2(Position.X + Width / 2, Position.Y + Height / 2); } }
		public Vector2 BottomCenter { get { return new Vector2(Position.X + Width / 2, Position.Y + Height); } }
		public Vector2 CenterBelow { get { return new Vector2(Position.X + Width / 2, Position.Y + Height + 1); } }
		#endregion

		//Position of the entity
		public Vector2 Position;

		//Velocity of the entity
		public Vector2 Velocity = new Vector2(0);

		//Maximum velocity of the entity
		public float MaxSpeedY = TERMINAL_VELOCITY;

		//Jump strength of this entity
		public float JumpStrength = 0f;

		//Speeds of the entity
		public float WalkSpeed = 2f;

		//If this entity ignores gravity
		public bool IgnoresGravity = false;
		#endregion

		public Entity(Vector2 position, Texture2D texture, World world)
		{
			Position = position;
			Velocity = new Vector2(0, 0);
			_texture = texture;
			_world = world;

			UpdateBoundingBox();
		}

		#region Frame Methods
		//ALWAYS ALWAYS ALWAYS CALL base.Update() WHEN OVERRIDING THIS METHOD
		//^^^^^^^^IMPORTANT^^^^^^^^^
		public virtual void Update()
		{
			Vector2 newPosition = this.Position + this.Velocity;
			BoundingPolygon offsetBox = this._boundingBox + this.Velocity;

			//Account for gravity
			this.Velocity.Y += STANDARD_GRAVITY;

			//Adjust to terminal velocity
			if (this.Velocity.Y > this.MaxSpeedY) this.Velocity.Y = this.MaxSpeedY;

			//Distances of the entity to nearby solid blocks
			float? disX = null;
			float? disY = null;

			//Testing block checking
			Block[] xBlocks = new Block[this.MaxBlockHeight + 1];
			Block[] yBlocks = new Block[this.MaxBlockWidth + 1];

			//Check for collisions
			if (this.Velocity.X > 0) //Moving right
			{
				disX = this.CheckNearestRightWall();

				if (disX != null && this.Velocity.X > disX)
				{
					this.Position.X += (float)disX;
					this.Velocity.X = 0f;
				}
			}
			else if (this.Velocity.X < 0) //Moving left
			{
				disX = this.CheckNearestLeftWall();

				if (disX != null && this.Velocity.X < -disX)
				{
					this.Position.X -= (float)disX;
					this.Velocity.X = 0f;
				}
			}

			if (this.Velocity.Y > 0) //Moving down
			{
				disY = this.CheckNearestGround();

				if (disY != null && this.Velocity.Y > disY)
				{
					this.Position.Y += (float)disY;
					this.Velocity.Y = 0f;
					this._onGround = true;
				}
			}
			else if (this.Velocity.Y < 0) //Moving up
			{
				disY = this.CheckNearestCeiling();

				if (disY != null && this.Velocity.Y < -disY)
				{
					this.Position.Y -= (float)disY;
					this.Velocity.Y = 0f;
				}
			}

			this.Position += this.Velocity;

			if (this.Velocity.Y > 0f)
			{
				this._onGround = false;
			}

			UpdateBoundingBox();
		}

		public virtual void Draw(SpriteBatch sBatch)
		{
			Vector2 screen = new Vector2(_world.Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
				_world.Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

			if (_focus)
			{
				sBatch.Draw(this._texture,
					new Vector2((screen.X / 2) - (this.Width / 2), (screen.Y / 2) - (this.Height / 2)), Color.White);
			}
			else
			{
				sBatch.Draw(this._texture,
					new Vector2(this.Position.X - this._world.Camera.Offset.X,
						this.Position.Y - this._world.Camera.Offset.Y), Color.White);
			}

			sBatch.Draw(this._texture, new Rectangle((int)(this.Position.X / 32), (int)(this.Position.Y / 32), 32, 32), Color.Red);
		}
		#endregion

		#region Action Methods
		public virtual bool TryJump()
		{
			if (this._onGround)
			{
				this.Velocity.Y = -this.JumpStrength;
				this._onGround = false;
				return true;
			}

			return false;
		}

		public virtual void ForceJump()
		{
			this.Velocity.Y = -this.JumpStrength;
			this._onGround = false;
		}
		#endregion

		#region Field Updates
		//If this entity is the focus of the camera
		public void SetFocus(bool focus)
		{
			_focus = focus;
		}

		public void SetTexture(Texture2D tex)
		{
			int diff = _texture.Height - tex.Height;

			this.Position.Y += diff;

			_texture = tex;

			UpdateBoundingBox();
		}

		public void UpdateBoundingBox()
		{
			_boundingBox = new BoundingPolygon
			(
				new Vector2(Position.X, Position.Y),
				new Vector2(Position.X + Width, Position.Y),
				new Vector2(Position.X, Position.Y + Height),
				new Vector2(Position.X + Width, Position.Y + Height)
			);
		}
		#endregion

		#region Utilities
		//Returns null for no ground or ground outside of needed check area, or float for distance to ground
		public float? CheckForGround()
		{
			int checkDelta = (int)(this.Width / this.MaxBlockWidth);

			float newY = this.Position.Y + this.Velocity.Y + this.Height + 1;

			for (int i = 0; i <= this.MaxBlockWidth; i++)
			{
				Block b = _world.GetBlockAt((int)((this.Position.X + (i * checkDelta)) / 32), (int)(newY / 32));

				if (b.Material.Solid)
				{
					return 32f - (this.Position.Y % 32f);
				}
			}

			return null;
		}
		#region Old Utilities
		//TODO: OPTIMIZE THE EVER-LIVING CRAP OUT OF THIS SECTION
		//Gets the distance to the nearest piece of ground
		protected float? CheckNearestGround()
		{
			int checkDelta = (int)(this.Width / this.MaxBlockWidth);

			float?[] distances = new float?[this.MaxBlockWidth + 1];

			for (int i = 0; i <= this.MaxBlockWidth; i++)
			{
				distances[i] = _world.DistanceToNearestGroundFrom(new Vector2(this.Position.X + (checkDelta * i), this.Position.Y + Height));
			}

			float? min = null;
			for (int i = 0; i < distances.Length; i++)
			{
				if (distances[i] != null)
				{
					if (min == null)
					{
						if (distances[i] != null)
						{
							min = distances[i];
						}
					}
					else
					{
						if (distances[i] != null)
						{
							if (min > distances[i])
							{
								min = distances[i];
							}
						}
					}
				}
			}

			return min;
		}

		//Gets the distance to the nearest ceiling
		protected float? CheckNearestCeiling()
		{
			int checkDelta = (int)(this.Width / this.MaxBlockWidth);

			float?[] distances = new float?[this.MaxBlockWidth + 1];

			for (int i = 0; i <= this.MaxBlockWidth; i++)
			{
				distances[i] = _world.DistanceToNearestCeilingFrom(new Vector2(this.Position.X + (i * checkDelta) - 1, this.Position.Y));
			}

			float? min = null;
			for (int i = 0; i < distances.Length; i++)
			{
				if (distances[i] != null)
				{
					if (min == null)
					{
						if (distances[i] != null)
						{
							min = distances[i];
						}
					}
					else
					{
						if (distances[i] != null)
						{
							if (min > distances[i])
							{
								min = distances[i];
							}
						}
					}
				}
			}

			return min;
		}

		//Gets the distance to the nearest right wall
		protected float? CheckNearestRightWall()
		{
			int checkDelta = (int)(this.Height / this.MaxBlockHeight);

			float?[] distances = new float?[this.MaxBlockHeight + 1];

			for (int i = 0; i <= this.MaxBlockHeight; i++)
			{
				distances[i] = _world.DistanceToNearestRightWallFrom(new Vector2(this.Position.X + Width, this.Position.Y + (i * checkDelta) - 1));
			}

			float? min = null;
			for (int i = 0; i < distances.Length; i++)
			{
				if (distances[i] != null)
				{
					if (min == null)
					{
						if (distances[i] != null)
						{
							min = distances[i];
						}
					}
					else
					{
						if (distances[i] != null)
						{
							if (min > distances[i])
							{
								min = distances[i];
							}
						}
					}
				}
			}

			return min;
		}

		//Gets the distance to the nearest left wall
		protected float? CheckNearestLeftWall()
		{
			int checkDelta = (int)(this.Height / this.MaxBlockHeight);

			float?[] distances = new float?[this.MaxBlockHeight + 1];

			for (int i = 0; i <= this.MaxBlockHeight; i++)
			{
				distances[i] = _world.DistanceToNearestLeftWallFrom(new Vector2(this.Position.X, this.Position.Y + (i * checkDelta) - 1));
			}

			float? min = null;
			for (int i = 0; i < distances.Length; i++)
			{
				if (distances[i] != null)
				{
					if (min == null)
					{
						if (distances[i] != null)
						{
							min = distances[i];
						}
					}
					else
					{
						if (distances[i] != null)
						{
							if (min > distances[i])
							{
								min = distances[i];
							}
						}
					}
				}
			}

			return min;
		}
		#endregion
		#endregion
	}
}
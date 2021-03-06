﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinShooterGame.GameObjects
{
    internal class Explosion
    {
        public Animation explosionAnimation;

        public Vector2 Position;

        public bool Active;

        private int timeToLive;

        public int Height
        {
            get
            {
                return explosionAnimation.FrameHeight;
            }
        }

        public int Width
        {
            get
            {
                return explosionAnimation.FrameWidth;
            }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            explosionAnimation = animation;
            Position = position;
            Active = true;

            timeToLive = 30;
        }

        public void Update(GameTime gameTime)
        {
            explosionAnimation.Update(gameTime);

            timeToLive -= 1;

            if (timeToLive <= 0)
            {
                this.Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            explosionAnimation.Draw(spriteBatch);
        }
    }
}
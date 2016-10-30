﻿/************************************************************************/
/* Author : David Amador
 * Web:      http://www.david-amador.com
 * Twitter : http://www.twitter.com/DJ_Link
 *
 * You can use this for whatever you want. If you want to give me some credit for it that's cool but not mandatory
/************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WinShooterGame
{
    public class Screen
    {
        protected GraphicsDevice _device = null;
        protected ContentManager _content = null;
        protected SpriteBatch _spriteBatch = null;

        /// <summary>
        /// Screen Constructor
        /// </summary>
        /// <param name="name">Must be unique since when you use ScreenManager is per name</param>
        public Screen(GraphicsDevice device, ContentManager content, string name)
        {
            Name = name;
            _device = device;
            _content = content;
            _spriteBatch = new SpriteBatch(_device);
        }

        ~Screen()
        {
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Virtual Function that's called when entering a Screen
        /// override it and add your own initialization code
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Virtual Function that's called when exiting a Screen
        /// override it and add your own shutdown code
        /// </summary>
        /// <returns></returns>
        public virtual void Shutdown()
        {
        }

        /// <summary>
        /// Override it to have access to elapsed time
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
            _spriteBatch.End();
        }

        public virtual void LoadContent()
        {
        }
    }
}
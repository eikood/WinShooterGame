﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Shooter;
using System;
using System.Collections.Generic;

namespace WinShooterGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    ///
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Represents the player
        private Player player;

        // Keyboard states used to determine key presses
        private KeyboardState currentKeyboardState;

        private KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        private GamePadState currentGamePadState;

        private GamePadState previousGamePadState;

        // Mouse states use to track Mouse button press
        private MouseState currentMouseState;

        //  private MouseState previousMouseState;

        // Movement speed for the player
        private float playerMoveSpeed;

        // Image used to display the static background
        private Texture2D mainBackground;

        private Rectangle rectBackground;
        private float scale = 1f;
        private ParallaxingBackground bgLayer1;
        private ParallaxingBackground bgLayer2;

        // texture to hold the laser.
        private Texture2D laserTexture;

        private List<Laser> laserBeams;

        // govern how fast our laser can fire.
        private TimeSpan laserSpawnTime;

        private TimeSpan previousLaserSpawnTime;

        // Collections of explosions
        List<Explosion> explosions;

        //Texture to hold explosion animation.
        Texture2D explosionTexture;

        // Enemies
        private Texture2D enemyTexture;

        private List<Enemy> enemies;

        // The rate at which the enemies appear
        private TimeSpan enemySpawnTime;

        private TimeSpan previousSpawnTime;

        // A random number generator
        private Random random;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the player class
            player = new Player();
            // init our laser
            laserBeams = new List<Laser>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
            // init our collection of explosions.
            explosions = new List<Explosion>();
            // Initialize the enemies list
            enemies = new List<Enemy>();
            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;
            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // Initialize our random number generator
            random = new Random();
            //Background
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            // Set a constant player move speed
            playerMoveSpeed = 8.0f;
            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player resources
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            player.Initialize(playerAnimation, playerPosition);
            // load th texture to serve as the laser
            laserTexture = Content.Load<Texture2D>("Graphics\\laser");
            // load the explosion sheet
            explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");
            // Load the enemies
            enemyTexture = Content.Load<Texture2D>("Graphics\\mineAnimation");
            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Graphics\\bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics\\bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Graphics\\mainbackground");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();
            //Update the player
            UpdatePlayer(gameTime);
            // update laserbeams
            for (var i = 0; i < laserBeams.Count; i++)
            {
                laserBeams[i].Update(gameTime);
                // Remove the beam when its deactivated or is at the end of the screen.
                if (!laserBeams[i].Active || laserBeams[i].Position.X > GraphicsDevice.Viewport.Width)
                {
                    laserBeams.Remove(laserBeams[i]);
                }
            }
            UpdateExplosions(gameTime);
            // Update the enemies
            UpdateEnemies(gameTime);
            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);
            // Update the collision
            UpdateCollision();
            base.Update(gameTime);
        }

        private void AddEnemy()

        {
            // Create the animation object
            Animation enemyAnimation = new Animation();
            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2,
            random.Next(100, GraphicsDevice.Viewport.Height - 100));
            // Create an enemy
            Enemy enemy = new Enemy();
            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);
            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)

        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // Add an Enemy
                AddEnemy();
            }
            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // we are going to use the rectangle's built in intersection methods.

            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle laserRectangle;
            // create the rectangle for the player
            playerRectangle = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            // detect collisions between the player and all enemies.
            for (var i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);
                // determine if the player and the enemy intersect.
                if (playerRectangle.Intersects(enemyRectangle))
                {
                    // kill off the enemy
                    enemies[i].Health = 0;
                    // Show the explosion where the enemy was...
                    AddExplosion(enemies[i].Position);
                    // deal damge to the player
                    player.Health -= enemies[i].Damage;
                    // if the player has no health destroy it.
                    if (player.Health <= 0)
                    {
                        player.Active = false;
                    }
                }
                for (var l = 0; l < laserBeams.Count; l++)
                {
                    // create a rectangle for this laserbeam
                    laserRectangle = new Rectangle((int)laserBeams[l].Position.X, (int)laserBeams[l].Position.Y, laserBeams[l].Width, laserBeams[l].Height);
                    // test the bounds of the laer and enemy
                    if (laserRectangle.Intersects(enemyRectangle))
                    {
                        // Show the explosion where the enemy was...
                        AddExplosion(enemies[i].Position);
                        // kill off the enemy
                        enemies[i].Health = 0;
                        // kill off the laserbeam
                        laserBeams[l].Active = false;
                    }
                }
            }
        }

        protected void FireLaser(GameTime gameTime)
        {
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                // Add the laser to our list.
                AddLaser();
            }
        }

        protected void AddLaser()
        {
            Animation laserAnimation = new Animation();
            // initlize the laser animation
            laserAnimation.Initialize(laserTexture, player.Position, 46, 16, 1, 30, Color.White, 1f, true);
            Laser laser = new Laser();
            // Get the starting postion of the laser.
            var laserPostion = player.Position;
            // Adjust the position slightly to match the muzzle of the cannon.
            laserPostion.Y += 37;
            laserPostion.X += 70;
            // init the laser
            laser.Initialize(laserAnimation, laserPostion);
            laserBeams.Add(laser);
            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        protected void AddExplosion(Vector2 enemyPosition)
        {
            Animation explosionAnimation = new Animation();
            explosionAnimation.Initialize(explosionTexture, enemyPosition, 134, 134, 12, 30, Color.White, 1.0f, true);
            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, enemyPosition);
            explosions.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (var e = 0; e < explosions.Count; e++)
            {
                explosions[e].Update(gameTime);
                if (!explosions[e].Active)
                    explosions.Remove(explosions[e]);
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            // Touch Gestures for MonoGame
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.Position += gesture.Delta;
                }
            }
            //Get Mouse State then Capture the Button type and Respond Button Press
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (currentMouseState.LeftButton == ButtonState.Pressed)

            {
                Vector2 posDelta = mousePosition - player.Position;
                posDelta.Normalize();
                posDelta = posDelta * playerMoveSpeed;
                player.Position = player.Position + posDelta;
            }

            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }
            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);
            // bind laserfire to spacebar
            if (currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                FireLaser(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Start drawing
            spriteBatch.Begin();
            // Draw the Main Background Texture
            spriteBatch.Draw(mainBackground, rectBackground, Color.White);
            // Draw the moving Background
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);
            // Draw the Player
            player.Draw(spriteBatch);
            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            // Draw the lasers.
            foreach (var l in laserBeams)
            {
                l.Draw(spriteBatch);
            }
            // draw explosions
            foreach (var e in explosions)
            {
                e.Draw(spriteBatch);
            }
            // Stop drawing
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
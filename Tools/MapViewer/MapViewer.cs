using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MapViewer.Core;
using MapViewer.Extra;

namespace MapViewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MapViewer : Microsoft.Xna.Framework.Game
    {
        public static ContentManager contentManager;
        public static GraphicsDeviceManager graphics;
        public static Camera camera;
        public static HUD hud;
        public static Random rnd = new Random();
        public static int mapResolution = 256;

        public static GroundCursor cursor;
        public static Map map;
        public static Game game;

        public MapViewer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            contentManager = Content;
            this.IsMouseVisible = true;
            Mouse.WindowHandle = this.Window.Handle;
            this.Window.Title = "Map Viewer";
            game = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera();
            hud = new HUD();
            cursor = new GroundCursor();
            map = new Map();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            camera.Update(gameTime);
            hud.Update(gameTime);
            cursor.Update();
            map.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.Clear(Color.Black);

            if (GraphicsDevice.RenderState.AlphaBlendEnable)
                GraphicsDevice.RenderState.AlphaBlendEnable = false;
            if (!GraphicsDevice.RenderState.DepthBufferEnable)
                GraphicsDevice.RenderState.DepthBufferEnable = true;
            if (!GraphicsDevice.RenderState.DepthBufferWriteEnable)
                GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            map.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            cursor.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            hud.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;

namespace MapViewer.Extra
{
    public class HUD
    {
        private float framesPerSecond = 0;
        private int frameAmount = 0;
        private double frameTime = 0;

        private SpriteBatch spriteBatch;
        SpriteFont MainFont;

        public HUD()
        {
            spriteBatch = new SpriteBatch(MapViewer.graphics.GraphicsDevice);
            MainFont = MapViewer.contentManager.Load<SpriteFont>("Fonts\\Impact");
        }

        public void Update(GameTime elapsedTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            // DONE: Calculate FPS
            frameAmount++;
            frameTime += gameTime.ElapsedRealTime.TotalSeconds;
            if (frameTime > 1.0)
            {
                framesPerSecond = (float)Math.Round(((double)frameAmount / frameTime), 1);
                frameTime = 0;
                frameAmount = 0;
            }

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            // DONE: Draw the FPS
            string FpsText = framesPerSecond.ToString() + " FPS";
            Vector2 FPSPos = new Vector2((MapViewer.graphics.GraphicsDevice.Viewport.Width - MainFont.MeasureString(FpsText).X) - 15, 10);
            spriteBatch.DrawString(MainFont, FpsText, FPSPos, Color.Black);
            FPSPos = new Vector2(FPSPos.X - 1, FPSPos.Y - 1);
            spriteBatch.DrawString(MainFont, FpsText, FPSPos, Color.White);

            //DONE: Draw camera coords
            if (MapViewer.camera != null)
            {
                string CoordsText = "X: " + MapViewer.camera.Position.X + "\r\nY: " + MapViewer.camera.Position.Y + "\r\nZ: " + MapViewer.camera.Position.Z;
                Vector2 CoordsPos = new Vector2(15, 10);
                spriteBatch.DrawString(MainFont, CoordsText, CoordsPos, Color.Black);
                CoordsPos = new Vector2(CoordsPos.X - 1, CoordsPos.Y - 1);
                spriteBatch.DrawString(MainFont, CoordsText, CoordsPos, Color.White);
            }

            //DONE: Draw cursor coords
            if (MapViewer.cursor != null)
            {
                string CursorCoordsText = "X: " + MapViewer.cursor.Position.X + "\r\nY: " + MapViewer.cursor.Position.Y + "\r\nZ: " + MapViewer.cursor.Position.Z;
                Vector2 CursorTextPos = new Vector2(15, 140);
                spriteBatch.DrawString(MainFont, CursorCoordsText, CursorTextPos, Color.Black);
                CursorTextPos = new Vector2(CursorTextPos.X - 1, CursorTextPos.Y - 1);
                spriteBatch.DrawString(MainFont, CursorCoordsText, CursorTextPos, Color.White);
            }

            spriteBatch.End();
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MapViewer.Extra
{
    public class Camera
    {

        #region Fields

        float cameraYaw = 0.0f;
        float cameraPitch = 0.0f;
        private Vector3 cameraPos;
        private Vector3 cameraTarget;
        private Vector3 cameraReference;
        private Vector3 direction = Vector3.Forward;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        public float fov;
        public float aspectRatio;
        public float NearPlane;
        public float FarPlane;

        private bool freeMode = false;
        private MouseState oldMouseState;
        private int lastScroll = 0;

        private Point center;

        #endregion

        #region Properties

        public Vector3 CameraPosition
        {
            get { return cameraPos; }
            set { cameraPos = value; }
        }

        public Vector3 CameraTarget
        {
            get { return cameraTarget; }
            set { cameraTarget = value; }
        }

        public Vector3 CameraDirection
        {
            get { return direction; }
        }

        public Vector3 Position
        {
            get { return cameraPos; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public bool FreeMode
        {
            get { return freeMode; }
        }

        #endregion

        public Camera()
        {
        }

        public void Initialize()
        {
            cameraPos = new Vector3(60.0f, 80.0f, -80.0f);
            cameraReference = new Vector3(0.0f, 0.0f, 1.0f);
            cameraTarget = cameraPos + cameraReference;
            direction = Vector3.Normalize(cameraPos - cameraTarget);
            cameraYaw = 0.0f;
            cameraPitch = 0.0f;
            viewMatrix = Matrix.CreateLookAt(cameraPos, cameraTarget, Vector3.Up);
            fov = MathHelper.PiOver4;
            aspectRatio = MapViewer.graphics.GraphicsDevice.Viewport.AspectRatio;
            NearPlane = .1f;
            FarPlane = 100000f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, NearPlane, FarPlane);

            Reinit();
        }

        public void Reinit()
        {
            center = new Point((MapViewer.graphics.GraphicsDevice.Viewport.Width / 2), (MapViewer.graphics.GraphicsDevice.Viewport.Height / 2));
            Mouse.SetPosition(center.X, center.Y);
            oldMouseState = Mouse.GetState();
            lastScroll = oldMouseState.ScrollWheelValue;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedRealTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();
            if (!freeMode && mouseState.RightButton == ButtonState.Pressed)
            {
                freeMode = true;
                Reinit();
                mouseState = oldMouseState;
            }
            else if (freeMode && mouseState.RightButton == ButtonState.Released)
                freeMode = false;

            if (freeMode)
            {
                float mouseX = mouseState.X - oldMouseState.X;
                float mouseY = mouseState.Y - oldMouseState.Y;
                cameraPitch += (mouseY * 0.4f) * deltaTime;
                cameraYaw -= (mouseX * 0.4f) * deltaTime;

                cameraPitch = MathHelper.Clamp(cameraPitch, MathHelper.ToRadians(-89.9f), MathHelper.ToRadians(89.9f));
            }

            UpdateFreeMode(gameTime);

            Matrix cameraViewRotation = Matrix.CreateRotationX(cameraPitch) *
                    Matrix.CreateRotationY(cameraYaw);

            Vector3 transformedCameraReference = Vector3.Transform(cameraReference, cameraViewRotation);
            cameraTarget = transformedCameraReference + cameraPos;

            direction = Vector3.Normalize(cameraPos - cameraTarget);

            viewMatrix = Matrix.CreateLookAt(cameraPos, cameraTarget, Vector3.Up);

            if (freeMode)
                Mouse.SetPosition(center.X, center.Y);
        }

        public void UpdateFreeMode(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedRealTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            Vector3 MoveDirection = Vector3.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                MoveDirection.Z = 50.0f * deltaTime;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                MoveDirection.Z = -50.0f * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                MoveDirection.X = 50.0f * deltaTime;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                MoveDirection.X = -50.0f * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                cameraPitch -= 0.7f * deltaTime;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                cameraPitch += 0.7f * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                cameraYaw += 1.0f * deltaTime;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                cameraYaw -= 1.0f * deltaTime;
            }

            Matrix cameraMoveRotation = Matrix.CreateRotationX(cameraPitch) *
                    Matrix.CreateRotationY(cameraYaw);

            cameraPos += Vector3.Transform(MoveDirection, cameraMoveRotation);
        }

    }
}
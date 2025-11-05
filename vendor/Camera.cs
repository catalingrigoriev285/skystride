using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skystride.vendor
{
    internal class Camera
    {
        // instances
        public Vector3 position { get; private set; }
        public Vector3 front { get; private set; } = -Vector3.UnitZ;
        public Vector3 up { get; private set; } = Vector3.UnitY;
        public Vector3 right { get; private set; } = Vector3.UnitX;

        // euler rotations
        private float yaw = -90.0f;
        private float pitch = 0.0f;

        private float speed = 5f;

        private float sensitivity = 0.2f;

        private float fov = 60.0f;
        private float aspectRatio;

        private bool firstMoveState = true;
        private Vector2 latestMousePosition;

        public Camera(Vector3 _position, float _aspectRatio)
        {
            this.position = _position;
            this.aspectRatio = _aspectRatio;

            this.UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(this.position, this.position + this.front, this.up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(this.fov), this.aspectRatio, 0.1f, 1000f);
        }

        public void UpdateKeyboardState(KeyboardState _currentKeyboardState, float deltaTime)
        {
            float velocity = speed * deltaTime;

            if (_currentKeyboardState.IsKeyDown(Key.W))
                this.position += this.front * velocity;
            if (_currentKeyboardState.IsKeyDown(Key.S))
                this.position -= this.front * velocity;
            if (_currentKeyboardState.IsKeyDown(Key.A))
                this.position -= this.right * velocity;
            if (_currentKeyboardState.IsKeyDown(Key.D))
                this.position += this.right * velocity;
            if (_currentKeyboardState.IsKeyDown(Key.Space))
                this.position += this.up * velocity;
            if (_currentKeyboardState.IsKeyDown(Key.ControlLeft))
                this.position -= this.up * velocity;
        }

        public void UpdateMouseState(MouseState _currentMouseState)
        {
            Vector2 _mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

            if (this.firstMoveState)
            {
                this.latestMousePosition = _mousePosition;
                this.firstMoveState = false;
            }

            float deltaX = _currentMouseState.X - this.latestMousePosition.X;
            float deltaY = _currentMouseState.Y - this.latestMousePosition.Y;
            this.latestMousePosition = _mousePosition;

            this.yaw += deltaX * this.sensitivity;
            this.pitch -= deltaY * this.sensitivity;

            pitch = MathHelper.Clamp(this.pitch, -89f, 89f);

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            Vector3 _frontState;
            _frontState.X = (float)(System.Math.Cos(MathHelper.DegreesToRadians(this.yaw)) * System.Math.Cos(MathHelper.DegreesToRadians(this.pitch)));
            _frontState.Y = (float)System.Math.Sin(MathHelper.DegreesToRadians(this.pitch));
            _frontState.Z = (float)(System.Math.Sin(MathHelper.DegreesToRadians(this.yaw)) * System.Math.Cos(MathHelper.DegreesToRadians(this.pitch)));

            this.front = Vector3.Normalize(_frontState);
            this.right = Vector3.Normalize(Vector3.Cross(this.front, Vector3.UnitY));
            this.up = Vector3.Normalize(Vector3.Cross(this.right, this.front));
        }

        public void Resize(float _aspect_ratio)
        {
            this.aspectRatio = _aspect_ratio;
        }
    }
}

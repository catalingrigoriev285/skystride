using OpenTK;
using OpenTK.Input;

namespace skystride.vendor
{
    internal class CameraPhysics
    {
        private readonly Camera camera;

        private float moveSpeed = 6.0f;         // horizontal move speed (m/s)
        private float jumpSpeed = 6.5f;         // initial jump velocity
        private float gravity = -18.0f;         // gravity acceleration (m/s^2)
        private float groundY = 0.0f;           // flat ground plane at Y=0
        private float damping = 8.0f;           // air damping for horizontal velocity blending
        private float eyeHeight = 1.7f;         // keep camera above ground to avoid clipping through the terrain plane

        private Vector3 velocity;
        private bool isGrounded = false;

        public CameraPhysics(Camera camera)
        {
            this.camera = camera;
        }

        public void Update(KeyboardState current, KeyboardState previous, float dt)
        {
            if (dt <= 0f) return;

            Vector3 forward = camera.front; forward.Y = 0f; forward.NormalizeFast();
            Vector3 right = camera.right; right.Y = 0f; right.NormalizeFast();

            Vector3 wishDir = Vector3.Zero;
            if (current.IsKeyDown(Key.W)) wishDir += forward;
            if (current.IsKeyDown(Key.S)) wishDir -= forward;
            if (current.IsKeyDown(Key.D)) wishDir += right;
            if (current.IsKeyDown(Key.A)) wishDir -= right;
            if (wishDir.LengthSquared > 0f) wishDir.NormalizeFast();

            Vector3 targetHorizontalVel = wishDir * moveSpeed;
            Vector3 currentHorizontalVel = new Vector3(velocity.X, 0f, velocity.Z);

            float t = 1f - (float)System.Math.Exp(-damping * dt);
            Vector3 newHorizontalVel = currentHorizontalVel + (targetHorizontalVel - currentHorizontalVel) * t;

            velocity.X = newHorizontalVel.X;
            velocity.Z = newHorizontalVel.Z;

            bool jumpPressed = current.IsKeyDown(Key.Space) && !previous.IsKeyDown(Key.Space);
            if (jumpPressed && isGrounded)
            {
                velocity.Y = jumpSpeed;
                isGrounded = false;
            }

            velocity.Y += gravity * dt;

            Vector3 pos = camera.position;
            pos += velocity * dt;

            float minY = groundY + eyeHeight;
            if (pos.Y <= minY)
            {
                pos.Y = minY;
                if (velocity.Y < 0f) velocity.Y = 0f;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            camera.SetPosition(pos);
        }

        public void SetGroundY(float y) { groundY = y; }
        public void SetMoveSpeed(float s) { moveSpeed = s; }
        public void SetJumpSpeed(float s) { jumpSpeed = s; }
        public void SetGravity(float g) { gravity = g; }
        public void SetEyeHeight(float h) { eyeHeight = h; }
    }
}

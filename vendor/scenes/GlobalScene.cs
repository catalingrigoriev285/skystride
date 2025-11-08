using System;
using System.Collections.Generic;
using OpenTK;
using skystride.vendor;

namespace skystride.scenes
{
    internal class GlobalScene : IDisposable
    {
        // Unified list for all entities
        protected readonly List<ISceneEntity> Entities = new List<ISceneEntity>();

        // Caching a dedicated list of collidables can speed up large scenes
        // so we don't have to cast 'as ICollidable' every single frame for every entity.
        protected readonly List<ICollidable> Collidables = new List<ICollidable>();

        private Vector3 _lastCameraPos;
        private bool _hasLastCam = false;

        // Tunables
        private const float MaxStepHeight = 0.35f; // Max height you can "snap" up onto
        private const float CollisionEpsilon = 0.001f; // Small buffer to prevent z-fighting/jitter

        protected sealed class ModelEntity : ISceneEntity, IDisposable
        {
            private readonly Model _model;
            private readonly Vector3 _position;
            private readonly float _scale;
            private readonly float _rx, _ry, _rz;

            public ModelEntity(Model model, Vector3 position, float scale, float rx, float ry, float rz)
            {
                _model = model; _position = position; _scale = scale; _rx = rx; _ry = ry; _rz = rz;
            }
            public void Render()
            {
                if (_model != null && _model.Loaded)
                    _model.Render(_position, _scale, _rx, _ry, _rz);
            }
            public void Dispose() { try { _model?.Dispose(); } catch { } }
        }

        protected void AddEntity(ISceneEntity entity)
        {
            Entities.Add(entity);
            // Optimization: Cache collidables on add
            if (entity is ICollidable collidable)
            {
                Collidables.Add(collidable);
            }
        }

        public virtual void Update(Camera camera, float dt)
        {
            if (camera == null) return;

            Vector3 camHalfExtents = new Vector3(0.4f, 0.9f, 0.4f);
            float eyeH = camera.EyeHeight;
            float feetY = camera.position.Y - eyeH;
            float prevFeetY = _hasLastCam ? (_lastCameraPos.Y - eyeH) : feetY;

            float? highestSupportY = null;

            int count = Collidables.Count;

            // --- PASS 1: Vertical Support & Landing ---
            for (int i = 0; i < count; i++)
            {
                var c = Collidables[i];
                if (!c.IsActive) continue;
                var box = c.GetBounds();

                // Strict horizontal overlap test for support (must be actually standing OVER it)
                // We use a slightly smaller 'feet' radius for support to prevent hanging off edges too easily
                float supportPadding = 0.05f;
                bool physicallyOverlapping = (camera.position.X + camHalfExtents.X - supportPadding) >= box.Min.X
                                          && (camera.position.X - camHalfExtents.X + supportPadding) <= box.Max.X
                                          && (camera.position.Z + camHalfExtents.Z - supportPadding) >= box.Min.Z
                                          && (camera.position.Z - camHalfExtents.Z + supportPadding) <= box.Max.Z;

                if (!physicallyOverlapping) continue;

                float topY = box.Max.Y;

                // FIX: Only consider this a support if our feet are at or above it, 
                // or within a small "step up" range. 
                // This prevents recognizing a tall wall as a floor just because we touched it.
                bool isViableSupport = (feetY >= (topY - MaxStepHeight));

                if (isViableSupport)
                {
                    // Track highest valid ground below us
                    if (!highestSupportY.HasValue || topY > highestSupportY.Value)
                    {
                        highestSupportY = topY;
                    }

                    // Landing logic: Must have been above it previously to land ON it.
                    if (prevFeetY >= topY && feetY <= topY)
                    {
                        camera.LandOn(topY);
                        feetY = topY; // Update local feetY immediately
                    }
                }
            }

            // Apply the support force (prevents falling through ground detected in Pass 1)
            camera.SetSupportTop(highestSupportY);

            // Re-calculate box after potential vertical snapping from LandOn/SetSupportTop
            var camBox = BoundingBox.FromCenterHalfExtents(camera.position, camHalfExtents);

            // --- PASS 2: Horizontal Push-out (Collision) ---
            for (int i = 0; i < count; i++)
            {
                var c = Collidables[i];
                if (!c.IsActive) continue;
                var box = c.GetBounds();

                if (!box.Intersects(camBox)) continue;

                // If we are comfortably on top of this object, don't collide with it horizontally.
                // (Assumes feet are exactly on topY due to SetSupportTop above)
                if (feetY >= box.Max.Y - CollisionEpsilon) continue;

                // Determine minimal push vector
                float pushX1 = box.Max.X - camBox.Min.X + CollisionEpsilon;
                float pushX2 = camBox.Max.X - box.Min.X + CollisionEpsilon;
                float pushZ1 = box.Max.Z - camBox.Min.Z + CollisionEpsilon;
                float pushZ2 = camBox.Max.Z - box.Min.Z + CollisionEpsilon;

                // Find smallest penetration axis
                float minPush = pushX1;
                int axis = 0; // 0 = +X, 1 = -X, 2 = +Z, 3 = -Z

                if (pushX2 < minPush) { minPush = pushX2; axis = 1; }
                if (pushZ1 < minPush) { minPush = pushZ1; axis = 2; }
                if (pushZ2 < minPush) { minPush = pushZ2; axis = 3; }

                // Apply push strictly on the horizontal plane
                switch (axis)
                {
                    case 0: camera.AddPosition(new Vector3(pushX1, 0, 0)); break;
                    case 1: camera.AddPosition(new Vector3(-pushX2, 0, 0)); break;
                    case 2: camera.AddPosition(new Vector3(0, 0, pushZ1)); break;
                    case 3: camera.AddPosition(new Vector3(0, 0, -pushZ2)); break;
                }

                // Update camBox for next collision check in this same frame
                camBox = BoundingBox.FromCenterHalfExtents(camera.position, camHalfExtents);
            }

            _lastCameraPos = camera.position;
            _hasLastCam = true;
        }

        public virtual void Render()
        {
            // Standard for-loop is often slightly faster than foreach on Lists in hot paths
            int count = Entities.Count;
            for (int i = 0; i < count; i++)
            {
                Entities[i].Render();
            }
        }

        public virtual void Dispose()
        {
            int count = Entities.Count;
            for (int i = 0; i < count; i++)
            {
                (Entities[i] as IDisposable)?.Dispose();
            }
            Entities.Clear();
            Collidables.Clear();
        }
    }
}
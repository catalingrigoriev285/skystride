using OpenTK;
using OpenTK.Input;
using skystride.objects;
using skystride.objects.templates;
using skystride.vendor;
using skystride.vendor.collision;
using System;
using System.Collections.Generic;

namespace skystride.scenes
{
    internal class GlobalScene : IDisposable
    {
        // Unified list for all entities
        protected readonly List<ISceneEntity> Entities = new List<ISceneEntity>();
        
        // Global colliders accumulated automatically
        protected readonly List<AABB> Colliders = new List<AABB>();

        protected sealed class ModelEntity : ISceneEntity, IDisposable
        {
            private readonly Model _model;
            private readonly Vector3 _position;
            private readonly float _scale;
            private readonly float _rx, _ry, _rz;
            public ModelEntity(Model model, Vector3 position, float scale, float rx, float ry, float rz, float texScaleU = 1f, float texScaleV = 1f)
            {
                _model = model; _position = position; _scale = scale; _rx = rx; _ry = ry; _rz = rz;
                _model?.SetTextureScale(texScaleU, texScaleV);
            }
            public void Render()
            {
                if (_model != null && _model.Loaded)
                    _model.Render(_position, _scale, _rx, _ry, _rz);
            }
            public Vector3 GetPosition() { return _position; }
            public Vector3 GetSize() { return _model != null ? _model.BoundsSize * _scale : Vector3.Zero; }
            public void SetTextureScale(float u, float v)
            {
                _model?.SetTextureScale(u, v);
            }
            public void Dispose()
            {
                try { _model?.Dispose(); } catch { }
            }
        }

        protected void AddEntity(ISceneEntity entity, bool collidable = true)
        {
            if(entity == null)
                return;

            Entities.Add(entity);

            if (!collidable)
                return;

            var modelEnt = entity as ModelEntity;
            if (modelEnt != null)
            {
                var size = modelEnt.GetSize();
                if (size != Vector3.Zero)
                    Colliders.Add(new AABB(modelEnt.GetPosition(), size));
                return;
            }

            var cube = entity as Cube;
            if (cube != null)
            {
                float size = cube.GetSize();
                Colliders.Add(new AABB(cube.GetPosition(), new Vector3(size, size, size)));
                return;
            }

            var plane = entity as Plane;
            if (plane != null)
            {
                var size = plane.GetSize();
                if (size != Vector3.Zero)
                {
                    // If plane is rotated we need to inflate its axis-aligned collider to cover the rotated extents.
                    Vector3 rotDeg = plane.GetRotation();
                    // Treat very thin planes (height <=0) as having a small thickness for collision purposes.
                    float effectiveHeight = size.Y <=0f ?0.05f : size.Y; // small thickness if flat
                    float hx = size.X *0.5f; // local half-width (X)
                    float hy = effectiveHeight *0.5f; // local half-height (Y)
                    float hz = size.Z *0.5f; // local half-depth (Z)

                    Vector3 colliderSize;
                    if (rotDeg != Vector3.Zero)
                    {
                        // Yaw (Y), Pitch (X), Roll (Z)
                        float ry = MathHelper.DegreesToRadians(rotDeg.Y);
                        float rx = MathHelper.DegreesToRadians(rotDeg.X);
                        float rz = MathHelper.DegreesToRadians(rotDeg.Z);
                        Matrix4 R = Matrix4.CreateRotationY(ry) * Matrix4.CreateRotationX(rx) * Matrix4.CreateRotationZ(rz);
                        
                        Vector4 r0 = R.Row0;
                        Vector4 r1 = R.Row1;
                        Vector4 r2 = R.Row2;
                        float ex = Math.Abs(r0.X) * hx + Math.Abs(r0.Y) * hy + Math.Abs(r0.Z) * hz; // extent along world X
                        float ey = Math.Abs(r1.X) * hx + Math.Abs(r1.Y) * hy + Math.Abs(r1.Z) * hz; // extent along world Y
                        float ez = Math.Abs(r2.X) * hx + Math.Abs(r2.Y) * hy + Math.Abs(r2.Z) * hz; // extent along world Z
                        colliderSize = new Vector3(ex *2f, ey *2f, ez *2f);
                    }
                    else
                    {
                        colliderSize = new Vector3(hx *2f, hy *2f, hz *2f); // unrotated
                    }

                    Colliders.Add(new AABB(plane.GetPosition(), colliderSize));
                }
                return;
            }

            var checkboardTerrain = entity as CheckboardTerrain;
            if(checkboardTerrain != null)
            {
                float halfSpan = checkboardTerrain.GetSize(); // tiles * tileSize (half span)
                float fullSpan = halfSpan *2f; // cover -tiles .. +tiles
                const float groundThickness =0.2f; // thin collision layer
                Colliders.Add(new AABB(checkboardTerrain.GetPosition(), new Vector3(fullSpan, groundThickness, fullSpan)));
                return;
            }

            // other entity types can be added here
        }

        // Per-frame logic hook for scenes
        public virtual void Update(float dt, Camera camera, KeyboardState currentKeyboard, KeyboardState previousKeyboard, MouseState currentMouse, MouseState previousMouse)
        {
            // base scene does nothing
        }

        public virtual void Render()
        {
            for (int i =0; i < Entities.Count; i++)
            {
                Entities[i].Render();
            }
        }

        public virtual void Dispose()
        {
            for (int i =0; i < Entities.Count; i++)
            {
                var disp = Entities[i] as IDisposable;
                if (disp != null)
                {
                    try { disp.Dispose(); } catch { }
                }
            }
        }
    }
}

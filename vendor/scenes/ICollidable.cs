using OpenTK;

namespace skystride.scenes
{
 // Axis-aligned bounding box used for simple collision checks
 public struct BoundingBox
 {
 public Vector3 Min;
 public Vector3 Max;

 public BoundingBox(Vector3 min, Vector3 max)
 {
 // ensure proper ordering
 Min = Vector3.ComponentMin(min, max);
 Max = Vector3.ComponentMax(min, max);
 }

 public static BoundingBox FromCenterHalfExtents(Vector3 center, Vector3 halfExtents)
 {
 return new BoundingBox(center - halfExtents, center + halfExtents);
 }

 public bool Intersects(BoundingBox other)
 {
 return (Min.X <= other.Max.X && Max.X >= other.Min.X)
 && (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y)
 && (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
 }
 }

 // Optional collidable contract for entities which should block the camera
 public interface ICollidable
 {
 // Whether this collider should participate in collisions
 bool IsActive { get; }

 // World-space AABB for this entity
 BoundingBox GetBounds();
 }
}

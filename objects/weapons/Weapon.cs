using System;
using OpenTK;
using skystride.vendor;

namespace skystride.objects.weapons
{
    internal class Weapon
    {
        protected int ammo, damage;
        protected string name;
        protected Model model; // 3D model

        public Weapon(string name, int ammo, int damage)
        {
            this.name = name;
            this.ammo = ammo;
            this.damage = damage;
        }

        public virtual void Render(Camera _camera)
        {
            if (model == null || !model.Loaded || _camera == null) return;
            Vector3 pos = new Vector3(0.6f, -0.6f, -1.6f); // X right, Y down, Z into screen

            float rotX = 0f;
            float rotY = 0f;
            float scale = 0.25f;
            model.Render(pos, scale, rotX, rotY, 0f);
        }
    }
}

using skystride.vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace skystride.objects.weapons.pistols
{
    internal class Glock : Weapon
    {
        public Glock() : base("Glock", 17, 25)
        {
            this.model = new Model("assets/models/weapons/glock.obj", "assets/models/weapons/glock.jpg");
            this.model.SetTextureScale(1f,1f);
        }

        public override void Render(Camera _camera)
        {
            if (model == null || !model.Loaded || _camera == null) return;

            Vector3 pos = new Vector3(0.9f, -0.7f, -1.8f); // tweak for glock
            float scale = 0.15f;
            model.Render(pos, scale, 0f, 0f, 0f);
        }
    }
}

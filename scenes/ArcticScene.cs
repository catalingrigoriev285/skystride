using OpenTK;
using skystride.objects;
using skystride.objects.templates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using skystride.shaders;

namespace skystride.scenes
{
    internal class ArcticScene : GlobalScene
    {
        public ArcticScene()
        {
            Plane platform = new Plane(new Vector3(0f, 0f, 0f), 70f, 70f, 1f, Color.IndianRed, new Vector3(0f, 1f, 0f));
            platform.SetTexture("assets/textures/snow.png");
            AddEntity(platform);


            AddEntity(new Snow(count: 7500, areaSize: 120f, spawnHeight: 50f, groundY: 0f, minSpeed: 1.5f, maxSpeed: 4.5f));
        }
    }
}

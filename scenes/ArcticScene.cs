using OpenTK;
using skystride.objects;
using skystride.objects.templates;
using skystride.shaders;
using skystride.vendor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skystride.scenes
{
    internal class ArcticScene : GlobalScene
    {
        public ArcticScene()
        {
            Plane platform = new Plane(new Vector3(0f, 0f, 0f), 40f, 70f, 1f, Color.IndianRed, new Vector3(0f, 1f, 0f));
            platform.SetTexture("assets/textures/snow.png");
            platform.SetTextureScale(5, 5);
            AddEntity(platform);

            Plane pereteA = new Plane(new Vector3(20f, 5f, 0f), 1f, 70f, 10f, Color.LightGray, new Vector3(-1f, 0f, 0f));
            pereteA.SetTexture("assets/textures/snow.png");
            pereteA.SetTextureScale(5, 5);
            AddEntity(pereteA);

            Plane pereteC = new Plane(new Vector3(-37f, 5f, 35.5f), 150f, 1f, 10f, Color.LightGray, new Vector3(0f, 0f, -1f));
            pereteC.SetTexture("assets/textures/snow.png");
            pereteC.SetTextureScale(5, 5);
            AddEntity(pereteC);

            Plane pereteD = new Plane(new Vector3(-37f, 0f, -35.5f), 150f, 1f, 20f, Color.LightGray, new Vector3(0f, 0f, -1f));
            pereteD.SetTexture("assets/textures/snow.png");
            pereteD.SetTextureScale(5, 5);
            AddEntity(pereteD);

            AddEntity(new ModelEntity(
                new Model("/assets/models/copac1.obj", "/assets/models/copac1.png"),
                new Vector3(12f, 6f, 22f), 1f, 0f, 0f, 0f));

            AddEntity(new ModelEntity(
                new Model("/assets/models/copac1.obj", "/assets/models/copac1.png"),
                new Vector3(-13f, 6f, 14f), 1f, 0f, 0f, 0f));

            AddEntity(new ModelEntity(
                new Model("/assets/models/iashik.obj", "/assets/models/iashik.jpg"),
                new Vector3(-27f, 0f, 0f), 5f, 0f, 90f, -360f));

            AddEntity(new ModelEntity(
                new Model("/assets/models/iashik.obj", "/assets/models/iashik.jpg"),
                new Vector3(-37f, 0f, 4f), 5f, 0f, 90f, -360f));

            AddEntity(new ModelEntity(
                new Model("/assets/models/copac1.obj", "/assets/models/copac1.png"),
                new Vector3(-4f, 6f, -22f), 1f, 0f, 0f, 0f));

            AddEntity(new Snow(count: 7500, areaSize: 120f, spawnHeight: 50f, groundY: 0f, minSpeed: 1.5f, maxSpeed: 4.5f));
        }
    }
}

using skystride.vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skystride.objects.weapons.pistols
{
    internal class glock : Weapon
    {
        private Model model;
        public glock() : base("Glock", 17, 25)
        {
            this.model = new Model("assets/models/weapons/glock.obj");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skystride.objects.weapons
{
    internal class Weapon
    {
        protected int ammo, damage;
        protected string name;

        public Weapon(string name, int ammo, int damage)
        {
            this.name = name;
            this.ammo = ammo;
            this.damage = damage;
        }
    }
}

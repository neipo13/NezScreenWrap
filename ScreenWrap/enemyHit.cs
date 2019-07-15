using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Microsoft.Xna.Framework;

namespace ScreenWrap
{
    public class EnemyHit : Component, ITriggerListener
    {
        public void onTriggerEnter(Collider other, Collider local)
        {
            if(other.active)
                Console.WriteLine("OUCH!");
        }

        public void onTriggerExit(Collider other, Collider local)
        {
        }
    }
}

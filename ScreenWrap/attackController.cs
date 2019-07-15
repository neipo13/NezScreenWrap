using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Microsoft.Xna.Framework;
using Nez.Sprites;

namespace ScreenWrap
{
    public class AttackController : Component, IUpdatable
    {
        Sprite<ObjectFactory.PlayerAnims> sprite;
        BoxCollider collider;
        Input input;
        ColliderTriggerHelper triggerHelper;

        public AttackController()
        {
        }
        public override void onAddedToEntity()
        {
            sprite = entity.getComponent<Sprite<ObjectFactory.PlayerAnims>>();
            collider = entity.getComponents<BoxCollider>().FirstOrDefault(c => c.name == "hitbox");
            input = entity.getComponent<Input>();
            triggerHelper = new ColliderTriggerHelper(entity);
        }

        public void update()
        {
            if (input.Button1Input.isPressed && sprite.currentAnimation != ObjectFactory.PlayerAnims.Attack)
            {
                sprite.play(ObjectFactory.PlayerAnims.Attack);
                collider.active = true;
            }
            triggerHelper.update();
        }
    }
}

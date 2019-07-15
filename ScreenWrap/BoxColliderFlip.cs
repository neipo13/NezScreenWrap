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
    /// <summary>
    /// Box collider that flips sides when the sprite flips.
    /// Useful for off-center colliders which need to change sides.
    /// </summary>
    public class BoxColliderFlip : BoxCollider, IUpdatable
    {
        Sprite sprite;
        float x = 0;
        float offsetX => x + width / 2;
        public BoxColliderFlip(float x, float y, float width, float height) : base(x, y, width, height)
        {
            this.x = x;
        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            sprite = entity.getComponent<Sprite>();
        }
        public void update()
        {
            if (!sprite.flipX)
            {
                this.localOffset = new Vector2(offsetX, this.localOffset.Y);
            }
            else
            {
                this.localOffset = new Vector2(-offsetX, this.localOffset.Y);
            }
        }
        
    }
}

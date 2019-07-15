using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace ScreenWrap
{
    public class ScreenWrapComponent : RenderableComponent, IUpdatable
    {
        Vector2 screenSize;
        Vector2 spriteSize;
        Vector2 halfSpriteSize => spriteSize / 2;
        Vector2 positionToCenter => entity.position - sprite.bounds.center;
        Sprite sprite;
        public override float width { get { return spriteSize.X; } }
        public override float height { get { return spriteSize.Y; } }

        public const string XDupeCollider = "XDupeCollider";
        public const string YDupeCollider = "YDupeCollider";
        public bool facingLeft => sprite.flipX;
        public bool facingRight => !facingLeft;
        public bool partiallyOffScreenLeft => entity.position.X < (halfSpriteSize.X + positionToCenter.X);
        public bool partiallyOffScreenRight => entity.position.X > screenSize.X - (halfSpriteSize.X - positionToCenter.X);
        public bool partiallyOffScreenTop => entity.position.Y < halfSpriteSize.Y + positionToCenter.Y;
        public bool partiallyOffScreenBottom => entity.position.Y > screenSize.Y - (halfSpriteSize.Y - positionToCenter.Y);

        public bool halfOffscreenLeft => entity.position.X < 0;
        public bool halfOffscreenRight => entity.position.X > screenSize.X;
        public bool completelyOffscreenLeft => entity.position.X < -halfSpriteSize.X;
        public bool completelyOffscreenRight => entity.position.X > halfSpriteSize.X + screenSize.X;
        public bool completelyOffscreenTop => entity.position.Y < -halfSpriteSize.Y;
        public bool completelyOffscreenBottom => entity.position.Y > halfSpriteSize.Y + screenSize.Y;
        /// <summary>
        /// this must return true always in a single screen game as otherwise the render will never happen as the entity in question is offscreen
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public override bool isVisibleFromCamera(Camera camera)
        {
            return true;
        }
        
        public ScreenWrapComponent(Vector2 screenSize, Vector2 spriteSize, Sprite sprite)
        {
            this.screenSize = screenSize;
            this.spriteSize = spriteSize;
            this.sprite = sprite;
        }

        /// <summary>
        /// Renders a copy of the sprite if any portion of the object is offscreen
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="camera"></param>
        public override void render(Graphics graphics, Camera camera)
        {
            if (partiallyOffScreenLeft)
            {
                graphics.batcher.draw(sprite.subtexture, new Vector2(screenSize.X + entity.position.X, entity.position.Y), sprite.color, entity.transform.rotation, sprite.origin, entity.transform.scale, sprite.spriteEffects, sprite.layerDepth);
            }
            else if (partiallyOffScreenRight)
            {
                graphics.batcher.draw(sprite.subtexture, new Vector2(entity.position.X - screenSize.X, entity.position.Y), sprite.color, entity.transform.rotation, sprite.origin, entity.transform.scale, sprite.spriteEffects, sprite.layerDepth);
            }
            if (partiallyOffScreenTop)
            {
                graphics.batcher.draw(sprite.subtexture, new Vector2(entity.position.X, screenSize.Y + halfSpriteSize.Y - (halfSpriteSize.Y - entity.position.Y)), sprite.color, entity.transform.rotation, sprite.origin, entity.transform.scale, sprite.spriteEffects, sprite.layerDepth);
            }
            else if (partiallyOffScreenBottom)
            {
                graphics.batcher.draw(sprite.subtexture, new Vector2(entity.position.X, -halfSpriteSize.Y + (-screenSize.Y + halfSpriteSize.Y + entity.position.Y)), sprite.color, entity.transform.rotation, sprite.origin, entity.transform.scale, sprite.spriteEffects, sprite.layerDepth);
            }
        }
        
        public void update()
        {
            UpdateEntityPosition();
            UpdateEntityColliders();
            UpdateEntityComponents();
        }

        protected virtual void UpdateEntityPosition()
        {
            
            //move entity when entirety is offscreen
            if (completelyOffscreenLeft)
            {
                entity.position = new Vector2(screenSize.X - halfSpriteSize.X, entity.position.Y);
            }
            else if (completelyOffscreenRight )
            {
                entity.position = new Vector2(halfSpriteSize.X, entity.position.Y);
            }
            if (completelyOffscreenTop)
            {
                entity.position = new Vector2(entity.position.X, screenSize.Y - halfSpriteSize.Y);
            }
            else if (completelyOffscreenBottom)
            {
                entity.position = new Vector2(entity.position.X, halfSpriteSize.Y);
            }

            if(facingLeft && halfOffscreenLeft)
            {
                entity.position = new Vector2(screenSize.X, entity.position.Y);
            }
            else if (facingRight && halfOffscreenRight)
            {
                entity.position = new Vector2(0, entity.position.Y);
            }

        }

        protected virtual void UpdateEntityColliders()
        {
            if(partiallyOffScreenLeft || partiallyOffScreenRight)
            {

                var colliders = entity.getComponents<BoxCollider>();
                var dupedX = colliders.Where(c => c.name.Contains(XDupeCollider)).ToList();
                var regularX = colliders.Where(c => !c.name.Contains(XDupeCollider)).ToList();
                var undupedX = new List<BoxCollider>();
                //duplicate colliders when wrapping at all
                if (partiallyOffScreenLeft)
                {
                    foreach (BoxCollider collider in regularX)
                    {
                        if (!dupedX.Any(c => c.name.Contains(collider.name)))
                        {
                            undupedX.Add(collider);
                        }
                    }
                    foreach (BoxCollider collider in dupedX)
                    {
                        var name = collider.name.Remove(collider.name.Length - XDupeCollider.Length);
                        var original = regularX.SingleOrDefault(c => c.name == name);
                        collider.localOffset = original.localOffset;
                        collider.localOffset += new Vector2(screenSize.X, 0);
                    }
                    foreach (BoxCollider collider in undupedX)
                    {
                        var box = entity.addComponent(new BoxCollider(collider.absolutePosition.X, collider.absolutePosition.Y, collider.width, collider.height));
                        box.name = collider.name + XDupeCollider;
                        box.localOffset += new Vector2(screenSize.X, 0);
                        box.physicsLayer = collider.physicsLayer;
                        box.collidesWithLayers = collider.collidesWithLayers;
                        box.isTrigger = collider.isTrigger;
                    }
                }
                else if (partiallyOffScreenRight)
                {
                    foreach (BoxCollider collider in regularX)
                    {
                        if (!dupedX.Any(c => c.name.Contains(collider.name)))
                        {
                            undupedX.Add(collider);
                        }
                    }
                    foreach (BoxCollider collider in dupedX)
                    {
                        var name = collider.name.Remove(collider.name.Length - XDupeCollider.Length);
                        var original = regularX.SingleOrDefault(c => c.name == name);
                        collider.localOffset = original.localOffset;
                        collider.localOffset -= new Vector2(screenSize.X, 0);
                    }
                    foreach (BoxCollider collider in undupedX)
                    {
                        var box = entity.addComponent(new BoxCollider(collider.absolutePosition.X, collider.absolutePosition.Y, collider.width, collider.height));
                        box.name = collider.name + XDupeCollider;
                        box.localOffset -= new Vector2(screenSize.X, 0);
                        box.physicsLayer = collider.physicsLayer;
                        box.collidesWithLayers = collider.collidesWithLayers;
                        box.isTrigger = collider.isTrigger;
                    }
                }
            }
            else
            {
                var dupedX = entity.getComponents<BoxCollider>().Where(c => c.name.Contains(XDupeCollider)).ToList();
                //clear duped colliders
                foreach (BoxCollider collider in dupedX)
                {
                    entity.removeComponent(collider);
                }
            }
            if (partiallyOffScreenTop || partiallyOffScreenBottom)
            {
                var colliders = entity.getComponents<BoxCollider>();
                var dupedY = colliders.Where(c => c.name.Contains(YDupeCollider)).ToList();
                var regularY = colliders.Where(c => !c.name.Contains(YDupeCollider)).ToList();
                var undupedY = new List<BoxCollider>();
                if (partiallyOffScreenTop)
                {
                    foreach (BoxCollider collider in regularY)
                    {
                        if (!dupedY.Any(c => c.name.Contains(collider.name)))
                        {
                            undupedY.Add(collider);
                        }
                    }
                    foreach (BoxCollider collider in dupedY)
                    {
                        var name = collider.name.Remove(collider.name.Length - YDupeCollider.Length);
                        var original = regularY.SingleOrDefault(c => c.name == name);
                        collider.localOffset = original.localOffset;
                        collider.localOffset += new Vector2(0, screenSize.Y);
                    }
                    foreach (BoxCollider collider in undupedY)
                    {
                        var box = entity.addComponent(new BoxCollider(collider.absolutePosition.X, collider.absolutePosition.Y, collider.width, collider.height));
                        box.name = collider.name + YDupeCollider;
                        box.localOffset += new Vector2(0, screenSize.Y);
                        box.physicsLayer = collider.physicsLayer;
                        box.collidesWithLayers = collider.collidesWithLayers;
                        box.isTrigger = collider.isTrigger;
                    }
                }
                else if (partiallyOffScreenBottom)
                {
                    foreach (BoxCollider collider in regularY)
                    {
                        if (!dupedY.Any(c => c.name.Contains(collider.name)))
                        {
                            undupedY.Add(collider);
                        }
                    }
                    foreach (BoxCollider collider in dupedY)
                    {
                        var name = collider.name.Remove(collider.name.Length - YDupeCollider.Length);
                        var original = regularY.SingleOrDefault(c => c.name == name);
                        collider.localOffset = original.localOffset;
                        collider.localOffset -= new Vector2(0, screenSize.Y);
                    }
                    foreach (BoxCollider collider in undupedY)
                    {
                        var box = entity.addComponent(new BoxCollider(collider.absolutePosition.X, collider.absolutePosition.Y, collider.width, collider.height));
                        box.name = collider.name + YDupeCollider;
                        box.localOffset -= new Vector2(0, screenSize.Y);
                        box.physicsLayer = collider.physicsLayer;
                        box.collidesWithLayers = collider.collidesWithLayers;
                        box.isTrigger = collider.isTrigger;
                    }
                }
            }
            else
            {

                var dupedY = entity.getComponents<BoxCollider>().Where(c => c.name.Contains(YDupeCollider)).ToList();
                //clear duped colliders
                foreach (BoxCollider collider in dupedY)
                {
                    entity.removeComponent(collider);
                }
            }
        }

        protected virtual void UpdateEntityComponents()
        {

        }
    }
}

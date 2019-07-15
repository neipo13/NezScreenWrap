using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace ScreenWrap
{
    public class ObjectFactory
    {
        public enum PlayerAnims
        {
            Normal,
            Attack
        }
        public static Entity CreatePlayer(Scene scene, int playerId, TiledObject spawnObj, TiledTileLayer collisionLayer)
        {
            var player = scene.addEntity(new Entity($"player{spawnObj.name}")).setPosition(spawnObj.position);
            var origin = new Vector2(8, 8);


            var texture = scene.content.Load<Texture2D>("samurai");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 16, 0, 4);
            subtextures[0].origin = new Vector2(8, 8);
            subtextures[1].origin = new Vector2(8, 8);
            subtextures[2].origin = new Vector2(8, 8);
            subtextures[3].origin = new Vector2(8, 8);
            var sprite = player.addComponent(new Sprite<PlayerAnims>(subtextures[2]));
            sprite.addAnimation(PlayerAnims.Normal, new SpriteAnimation(subtextures[0]));
            var attackAnim = new SpriteAnimation(subtextures.GetRange(0, 4));
            attackAnim.setLoop(false);
            sprite.addAnimation(PlayerAnims.Attack, attackAnim);
            sprite.play(PlayerAnims.Normal);
            player.addComponent(new Input(playerId));
            var box = player.addComponent(new BoxColliderFlip(-3, -6, 10, 14));
            box.name = "movebox";
            box.physicsLayer = 1 << 2;
            box.collidesWithLayers = 1 << 0;
            player.addComponent(new TiledMapMover(collisionLayer));
            var controller = player.addComponent<PlayerController>();

            player.addComponent(new ScreenWrapComponent(new Vector2(256, 144), new Vector2(16, 16), sprite));
            var hitbox = player.addComponent(new BoxColliderFlip(8, -8, 16, 16));
            hitbox.name = "hitbox";
            hitbox.physicsLayer = 1 << 3;
            hitbox.collidesWithLayers = 1 << 4;
            hitbox.active = false;
            hitbox.isTrigger = true;

            var hurtBox = player.addComponent(new BoxColliderFlip(-3, -6, 10, 14));
            hurtBox.name = "hurtbox";
            hurtBox.physicsLayer = 1 << 4;
            hurtBox.collidesWithLayers = 1 << 3;
            player.addComponent(new AttackController());
            player.addComponent<EnemyHit>();


            sprite.onAnimationCompletedEvent += (anim) =>
            {
                if (anim == PlayerAnims.Attack)
                {
                    hitbox.active = false;
                    sprite.play(PlayerAnims.Normal);
                }
            };
            return player;
        }
    }
}

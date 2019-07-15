using System;
using System.Linq;
using Nez;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Nez.Sprites;

namespace ScreenWrap
{
    public class DebugScene : Nez.Scene
    {
        protected int[] players;
        protected TiledMap tiledMap;
        protected TiledTileLayer collisionLayer;
        public DebugScene(int[] players)
        {
            this.players = players;
        }

        public override void initialize()
        {
            
            clearColor = Color.CornflowerBlue;
            var renderer = addRenderer(new DefaultRenderer());

            tiledMap = content.Load<TiledMap>("test");
            var tiledEntity = this.createEntity("debugMap");
            var tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledMap, "collision"));
            tiledMapComponent.setLayersToRender("collision");
            collisionLayer = tiledMapComponent.collisionLayer;
            var offscreenColliders = addEntity(new Entity());
            offscreenColliders.addComponent(new BoxCollider(-32, 112, 32, 32));
            offscreenColliders.addComponent(new BoxCollider(256, 112, 32, 32));
            


        }
        public override void onStart()
        {
            base.onStart();
            var spawns = tiledMap.getObjectGroup("spawn");
            for (var i = 0; i < spawns.objects.Length; i++)
            {
                string playerIdStr;
                spawns.objects[i].properties.TryGetValue("player_id", out playerIdStr);
                int playerId = int.Parse(playerIdStr);
                if (players.contains(playerId))
                {
                    ObjectFactory.CreatePlayer(this, playerId, spawns.objects[i], collisionLayer);
                }
            }
        }

    }
}

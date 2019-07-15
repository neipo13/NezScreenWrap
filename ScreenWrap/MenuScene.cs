using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Nez.Sprites;
using Nez.UI;
using Microsoft.Xna.Framework;
using Nez.UI.Widgets;

namespace ScreenWrap 
{
    public class MenuScene : Scene
    {
        Entity entity;
        public override void initialize()
        {
            clearColor = Color.Black;
            var renderer = addRenderer(new DefaultRenderer());

            entity = addEntity(new Entity());
            var canvas = entity.addComponent(new UICanvas());
            canvas.setRenderLayer(-10);
            var textWidth = 256;
            var table = canvas.stage.addElement(new Table())
                .setFillParent(true);
            table.setBounds(1, 1, textWidth-2, 142);
            var style = new LabelStyle(Color.White);
            var textLabel = new EffectLabel("THE /1ONLY/1 SAMURAI", style).setAlignment(Align.center);
            var pressStart = new EffectLabel("/2Press Start/2", style).setAlignment(Align.center);

            textLabel
                .setFontScale(2f)
                .setAlignment(Align.center);
            pressStart
                .setAlignment(Align.center);

            var cell = table.add(textLabel)
                .setAlign(Align.center);
            table.row().height(50);
            table.add(pressStart).setAlign(Align.bottom).setFillX();

            entity.addComponent(new Input(0));
            entity.addComponent(new Input(1));
            entity.addComponent(new Input(2));
            entity.addComponent(new Input(3));
        }

        public override void update()
        {
            base.update();
            var components = entity.getComponents<Input>();
            var transitioning = false;
            for (var i = 0; i < components.Count; i++)
            {
                if(components[i].Button1Input.isPressed || components[i].Button5Input.isPressed)
                {
                    transitioning = true;
                }
            }
            if (transitioning)
            {
                Core.startSceneTransition(new FadeTransition(() => new PlayerSelectScene()));
            }
        }
    }
}

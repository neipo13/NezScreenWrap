using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez.UI;
using Nez.UI.Widgets;

namespace ScreenWrap
{
    public class PlayerSelectScene : Scene
    {
        protected Input[] input = new Input[4];
        protected bool[] playersReady = new bool[4] { false, false, false, false };
        protected EffectLabel[] labels = new EffectLabel[4];
        protected bool multiplePlayersReady => playersReady.Where(b => b).Count() > 1;
        const string Join = "JOIN";
        const string Ready = "/4READY/4";
        public override void initialize()
        {
            clearColor = Color.Black;
            var renderer = addRenderer(new DefaultRenderer());

            var entity = addEntity(new Entity());

            var canvas = entity.addComponent(new UICanvas());
            canvas.setRenderLayer(-10);
            var textWidth = 256;
            var table = canvas
                .stage
                .addElement(new Table())
                .setFillParent(false)
                .left()
                .top();
            table.setBounds(0, 0, textWidth, 144);


            var style = new LabelStyle(Color.White);

            labels[0] = new EffectLabel(Join, style).setAlignment(Align.center).setWrap(true);
            labels[1] = new EffectLabel(Join, style).setAlignment(Align.center).setWrap(true);
            labels[2] = new EffectLabel(Join, style).setAlignment(Align.center).setWrap(true);
            labels[3] = new EffectLabel(Join, style).setAlignment(Align.center).setWrap(true);
            labels[0].setWidth(textWidth / 4);
            labels[1].setWidth(textWidth / 4);
            labels[2].setWidth(textWidth / 4);
            labels[3].setWidth(textWidth / 4);

            canvas.stage.addElement(labels[0]).setX(textWidth / 4 - textWidth / 8).setY(144/4);
            canvas.stage.addElement(labels[1]).setX(textWidth / 4 * 3 - textWidth / 8).setY(144 / 4);
            canvas.stage.addElement(labels[2]).setX(textWidth / 4 - textWidth / 8).setY(144 / 4 * 3);
            canvas.stage.addElement(labels[3]).setX(textWidth / 4 * 3 - textWidth / 8).setY(144 / 4 * 3);


            input[0] = entity.addComponent(new Input(0));
            input[1] = entity.addComponent(new Input(1));
            input[2] = entity.addComponent(new Input(2));
            input[3] = entity.addComponent(new Input(3));

            input[0].SetupInput();
            input[1].SetupInput();
            input[2].SetupInput();
            input[3].SetupInput();
        }

        public override void update()
        {
            base.update();
            var transition = false;
            
            if (input[0].Button2Input.isPressed && playersReady[0]) playersReady[0] = false;
            if (input[1].Button2Input.isPressed && playersReady[1]) playersReady[1] = false;
            if (input[2].Button2Input.isPressed && playersReady[2]) playersReady[2] = false;
            if (input[3].Button2Input.isPressed && playersReady[3]) playersReady[3] = false;

            //check for input from any input mapping
            var p1 = input[0].Button1Input.isPressed || input[0].Button5Input.isPressed;
            var p2 = input[1].Button1Input.isPressed || input[1].Button5Input.isPressed;
            var p3 = input[2].Button1Input.isPressed || input[2].Button5Input.isPressed;
            var p4 = input[3].Button1Input.isPressed || input[3].Button5Input.isPressed;
            if (p1)
            {
                if (multiplePlayersReady && playersReady[0])
                    transition = true;
                else
                    playersReady[0] = true;
            }
            if (p2)
            {
                if (multiplePlayersReady && playersReady[1])
                    transition = true;
                else
                    playersReady[1] = true;
            }
            if (p3)
            {
                if (multiplePlayersReady && playersReady[2])
                    transition = true;
                else
                    playersReady[2] = true;
            }
            if (p4)
            {
                if (multiplePlayersReady && playersReady[3])
                    transition = true;
                else
                    playersReady[3] = true;
            }

            if (transition)
                start();

            updateText();
        }

        public void start()
        {
            var players = new List<int>();
            for(var i = 0; i < playersReady.Length; i++)
            {
                if (playersReady[i])
                {
                    players.Add(i);
                }
            }
            Core.startSceneTransition(new FadeTransition(() => new DebugScene(players.ToArray<int>())));
        }

        public void updateText()
        {
            for(var i = 0; i < 4; i++)
            {
                labels[i].setText(playersReady[i] ? Ready : Join);
            }
        }
    }
}

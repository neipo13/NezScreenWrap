using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace ScreenWrap
{
    public class Input : Nez.Component
    {
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;
        VirtualButton _button1;
        VirtualButton _button2;
        VirtualButton _button3;
        VirtualButton _button4;
        VirtualButton _button5;
        Vector2 _axialInput; //utility vec2 to hold input values without constantly creating/destroying vec2s
        int gamepadIndex = 0;
        InputMapping mapping;
        public Vector2 axialInput
        {
            get
            {
                _axialInput.X = _xAxisInput.value;
                _axialInput.Y = _yAxisInput.value;
                return _axialInput;
            }
        }
        public Vector2 onlyXInput
        {
            get
            {
                _axialInput.X = _xAxisInput.value;
                _axialInput.Y = 0;
                return _axialInput;
            }
        }
        public Vector2 onlyYInput
        {
            get
            {
                _axialInput.X = 0;
                _axialInput.Y = _yAxisInput.value;
                return _axialInput;
            }
        }

        public VirtualButton Button1Input
        {
            get
            {
                return _button1;
            }
        }
        public VirtualButton Button2Input
        {
            get
            {
                return _button2;
            }
        }
        public VirtualButton Button3Input
        {
            get
            {
                return _button3;
            }
        }
        public VirtualButton Button4Input
        {
            get
            {
                return _button4;
            }
        }
        public VirtualButton Button5Input
        {
            get
            {
                return _button5;
            }
        }

        public Input(int index)
        {
            this.gamepadIndex = index;
            using (StreamReader reader = new StreamReader("input.json"))
            {
                string json = reader.ReadToEnd();
                mapping = new JavaScriptSerializer().Deserialize<List<InputMapping>>(json).Single(m => m.index == index);
            }
        }

        public Input(InputMapping mapping)
        {
            gamepadIndex = mapping.index;
        }

        /// <summary>
        /// Needs a better way to bind keys, just hard bind for now
        /// </summary>
        public void SetupInput()
        {
            _axialInput = Vector2.Zero;
            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadDpadLeftRight(gamepadIndex));
            _xAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadLeftStickX(gamepadIndex));
            _xAxisInput.nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Left, (Keys)mapping.Right));

            // vertical input from dpad, left stick or keyboard up/down
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadDpadUpDown(gamepadIndex));
            _yAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadLeftStickY(gamepadIndex));
            _yAxisInput.nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Up, (Keys)mapping.Down));

            //action buttons
            _button1 = new VirtualButton();
            _button1.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)mapping.key1));
            _button1.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)mapping.button1));


            _button2 = new VirtualButton();
            _button2.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)mapping.key2));
            _button2.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)mapping.button2));


            _button3 = new VirtualButton();
            _button3.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)mapping.key3));
            _button3.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)mapping.button3));

            _button4 = new VirtualButton();
            _button4.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)mapping.key4));
            _button4.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)mapping.button4));

            _button5 = new VirtualButton();
            _button5.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)mapping.key5));
            _button5.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)mapping.button5));


        }

        public override void onAddedToEntity()
        {
            SetupInput();
        }
    }
}

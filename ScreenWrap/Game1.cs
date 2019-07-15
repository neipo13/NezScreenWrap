using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;

namespace ScreenWrap
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Nez.Core
    {
        Scene.SceneResolutionPolicy policy;
        public Game() : base(256 * 4, 144 * 4, windowTitle: "Screenwrap")
        {
            policy = Scene.SceneResolutionPolicy.BestFit;
            Scene.setDefaultDesignResolution(256, 144, policy, 0, 0);
            Window.AllowUserResizing = true;

            Window.Position = new Point(0, 0);
            DebugConsole.renderScale = 4;

        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            scene = Scene.createWithDefaultRenderer();
            base.Update(new GameTime());
            base.Draw(new GameTime());
            Nez.Input.maxSupportedGamePads = 4;
            var s = new MenuScene();//new DebugScene(new int[2] { 0, 1 })
            scene = s;
        }
    }
}

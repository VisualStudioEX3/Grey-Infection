using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class MainWindowFrame : Entity
    {
        private const string titleString = "joss@UCS: /etc/vconn/modules/m_N.sh$";

        private Bitmap window;
        private TextLabel title;

        public override void Initialize()
        {
            base.Initialize();
            Manager.Graphics.LoadTexture(@"GameUI\mainWindow");
            window = new Bitmap(@"GameUI\mainWindow");
            window.Fixed = true;
            window.Location = new Vector2(1, 0);

            Manager.Graphics.LoadFont(@"Fonts\androidnation14");
            title = new TextLabel(@"Fonts\androidnation14");
            title.Text = "joss@UCS: /etc/vconn/modules/m_#[" + Manager.Vars["currentLevel"].ToString() + "].sh$";
            title.Color = Color.Wheat;
            title.Location = new Vector2(32, -29);

            base.ZOrder = -999999;
        }

        public override void Draw(GameTime gameTime)
        {
            window.Draw(gameTime);
            title.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}

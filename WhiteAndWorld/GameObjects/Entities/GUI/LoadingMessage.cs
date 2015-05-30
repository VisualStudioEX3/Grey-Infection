using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Graphics.Primitives;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;


namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class LoadingMessage : Entity
    {
        private Sprite loading;

        public override void Initialize()
        {
            base.Initialize();

            loading = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Loading"));
            loading.Center = true;
            loading.Fixed = true;
            loading.Location = Helper.PointToVector2(Manager.Graphics.ScreenSafeArea.Center);

            this.Priority = 99999;
            this.ZOrder = -99999;
        }

        public override void Draw(GameTime gameTime)
        {
            loading.Draw(gameTime);
        }
    }
}

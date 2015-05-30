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
using TLSA.Engine.Graphics.Primitives;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.Options;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class BackBox : Entity
    {
        private RectangleOverlay backbox;

        public override void Initialize()
        {
            base.Initialize();

            backbox = new RectangleOverlay(Manager.Graphics.ScreenBounds);
            backbox.BackColor = new Color(0, 0, 0, 0.8f);
            backbox.ForeColor = new Color(0, 0, 0, 0.8f);
            backbox.Fixed = true;
        }

        public override void Update(GameTime gameTime)
        {
            backbox.UpdateRectangle();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            backbox.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}

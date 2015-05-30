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

namespace WhiteAndWorld.GameObjects.Entities
{
    public class Fog : Entity
    {
        private Bitmap fog;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture("backgroundFog");
            fog = new Bitmap("backgroundFog");
            fog.Size = new Vector2(Manager.Graphics.ScreenBounds.Width, Manager.Graphics.ScreenBounds.Height);
            fog.Fixed = true;
        }

        public override void Draw(GameTime gameTime)
        {
            fog.Draw(gameTime);
        }
    }
}
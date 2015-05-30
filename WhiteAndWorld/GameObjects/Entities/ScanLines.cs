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
    public class ScanLines : Entity
    {
        Bitmap background;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture("scanline");
            background = new Bitmap("scanline");
            background.Fixed = true;
            background.Color = new Color(255, 255, 255, 128);
            background.Size = new Vector2(1280 * 2, 720 * 2);

            base.ZOrder = -999999;
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw(gameTime);
        }
    }
}
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
    public class Background : Entity
    {
        private Bitmap background;
        
        public override void Initialize()
        {
            base.Initialize();

            string texture = @"Backgrounds\level_" + Manager.Vars["currentLevel"].ToString();
            Manager.Graphics.LoadTexture(texture);
            background = new Bitmap(texture);
                        
            base.ZOrder = 999;
        }
     
        public override void Draw(GameTime gameTime)
        {
            background.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
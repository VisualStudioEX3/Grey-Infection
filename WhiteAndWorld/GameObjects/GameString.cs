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
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.MainMenu;

namespace WhiteAndWorld.GameObjects
{
    public class GameString : TLSA.Engine.IDrawable
    {
        private TextLabel label, shadow;

        public GameString()
        {
        }

        private Vector2 location = Vector2.Zero;
        public Vector2 Location 
        {
            get { return location; }
            set
            {
                location = value;
                if (label != null)
                {
                    label.Location = value;
                    shadow.Location = value - new Vector2(2, 2);
                }
            }
        }



        public bool Visible { get; set; }

        public void Draw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            label.Draw(gameTime);
        }
    }
}
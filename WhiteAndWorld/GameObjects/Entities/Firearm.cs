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
    public class Firearm : Entity
    {
        private Sprite sparks;

        public override Vector2 Location
        {
            get
            {
                return sparks != null ? sparks.Location : Vector2.Zero;
            }
            set
            {
                if (sparks != null) sparks.Location = value;
            }
        }

        public SpriteEffects Mirror
        {
            get { return sparks.Mirror; }
            set { sparks.Mirror = value; }
        }


        public override void Initialize()
        {
            base.Initialize();
            sparks = new Sprite(Manager.Graphics.LoadTexture("firearm"));

            sparks.Animations.AddSecuence("fire", new Rectangle(0, 0, 32, 32), 5, 100, false);
            sparks.Animations.Play("fire");

            sparks.Update(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            if (sparks.Animations.IsEnded) Kill();
            sparks.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sparks.Draw(gameTime);
        }
    }
}
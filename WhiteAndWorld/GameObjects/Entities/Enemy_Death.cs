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
    public class Enemy_Death : Entity
    {
        private Sprite sparks;
        private SoundPlayer death;

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

        public override void Initialize()
        {
            base.Initialize();
            sparks = new Sprite(Manager.Graphics.LoadTexture("enemy_death"));

            sparks.Animations.AddSecuence("death", new Rectangle(0, 0, 110, 110), 20, 32, false);
            sparks.Animations.Play("death");
            sparks.OffSet = new Vector2(40, 40);

            death = new SoundPlayer(@"Audio\FX\enemy_death");
            death.Play();

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

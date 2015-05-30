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
    public class TextButton : ButtonBase
    {
        private TextLabel caption, shadow;

        public override string Caption
        {
            get { return base.Caption; }
            set
            {
                base.Caption = value;

                if (caption != null)
                {
                    caption.Text = value;
                    shadow.Text = value;
                }
            }
        }

        public override Vector2 Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                if (caption != null)
                {
                    caption.Location = value;
                    shadow.Location = new Vector2(value.X + 2, value.Y + 2);
                }
            }
        }

        public TextButton()
        {
            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            caption = new TextLabel(@"Fonts\androidNation14");

            shadow = new TextLabel(@"Fonts\androidNation14");
            shadow.Color = Color.Black;

            Update(Manager.GameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Selected)
            {
                caption.Scale = 1.3f;
                shadow.Scale = 1.3f;

                caption.Color = Color.Yellow;
                shadow.Color = Color.Black;
            }
            else
            {
                caption.Scale = 1;
                shadow.Scale = 1;

                caption.Color = new Color(1, 1, 1, 0.5f);
                shadow.Color = new Color(0, 0, 0, 0.5f);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            caption.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
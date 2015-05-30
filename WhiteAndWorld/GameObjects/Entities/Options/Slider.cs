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

namespace WhiteAndWorld.GameObjects.Entities.Options
{
    public class Slider : Entity 
    {
        private Sprite arrow_left, arrow_right;
        private TextLabel sliderValue, sliderCaption;

        public int Value { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }

        public override Vector2 Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;

                if (base.IsInitialized)
                {
                    sliderCaption.Location = value;
                    arrow_left.Location = new Vector2(value.X + 300, value.Y);
                    sliderValue.Location = new Vector2(value.X + 340, value.Y);
                    arrow_right.Location = new Vector2(value.X + 400, value.Y);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            arrow_left = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Options\slider_arrow"));
            arrow_right = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Options\slider_arrow"));
            arrow_right.Mirror = SpriteEffects.FlipHorizontally;

            Manager.Graphics.LoadFont(@"Fonts\font24");

            sliderValue = new TextLabel(@"Fonts\font24");
            sliderValue.Color = Color.Black;

            sliderCaption = new TextLabel(@"Fonts\font24");
            sliderCaption.Color = Color.Black;
            sliderCaption.Text = "Caption";

            Value = 0;
            Max = 10;
            Min = 0;

            ZOrder = -32;
        }

        public override void Update(GameTime gameTime)
        {
            sliderValue.Text = string.Format("{0,2}", sliderValue);

        }
    }
}

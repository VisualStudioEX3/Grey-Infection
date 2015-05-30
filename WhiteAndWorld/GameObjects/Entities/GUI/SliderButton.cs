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
    public class SliderButton : ButtonBase
    {
        private Sprite slider, sliderShadow;
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
                    slider.Location = new Vector2(value.X + 524, value.Y - 4);
                    sliderShadow.Location = new Vector2(value.X + 524 + 2, value.Y - 4 + 2);
                }
            }
        }

        public SliderButton()
        {
            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            caption = new TextLabel(@"Fonts\androidNation14");

            shadow = new TextLabel(@"Fonts\androidNation14");
            shadow.Color = Color.Black;

            slider = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Options\slider"));
            for (int i = 0; i < 11; i++)
                slider.Animations.AddSecuence(i.ToString(), new Rectangle(0, i * 23, 116, 23), 1, 0, false);
            slider.Animations.Play("10");
            slider.OffSet = new Vector2(slider.Texture.Bounds.Width, 0);
            slider.Update(new GameTime());
            
            sliderShadow = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Options\slider"));
            for (int i = 0; i < 11; i++)
                sliderShadow.Animations.AddSecuence(i.ToString(), new Rectangle(0, i * 23, 116, 23), 1, 0, false);
            sliderShadow.Animations.Play("10");
            sliderShadow.OffSet = new Vector2(sliderShadow.Texture.Bounds.Width, 0);
            sliderShadow.Color = Color.Black;
            sliderShadow.Update(new GameTime());

            OnChange += OnChangeValue;

            Update(Manager.GameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Selected)
            {
                caption.Scale = 1.3f;
                shadow.Scale = 1.3f;
                slider.Scale = 1.3f;
                sliderShadow.Scale = 1.3f;

                caption.Color = Color.Yellow;
                shadow.Color = Color.Black;
                sliderShadow.Color = Color.Black;
            }
            else
            {
                caption.Scale = 1;
                shadow.Scale = 1;
                slider.Scale = 1;
                sliderShadow.Scale = 1;

                caption.Color = new Color(1, 1, 1, 0.5f);
                shadow.Color = new Color(0, 0, 0, 0.5f);
                sliderShadow.Color = new Color(0, 0, 0, 0.7f);
            }

            slider.Update(gameTime);
            sliderShadow.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            sliderShadow.Draw(gameTime);

            caption.Draw(gameTime);            
            slider.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void OnChangeValue(ButtonGroup group, int value)
        {
            slider.Animations.Play(value.ToString());
            sliderShadow.Animations.Play(value.ToString());
        }
    }
}
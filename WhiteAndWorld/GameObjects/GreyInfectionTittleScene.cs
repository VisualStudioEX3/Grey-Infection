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

namespace WhiteAndWorld.GameObjects.States
{
    public class GreyInfectiontitleScene : Entity
    {
        private Bitmap background;
        private Bitmap fog;
        private Sprite layer1, layer2;  // Scroll
        private Bitmap wall;
        private Sprite joss;
        private Bitmap shine;
        private Sprite fan;
        private Sprite lightLamp;
        private Sprite logo;

        private TextLabel footstep, shadow;

        private GlitchColor glitch = new GlitchColor();

        private Timer timer;
        private int flag = 0;

        public bool FootstepIsVisible 
        {
            get { return footstep.Visible; }
            set { footstep.Visible = true; shadow.Visible = true; } 
        }

        public bool EnableGlitchColor { get { return glitch != null ? glitch.Enabled : false; } set { if (glitch != null) glitch.Enabled = value; } }

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture("theGrid");
            background = new Bitmap("theGrid");
            background.Size = new Vector2(1280, 720);
            background.Fixed = true;

            Manager.Graphics.LoadTexture("backgroundFog");
            fog = new Bitmap("backgroundFog");
            fog.Size = new Vector2(1280, 720);
            fog.Fixed = true;

            layer1 = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\backgroundLayer1"));
            layer1.Fixed = true;

            layer2 = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\backgroundLayer1"));
            layer2.Location = new Vector2(1280, 0);
            layer2.Visible = false;
            layer2.Fixed = true;

            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\wall");
            wall = new Bitmap(@"GameUI\Gametitle\wall");
            wall.Fixed = true;

            fan = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\fan"));
            fan.Location = new Vector2(-25, -100);
            fan.Animations.AddSecuence("default", new Rectangle(0, 0, 400, 400), 4, 64, true);
            fan.Animations.Play("default");
            fan.Fixed = true;
            fan.Update(new GameTime());

            lightLamp = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\lightLamp"));
            lightLamp.Location = new Vector2(-24, 400);
            lightLamp.Fixed = true;

            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\joss");
            joss = new Sprite(@"GameUI\Gametitle\joss");
            joss.Fixed = true;

            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\shine");
            shine = new Bitmap(@"GameUI\Gametitle\shine");
            shine.Fixed = true;

            logo = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\logo"));
            logo.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Right - 500, 60);
            logo.Scale = 0.6f;
            logo.Fixed = true;

            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            footstep = new TextLabel(@"Fonts\androidNation14");
            footstep.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            footstep.Text = Session.Strings["copyright"];
            footstep.Visible = false;
            footstep.Center = true;
            footstep.Fixed = true;

            shadow = new TextLabel(@"Fonts\androidNation14");
            shadow.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X + 2, Manager.Graphics.ScreenSafeArea.Bottom - 14 + 2);
            shadow.Text = Session.Strings["copyright"];
            shadow.Color = Color.Black;
            shadow.Visible = false;
            shadow.Center = true;
            shadow.Fixed = true;

            timer = new Timer();

            Manager.Scene.AddEntity(glitch);

            ZOrder = 9999;
        }

        public override void Update(GameTime gameTime)
        {
            if (flag == 0 && timer.Value > new Random().Next(1500, 5000))
            {
                timer.Reset();
                flag = 1;
                lightLamp.Color = Color.Gray;
            }
            else if (flag == 1 && timer.Value > 50)
            {
                timer.Reset();
                flag = 0;
                lightLamp.Color = Color.White;
            }

            fan.Update(gameTime);
                        
            layer1.Location -= new Vector2(1, 0);
            layer2.Location -= new Vector2(1, 0);
            layer2.Visible = true;
            if (layer1.Location.X == -1280) layer1.Location = new Vector2(1280, 0);
            if (layer2.Location.X == -1280) layer2.Location = new Vector2(1280, 0);
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw(gameTime);

            layer1.Draw(gameTime);
            layer2.Draw(gameTime);

            fog.Draw(gameTime);

            wall.Draw(gameTime);
            fan.Draw(gameTime);

            lightLamp.Location = new Vector2(-24, 400); lightLamp.Draw(gameTime);
            lightLamp.Location = new Vector2(250, 400); lightLamp.Draw(gameTime);
            lightLamp.Location = new Vector2(325, 400); lightLamp.Draw(gameTime);

            joss.Draw(gameTime);
            shine.Draw(gameTime);

            logo.Draw(gameTime);

            shadow.Draw(gameTime);
            footstep.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
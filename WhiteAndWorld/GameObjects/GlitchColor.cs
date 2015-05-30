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
    public class GlitchColor : Entity
    {
        private Bitmap glitch;
        private Timer timer;
        private int step = 0;
        private bool force;

        private SoundPlayer glitchSound;

        private bool _manual;
        public bool Manual 
        {
            get { return _manual; }
            set 
            { 
                _manual = value;
                step = value ? -1 : 0;
            } 
        }

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture("glitch");
            glitch = new Bitmap("glitch");
            glitch.BlendingEffect = BlendFilters.XOR;
            glitch.Visible = false;
            glitch.Fixed = true;

            glitchSound = new SoundPlayer(@"Audio\FX\glitch");

            timer = new Timer();

            force = false;
            Manual = false;

            ZOrder = -1999999999;
        }

        public override void Update(GameTime gameTime)
        {
            if (Manual)
            {
                if (step == 0 && timer.Value >= 25)
                {
                    timer.Reset();
                    Manager.Graphics.EndShader();
                    glitchSound.Play();
                    glitch.Visible = true;
                    step++;
                }
                else if (step == 1 && timer.Value >= 25)
                {
                    timer.Reset();
                    glitch.Visible = false;
                    step++;
                }
                else if (step == 2 && timer.Value >= 25)
                {
                    force = false;
                    timer.Reset();
                    Manager.Graphics.BeginShader();
                    glitchSound.Play();
                    glitch.Visible = true;
                    step++;
                }
                else if (step == 3 && timer.Value >= 25)
                {
                    timer.Reset();
                    Manager.Graphics.BeginShader();
                    glitch.Visible = false;
                    step++;
                }
            }
            else
            {
                if (step == 0 && timer.Value >= 5000)
                {
                    timer.Reset();
                    Manager.Graphics.EndShader();
                    glitchSound.Play();
                    glitch.Visible = true;
                    step++;
                }
                else if (step == 1 && timer.Value >= 50)
                {
                    timer.Reset();
                    glitch.Visible = false;
                    step++;
                }
                else if (step == 2 && (timer.Value >= 1500 || force))
                {
                    force = false;
                    timer.Reset();
                    Manager.Graphics.BeginShader();
                    glitchSound.Play();
                    glitch.Visible = true;
                    step++;
                }
                else if (step == 3 && timer.Value >= 50)
                {
                    timer.Reset();
                    Manager.Graphics.BeginShader();
                    glitch.Visible = false;
                    step = 0;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            glitch.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void Now()
        {
            if (Manual)
            {
                step = 0;
            }
            else
            {
                step = 2;
                force = true;
            }
        }
    }
}

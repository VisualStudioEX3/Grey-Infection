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
using TLSA.Engine.Graphics.Primitives;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities.GUI;

namespace WhiteAndWorld.GameObjects.Entities.Intro
{
    public class IntroBox : Entity
    {
        private Label text;
        private string contentText;
        private Sprite box;
        private Timer timer;

        private int currentChar;
        private float alpha;

        private SoundPlayer[] typing;

        public delegate void OnEndTyppingEvent();

        public OnEndTyppingEvent OnEndTypping;

        private int _index = 0;
        public int Index 
        {
            get { return _index; }
            set
            {
                _index = value;

                if (box != null)
                {
                    box.Texture = Manager.Graphics.LoadTexture(@"Intro\intro" + value);
                    contentText = Session.Strings["intro_0" + value];

                    switch (value)
                    {
                        case 1:
                            box.Location = new Vector2(0, 0);
                            text.Location = new Vector2(384, 36);
                            break;
                        case 2:
                            box.Location = new Vector2(0, 180);
                            text.Location = new Vector2(26, 180 + 36);
                            break;
                        case 3:
                            box.Location = new Vector2(0, 360);
                            text.Location = new Vector2(384, 360 + 36);
                            break;
                    }
                    timer.Reset();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            text = new Label(); 
            text.Initialize();
            text.SmallFont = true;
            text.Fixed = false;
            text.Scale = 0.8f;

            box = new Sprite();
            box.Color = Color.Transparent;

            currentChar = 0;
            alpha = 0;

            timer = new Timer();

            typing = new SoundPlayer[4];
            for (int i = 0; i < 4; i++)
                typing[i] = new SoundPlayer(@"Audio\FX\typing0" + i);

            ZOrder = -999999999;

            Enabled = false;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (Manager.UIInput.Hit("jump"))
            {
                currentChar = contentText.Length - 1;
                box.Color = Color.White;
            }

            if (timer.Value > (alpha == 1 ? 50 : 1))
            {
                if (alpha == 1)
                {
                    timer.Reset();
                    text.Caption = contentText.Substring(0, currentChar) + (currentChar % 2 == 0 && currentChar < contentText.Length ? "|" : "");
                    currentChar++;
                    typing[MathTools.RandomNumberGenerator.Next(0, 3)].Play();
                    if (currentChar > contentText.Length)
                    {
                        if (OnEndTypping != null) OnEndTypping();
                        Enabled = false;
                    }
                }
                else
                {
                    timer.Reset();
                    alpha += 0.05f;
                    if (alpha > 1) 
                        alpha = 1;
                    else
                        box.Color = new Color(1, 1, 1, alpha);
                } 
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            box.Draw(gameTime);
            text.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

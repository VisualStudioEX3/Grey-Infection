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
using WhiteAndWorld.GameDefinitions;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.Intro;
using WhiteAndWorld.GameObjects.Entities.Options;

namespace WhiteAndWorld.GameObjects.States
{
    public class Intro : StateComponent
    {        
        private Timer timer;
        private IntroBox intro1, intro2, intro3;
        private bool showIntro, showGameInput;
        
        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            MusicPlayer.Play(MusicPlayer.LevelTheme);

            Manager.Graphics.OffSet = new Vector2(Manager.Graphics.ScreenSafeArea.X, Manager.Graphics.ScreenSafeArea.Y);

            Manager.Graphics.BeginShader();

            GreyInfectiontitleScene background = new GreyInfectiontitleScene();
            Manager.Scene.AddEntity(background);
            background.EnableGlitchColor = false;

            Manager.Scene.AddEntity(new BackBox());

            intro1 = new IntroBox();
            Manager.Scene.AddEntity(intro1);
            intro1.Index = 1;
            intro1.OnEndTypping = OnEndIntro1;
            intro1.Enabled = true;
            intro1.Visible = true;

            intro2 = new IntroBox();
            Manager.Scene.AddEntity(intro2);
            intro2.Index = 2;
            intro2.OnEndTypping = OnEndIntro2;

            intro3 = new IntroBox();
            Manager.Scene.AddEntity(intro3);
            intro3.Index = 3;
            intro3.OnEndTypping = OnEndIntro3;

            // Pistas:
            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_continue"];
            inputHint.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X - 40, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            inputHint.ZOrder = -9999999;

            showIntro = false;
            showGameInput = false;

            timer = new Timer();
        }
                
        public override void Update(GameTime gameTime)
        {
            if (showIntro && Manager.UIInput.Hit("jump"))
            {
                if (!showGameInput)
                {
                    GameInputMap controls = new GameInputMap();
                    Manager.Scene.AddEntity(controls);
                    controls.DisableBackground = true;
                    controls.DisableHint = true;

                    intro1.Visible = false;
                    intro2.Visible = false;
                    intro3.Visible = false;

                    showGameInput = true;
                }
                else
                {
                    Manager.Vars["currentLevel"] = 1;
                    Manager.Vars["CMYK_Ammo"] = 0;
                    Manager.Vars["score"] = 0;
                    ResetLevel reset = new ResetLevel();
                    Manager.Scene.AddEntity(reset);
                    reset.Inmediate = true;
                }
            }
        }

        private void OnEndIntro1()
        {
            intro2.Enabled = true;
            intro2.Visible = true;
        }

        private void OnEndIntro2()
        {
            intro3.Enabled = true;
            intro3.Visible = true;
        }

        private void OnEndIntro3()
        {
            showIntro = true;
        }
    }
}
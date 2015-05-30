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
using WhiteAndWorld.GameDefinitions;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.MainMenu;
using WhiteAndWorld.GameObjects.Entities.EndGame;

namespace WhiteAndWorld.GameObjects.States
{
    public class GameOver : StateComponent
    {        
        private Timer timer;
        private ConditionAlert alert;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            MusicPlayer.Play(MusicPlayer.MenuTheme);

            Manager.Graphics.OffSet = Vector2.Zero;
            Manager.Graphics.EndShader();

            GreyInfectiontitleScene background = new GreyInfectiontitleScene();
            Manager.Scene.AddEntity(background);
            background.EnableGlitchColor = false;

            Fog fog = new Fog();
            Manager.Scene.AddEntity(fog);
            fog.ZOrder = -9999998;

            BackBox black = new BackBox();
            Manager.Scene.AddEntity(black);
            black.ZOrder = -9999997;

            alert = new ConditionAlert();
            Manager.Scene.AddEntity(alert);
            alert.EndEvent += ExitAlert;

            // Indicaciones de input:
            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_continue"];
            inputHint.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X - 64, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            inputHint.ZOrder = -999999999;
        }
               
        private void ExitAlert()
        {
            Manager.Scene.AddEntity(new WarningBox());
        }
    }
}
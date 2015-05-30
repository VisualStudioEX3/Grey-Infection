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
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;

namespace WhiteAndWorld.GameObjects.States
{
    public class LevelSelection : StateComponent
    {
        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            // Escena de fondo:
            Manager.Scene.AddEntity(new GreyInfectiontitleScene());

            // Sombreamos el fondo:
            Manager.Scene.AddEntity(new BackBox());

            // Titulo:
            Label title = new Label();
            Manager.Scene.AddEntity(title);
            title.Caption = Session.Strings["levmenu_title"];
            title.Location = new Vector2(Manager.Graphics.ScreenSafeArea.X, Manager.Graphics.ScreenSafeArea.Y + 100);
            
            // Rejilla de niveles:
            GridButtonGroup grid = new GridButtonGroup();
            grid.Columns = 8;
            grid.Rows = 4;
            grid.ButtonWidth = 136;
            grid.ButtonHeight = 86;
            grid.Location = new Vector2(Manager.Graphics.ScreenSafeArea.X + 30, Manager.Graphics.ScreenSafeArea.Y + 200);
            grid.OnExit += OnExitState;
            Manager.Scene.AddEntity(grid);

            LevelSelectionButton button;
            for (int i = 0; i < 32; i++)
            {
                button = new LevelSelectionButton();
                button.ConfirmFXWaitToEnd = true;
                grid.Add(button);
                button.OnGridEnter += OnEnterLevel;
            }

            // Indicaciones de input:
            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_select"];
            inputHint.Location = new Vector2(340, Manager.Graphics.ScreenSafeArea.Bottom - 42);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.B;
            inputHint.Caption = Session.Strings["button_back"];
            inputHint.Location = new Vector2(600, Manager.Graphics.ScreenSafeArea.Bottom - 42);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.LeftThumb;
            inputHint.Caption = Session.Strings["button_move"];
            inputHint.Location = new Vector2(850, Manager.Graphics.ScreenSafeArea.Bottom - 42);
        }

        public void OnEnterLevel(GridButtonGroup group)
        {
            if (group.Selected.Level == 1)
            {
                Manager.GameStates.ChangeState("intro");
            }
            else
            {
                Session.GameProgress = StorageSession.LoadData<GameDefinitions.SaveGame>(Session.SaveGameFilename);

                Manager.Vars["currentLevel"] = group.Selected.Level;
                Manager.Vars["CMYK_Ammo"] = Session.GameProgress.CurrentCMYKAmmo;
                Manager.Vars["score"] = Session.GameProgress.CurrentScore;
                ResetLevel reset = new ResetLevel();
                Manager.Scene.AddEntity(reset);
                reset.Inmediate = true;
            }
        }

        public void OnExitState(GridButtonGroup group)
        {
            Manager.GameStates.ChangeState("menu");
        }
    }
}

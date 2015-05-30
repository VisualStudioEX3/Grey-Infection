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
using WhiteAndWorld.GameObjects.Entities;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class PauseMenu : Entity
    {
        ButtonGroup menu;
        BackBox background;
        Label title;
        List<InputHintLabel> hints;

        public override void Initialize()
        {
            base.Initialize();

            background = new BackBox();
            Manager.Scene.AddEntity(background);
            background.ZOrder = -1000000;

            title = new Label();
            Manager.Scene.AddEntity(title);
            title.Caption = Session.Strings["pause_title"];
            title.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X, 200);
            title.Center = true;
            title.ZOrder = -1000001;

            menu = new ButtonGroup();
            Manager.Scene.AddEntity(menu);
            menu.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X, 300);
            menu.ButtonHeight = 70;
            menu.OnExit += OnEnterResume;
            menu.ZOrder = -1000002;

            Button button;

            button = new Button();
            button.Caption = Session.Strings["pause_resume"];
            button.OnEnter += OnEnterResume;
            menu.Add(button);

            button = new Button();
            button.Caption = Session.Strings["pause_reset"];
            button.OnEnter += OnEnterReset;
            menu.Add(button);

            button = new Button();
            button.Caption = Session.Strings["pause_exit"];
            button.OnEnter += OnEnterExit;
            menu.Add(button);

            // Indicaciones de input:
            hints = new List<InputHintLabel>();

            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_select"];
            inputHint.Location = new Vector2(384, Manager.Graphics.ScreenSafeArea.Bottom - 100);
            inputHint.ZOrder = -1000002;
            hints.Add(inputHint);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.B;
            inputHint.Caption = Session.Strings["button_back"];
            inputHint.Location = new Vector2(616, Manager.Graphics.ScreenSafeArea.Bottom - 100);
            inputHint.ZOrder = -1000002;
            hints.Add(inputHint);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.LeftThumb;
            inputHint.Caption = Session.Strings["button_move"];
            inputHint.Location = new Vector2(825, Manager.Graphics.ScreenSafeArea.Bottom - 100);
            inputHint.ZOrder = -1000002;
            hints.Add(inputHint);

            if (MusicPlayer.Volume > 0) MusicPlayer.Volume /= 2;
        }

        public override void Terminate()
        {
            Manager.Vars["showMessageReady"] = false;
            Manager.Vars["showMessagePause"] = false;

            background.Kill();
            title.Kill();
            menu.Kill();
            foreach (InputHintLabel hint in hints) hint.Kill();

            MusicPlayer.Volume *= 2;

            base.Terminate();
        }

        private void OnEnterResume(ButtonGroup group)
        {
            Kill();
        }

        public void OnEnterReset(ButtonGroup group)
        {
            group.Enabled = false;
            ConfirmationScreen dialog = new ConfirmationScreen();
            Manager.Scene.AddEntity(dialog);
            dialog.Caption = Session.Strings["confirm_levelreset"];
            dialog.Group = menu;
            dialog.OnOkEvent += OnResetEvent;
        }

        public void OnEnterExit(ButtonGroup group)
        {
            group.Enabled = false;
            ConfirmationScreen dialog = new ConfirmationScreen();
            Manager.Scene.AddEntity(dialog);
            dialog.Caption = Session.Strings["confirm_levelexit"];
            dialog.Group = menu;
            dialog.OnOkEvent += OnExitEvent;
        }

        public void OnResetEvent()
        {
            ResetLevel reset = new ResetLevel();
            Manager.Scene.AddEntity(reset);
            reset.Inmediate = true;
            Kill();
        }

        public void OnExitEvent()
        {
            Manager.GameStates.ChangeState("levelSelection");
        }
    }
}
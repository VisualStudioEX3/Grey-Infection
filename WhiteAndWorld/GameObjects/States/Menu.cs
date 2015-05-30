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
using WhiteAndWorld.GameObjects.Entities.MainMenu;

namespace WhiteAndWorld.GameObjects.States
{
    public class Menu : StateComponent
    {
        private SoundPlayer menuSelect, menuConfirm;
        private ButtonGroup menu;
        private PurchaseButton purchase;

        public override void Initialize()
        {
            Manager.Scene.AddEntity(new PlayerSignedOut());

            Manager.Vars["CMYK_Ammo"] = 0;
            Manager.Vars["score"] = 0;

            MusicPlayer.Play(MusicPlayer.MenuTheme);

            Manager.Scene.AddEntity(new GreyInfectiontitleScene());

            // Opciones del menu:
            menu = new ButtonGroup();
            Manager.Scene.AddEntity(menu);
            menu.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Right - 240, 360);
            menu.ButtonHeight = 70;

            Button button;

            button = new Button();
            button.Caption = Session.Strings["mainmenu_play"];
            button.ConfirmFXWaitToEnd = true;
            button.OnEnter += OnEnterPlay;
            menu.Add(button);

            button = new Button();
            button.Caption = Session.Strings["mainmenu_options"];
            button.ConfirmFXWaitToEnd = true;
            button.OnEnter += OnEnterOptions;
            menu.Add(button);

            button = new Button();
            button.Caption = Session.Strings["mainmenu_credits"];
            button.ConfirmFXWaitToEnd = true;
            button.OnEnter += OnEnterCredits;
            menu.Add(button);

            button = new Button();
            button.Caption = Session.Strings["mainmenu_exit"];
            button.OnEnter += OnEnterExit;
            menu.Add(button);

            // Si es modo Trial se activa el boton de compra:
            if (Guide.IsTrialMode)
            {
                purchase = new PurchaseButton();
                Manager.Scene.AddEntity(purchase);                
            }

            // Indicaciones de input:
            InputHintLabel inputHint = new InputHintLabel();            
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_select"];
            inputHint.Location = new Vector2(475, Manager.Graphics.ScreenSafeArea.Bottom - 14);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.LeftThumb;
            inputHint.Caption = Session.Strings["button_move"];
            inputHint.Location = new Vector2(750, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            
            base.Initialize();
        }

        public void OnEnterPlay(ButtonGroup group)
        {
            Manager.GameStates.ChangeState("levelSelection");
        }

        public void OnEnterOptions(ButtonGroup group)
        {
            Manager.GameStates.ChangeState("options");            
        }

        public void OnEnterCredits(ButtonGroup group)
        {
            Manager.GameStates.ChangeState("credits");
        }

        public void OnEnterExit(ButtonGroup group)
        {
            group.Enabled = false;
            ConfirmationScreen dialog = new ConfirmationScreen();
            Manager.Scene.AddEntity(dialog);
            dialog.Caption = Session.Strings["confirm_exit"];
            dialog.Group = menu;
            dialog.OnOkEvent += OnExitEvent;
            dialog.OnCancelEvent += OnCancelExitEvent;

            if (purchase != null) purchase.Enabled = false;
        }

        public void OnExitEvent()
        {
            if (Guide.IsTrialMode) Manager.GameStates.ChangeState("endtrial"); else Manager.Terminate();
        }

        public void OnCancelExitEvent()
        {
            if (purchase != null) purchase.Enabled = true;
        }
    }
}
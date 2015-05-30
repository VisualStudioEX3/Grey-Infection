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
using WhiteAndWorld.GameObjects.Entities.MainMenu;
using WhiteAndWorld.GameObjects.Entities.EndTrial;

namespace WhiteAndWorld.GameObjects.States
{
    public class EndTrial : StateComponent
    {
        private InputHintLabel purchase, exit;
        private Timer timer;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            MusicPlayer.Play(MusicPlayer.MenuTheme);

            Manager.Scene.AddEntity(new GreyInfectiontitleScene());
            Manager.Scene.AddEntity(new CheckList());

            Label title = new Label();
            Manager.Scene.AddEntity(title);
            title.Caption = Session.Strings["endtrial_title"];
            title.Location = new Vector2(540, 340);
            title.ZOrder = -999999;

            Label text = new Label();
            Manager.Scene.AddEntity(text);
            text.SmallFont = true;
            text.Caption = Session.Strings["endtrial_text"];
            text.Location = new Vector2(540, 390);
            text.ZOrder = -999999;

            Thumbnail thumbnail;
            for (int i = 3; i > -1; i--)
            {
                thumbnail = new Thumbnail();
                Manager.Scene.AddEntity(thumbnail);
                thumbnail.Index = i;
                thumbnail.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Left - (30 * i) + 40, 340 - (30 * i));
            }

            purchase = new InputHintLabel();
            Manager.Scene.AddEntity(purchase);
            purchase.Button = InputHintLabel.GamepadButtonChar.X;
            purchase.Caption = Session.Strings["endtrial_purchase"];
            purchase.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Left + 20, Manager.Graphics.ScreenSafeArea.Bottom - 70);
            purchase.ZOrder = -999999999;

            exit = new InputHintLabel();
            Manager.Scene.AddEntity(exit);
            exit.Button = (bool)Manager.Vars["purchaseAndReturnMenu"] ? InputHintLabel.GamepadButtonChar.B : InputHintLabel.GamepadButtonChar.A;
            exit.Caption = Session.Strings["endtrial_back"];
            exit.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Left + 20, Manager.Graphics.ScreenSafeArea.Bottom - 20);
            exit.ZOrder = -999999999;

            timer = new Timer();
        }

        // Indica si el jugador tiene privilegios de compra:
        private bool IsPlayerAllowPurchase(PlayerIndex index)
        {
            foreach (SignedInGamer player in Manager.GamerServices.SignedInGamers)
                if (player.PlayerIndex == index) return player.Privileges.AllowPurchaseContent;

            return false;
        }

        public override void Update(GameTime gameTime)
        {
            // Se supone que una vez realizada la compra esta pantalla deberia anularse y volver al menu principal:
            if (!Guide.IsTrialMode) Manager.GameStates.ChangeState("menu");

            if (Manager.UIInput.Hit("switch") && !Guide.IsVisible && IsPlayerAllowPurchase(Manager.UIInput.Player))
            {
                Guide.ShowMarketplace(Manager.UIInput.Player);
            }
            else if (Manager.UIInput.Hit((bool)Manager.Vars["purchaseAndReturnMenu"] ? "exit" : "jump"))
            {
                if ((bool)Manager.Vars["purchaseAndReturnMenu"])
                {
                    Manager.Vars["purchaseAndReturnMenu"] = false;
                    Manager.GameStates.ChangeState("menu");
                }
                else
                {
                    Manager.Terminate();
                }
            }

            base.Update(gameTime);
        }
    }
}

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

namespace WhiteAndWorld.GameObjects.Entities.MainMenu
{
    public class PurchaseButton : Entity
    {
        private Clue[] clue;
        private Label text;
        private InputHintLabel button;
        private Timer timer;

        public override void Initialize()
        {
            base.Initialize();

            text = new Label();
            text.Initialize();            
            text.Location = new Vector2(160, 380);
            text.Caption = Session.Strings["endtrial_buttonPurchase"];

            button = new InputHintLabel();
            button.Initialize();
            button.Button = InputHintLabel.GamepadButtonChar.X;
            button.Caption = Session.Strings["endtrial_unlock"];
            button.Location = new Vector2(280, 490);

            clue = new Clue[2];

            string texture = "msc_281x282";
            Manager.Graphics.LoadTexture(@"Clues\" + texture);
            clue[0] = new Clue();
            clue[0].Texture = texture;
            clue[0].Location = new Vector2(100 + 40, 300);

            texture = "msc_281x282b";
            Manager.Graphics.LoadTexture(@"Clues\" + texture);
            clue[1] = new Clue();
            clue[1].Texture = texture;
            clue[1].Location = new Vector2(290 + 40, 310);

            timer = new Timer();
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < clue.Length; i++) clue[i].Draw(gameTime);
            if (button.Visible) button.Draw(gameTime);
            text.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (Manager.UIInput.Hit("switch"))
            {
                Manager.Vars["purchaseAndReturnMenu"] = true;
                Manager.GameStates.ChangeState("endtrial");
            }

            for (int i = 0; i < clue.Length; i++) clue[i].Update(gameTime);
            text.Update(gameTime);
            button.Update(gameTime);

            if (timer.Value > 700)
            {
                timer.Reset();
                button.Visible = !button.Visible;
            }

            base.Update(gameTime);
        }
    }
}

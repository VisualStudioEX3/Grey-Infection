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

namespace WhiteAndWorld.GameObjects.Entities.EndGame
{
    public class WarningBox : Entity
    {        
        private string contentText;
        private Timer timer;
        private int currentChar;

        private Sprite box;
        private Label title, warningContent;
        private Clue clue1, clue2, clue3;

        private SoundPlayer[] typing;

        public override void Initialize()
        {
            base.Initialize();

            box = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\EndGame\main1"));
            box.Center = true;
            box.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);

            contentText = Session.Strings["gameover_message"];
            currentChar = 0;

            title = new Label();
            title.Initialize();
            title.Center = true;
            title.Caption = Session.Strings["gameover_warningtitle"];
            title.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X, Manager.Graphics.ScreenSafeArea.Center.Y - 170);
            title.Scale = 1.5f;

            warningContent = new Label();
            warningContent.Initialize();
            warningContent.SmallFont = true;
            warningContent.Caption = "";
            warningContent.Location = new Vector2(Manager.Graphics.ScreenSafeArea.X + 250, Manager.Graphics.ScreenSafeArea.Y + 210);
            warningContent.Scale = 0.9f;

            Manager.Graphics.LoadTexture(@"Clues\msc_281x282");
            clue1 = new Clue();
            clue1.Texture = "msc_281x282";
            clue1.Location = new Vector2(144, 380);

            Manager.Graphics.LoadTexture(@"Clues\msc_281x282b");
            clue2 = new Clue();
            clue2.Texture = "msc_281x282b";
            clue2.Location = new Vector2(250, 60);

            Manager.Graphics.LoadTexture(@"Clues\msc_281x282c");
            clue3 = new Clue();
            clue3.Texture = "msc_281x282c";
            clue3.Location = new Vector2(848, 200);

            typing = new SoundPlayer[4];
            for (int i = 0; i < 4; i++)
                typing[i] = new SoundPlayer(@"Audio\FX\typing0" + i);

            timer = new Timer();

            ZOrder = -999999999;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentChar < contentText.Length && timer.Value > 50)
            {
                if (Manager.UIInput.Hit("jump")) currentChar = contentText.Length;

                timer.Reset();
                warningContent.Caption = contentText.Substring(0, currentChar) + (currentChar % 2 == 0 && currentChar < contentText.Length ? "|" : "");
                currentChar++;
                typing[MathTools.RandomNumberGenerator.Next(0, 3)].Play();
            }
            else if (currentChar >= contentText.Length)
            {
                warningContent.Caption = contentText;
                Manager.Vars["creditsFromEndGame"] = true;
                if (Manager.UIInput.Hit("jump")) Manager.GameStates.ChangeState("credits");
            }

            clue1.Update(gameTime);
            clue2.Update(gameTime);
            clue3.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            clue1.Draw(gameTime);
            clue2.Draw(gameTime);
            clue3.Draw(gameTime);
            box.Draw(gameTime);
            title.Draw(gameTime);
            warningContent.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}

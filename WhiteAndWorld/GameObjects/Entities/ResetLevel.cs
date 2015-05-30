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

namespace WhiteAndWorld.GameObjects.Entities
{
    public class ResetLevel : Entity
    {
        private Timer timer;
        private Label loading;
        private BackBox background;

        public bool Inmediate { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            background = new BackBox();
            Manager.Scene.AddEntity(background);
            background.Visible = false;
            background.ZOrder = -999999998;

            loading = new Label();
            Manager.Scene.AddEntity(loading);
            loading.Caption = Session.Strings["loading"];
            loading.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);
            loading.Center = true;
            loading.Visible = false;
            loading.ZOrder = -999999999;

            Inmediate = false;

            timer = new Timer();
        }

        public override void Update(GameTime gameTime)
        {
            if (timer.Value > (Inmediate ? 500 : 2000))
            {
                if (Guide.IsTrialMode && (int)Manager.Vars["currentLevel"] > 7)
                {
                    Session.GameProgress.CurrentLevel = 7;
                    Manager.Vars["currentLevel"] = 7;
                    Manager.Vars["purchaseAndReturnMenu"] = true;
                    Manager.GameStates.ChangeState("endtrial");
                }
                else
                {
                    if ((int)Manager.Vars["currentLevel"] > 32)
                    {
                        Session.GameProgress.CurrentLevel = 32;
                        Manager.Vars["currentLevel"] = 32;
                        Manager.GameStates.ChangeState("gameover");
                    }
                    else
                        Manager.GameStates.ChangeState("game");
                }
            }
            else if (timer.Value > 1500 || Inmediate)
            {
                loading.Visible = true;
                background.Visible = true;
            }

            base.Update(gameTime);
        }
    }
}

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

namespace WhiteAndWorld.GameObjects.States
{
    public class Credits : StateComponent
    {
        private List<CreditsEntry> credits;
        private Timer timer;
        private int offset;
        private int y;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            if ((bool)Manager.Vars["creditsFromEndGame"]) Manager.Graphics.EndShader(); else Manager.Graphics.BeginShader();

            GreyInfectiontitleScene background = new GreyInfectiontitleScene();
            Manager.Scene.AddEntity(background);
            background.EnableGlitchColor = false;

            credits = Serializers.DeserializeFromTitleStorage<List<CreditsEntry>>("GameData/Credits/" + Session.Culture + ".xml");

            Label entry;
            y = 0;

            foreach (CreditsEntry c in credits)
            {
                entry = new Label();
                Manager.Scene.AddEntity(entry);
                entry.SmallFont = false;
                entry.Caption = c.Title;
                entry.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X, y);
                entry.Fixed = false;
                entry.Center = true;

                entry = new Label();
                Manager.Scene.AddEntity(entry);
                entry.SmallFont = true;
                entry.Caption = c.Text;                
                entry.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X - (entry.Bounds.Width / 2), y + 32);
                entry.Fixed = false;

                y += 250;
            }

            Fog fog = new Fog();
            Manager.Scene.AddEntity(fog);
            fog.ZOrder = -9999998;

            // Pistas:
            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = (bool)Manager.Vars["creditsFromEndGame"] ? InputHintLabel.GamepadButtonChar.A : InputHintLabel.GamepadButtonChar.B;
            inputHint.Caption = (bool)Manager.Vars["creditsFromEndGame"] ? Session.Strings["button_continue"] : Session.Strings["button_back"];
            inputHint.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X - 40, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            inputHint.ZOrder = -9999999;

            timer = new Timer();
            offset = 600;

            Manager.Graphics.OffSet = new Vector2(0, offset);
        }

        public override void Terminate()
        {
            Manager.Vars["creditsFromEndGame"] = false;
            Manager.Graphics.OffSet = Vector2.Zero;
            base.Terminate();
        }

        public override void Update(GameTime gameTime)
        {
            bool pressed = Manager.UIInput.Press("jump");
            if (timer.Value > (pressed ? 1 : 25) && y > 0)
            {
                Manager.Graphics.OffSet = new Vector2(0, offset);
                if (pressed) offset -= 5; else offset--;
                if (pressed) y -= 5; else y--;
                timer.Reset();
            }

            if ((bool)Manager.Vars["creditsFromEndGame"] ? Manager.UIInput.Hit("jump") : Manager.UIInput.Hit("exit")) Manager.GameStates.ChangeState("menu");
        }
    }
}
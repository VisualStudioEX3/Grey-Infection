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
using WhiteAndWorld.GameObjects.Entities.GUI;

namespace WhiteAndWorld.GameObjects.Entities.Options
{
    public class GameInputMap : Entity
    {
        private RectangleOverlay back;
        private Sprite gameInput;
        private InputHintLabel inputHint;

        private List<Label> labels;

        public ButtonGroup Sender { get; set; }

        public bool DisableBackground 
        {
            get { return back != null ? !back.Visible : false; }
            set { if (back != null) back.Visible = !value; }
        }

        public bool DisableHint 
        {
            get { return inputHint != null ? !inputHint.Visible : false; }
            set { if (inputHint != null) inputHint.Visible = !value; }
        }

        public override void Initialize()
        {
            base.Initialize();

            back = new RectangleOverlay(Manager.Graphics.ScreenBounds);
            back.BackColor = new Color(0, 0, 0, 0.85f);
            back.Fixed = true;

            // Esquema del gamepad de XBox360:
            gameInput = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Options\pad_menu"));
            gameInput.Center = true;
            gameInput.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);
            gameInput.Fixed = true;

            // Textos de ayuda:
            Label label;
            labels = new List<Label>();

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_pause"];
            label.Location = new Vector2(300, 200);            
            label.Center = true;
            label.ZOrder = -999999908;

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_move"];
            label.Location = new Vector2(300, 320);            
            label.Center = true;
            label.ZOrder = -999999908;

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_crouch"];
            label.Location = new Vector2(300, 516);
            label.Center = true;
            label.ZOrder = -999999908;

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_weapon"];
            label.Location = new Vector2(955, 204);
            label.Center = true;
            label.ZOrder = -999999908;

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_jump"];
            label.Location = new Vector2(955, 320);
            label.Center = true;
            label.ZOrder = -999999908;

            label = new Label();
            labels.Add(label);
            Manager.Scene.AddEntity(label);
            label.SmallFont = true;
            label.Caption = Session.Strings["gamepad_shoot"];
            label.Location = new Vector2(955, 516);
            label.Center = true;
            label.ZOrder = -999999908;

            // Pistas:
            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.B;
            inputHint.Caption = Session.Strings["button_back"];
            inputHint.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X - 40, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            inputHint.ZOrder = -999999908;

            base.ZOrder = -999999907;
        }

        public override void Terminate()
        {
            inputHint.Kill();
            foreach (Label l in labels)
                l.Kill();

            if (Sender != null) Sender.Enabled = true;
            base.Terminate();
        }

        public override void Update(GameTime gameTime)
        {
            if (!DisableHint && Manager.UIInput.Hit("exit")) Kill();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            back.Draw(gameTime);
            gameInput.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
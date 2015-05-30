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

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class ConfirmationScreen : Entity
    {
        private BackBox background;
        private Label caption;
        private InputHintLabel ok, cancel;

        public delegate void OnEventHandler();

        public OnEventHandler OnOkEvent, OnCancelEvent;

        public ButtonGroup Group { get; set; }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                if (caption != null) caption.Caption = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            background = new BackBox();
            background.ZOrder = -2000000;
            Manager.Scene.AddEntity(background);

            // Texto de la pregunta:
            caption = new Label();
            Manager.Scene.AddEntity(caption);
            caption.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X, Manager.Graphics.ScreenBounds.Center.Y - 50);
            caption.SmallFont = false;
            caption.Center = true;
            caption.ZOrder = -2000001;

            // Indicaciones de input:
            ok = new InputHintLabel();
            Manager.Scene.AddEntity(ok);
            ok.Button = InputHintLabel.GamepadButtonChar.A;
            ok.Caption = Session.Strings["button_confirm"];
            ok.Location = new Vector2(450, Manager.Graphics.ScreenSafeArea.Bottom - 225);
            ok.ZOrder = -2000002;

            cancel = new InputHintLabel();
            Manager.Scene.AddEntity(cancel);
            cancel.Button = InputHintLabel.GamepadButtonChar.B;
            cancel.Caption = Session.Strings["button_cancel"];
            cancel.Location = new Vector2(750, Manager.Graphics.ScreenSafeArea.Bottom - 225);
            cancel.ZOrder = -2000002;
        }

        public override void Terminate()
        {
            background.Kill();
            caption.Kill();
            ok.Kill();
            cancel.Kill();

            Group.Enabled = true;

            base.Terminate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Manager.UIInput.Hit("jump"))
            {
                if (OnOkEvent != null) OnOkEvent();
                Group.Enabled = true;
                Kill();
            }
            else if (Manager.UIInput.Hit("exit"))
            {                
                if (OnCancelEvent != null) OnCancelEvent();
                Group.Enabled = true;
                Kill();
            }

            base.Update(gameTime);
        }
    }
}
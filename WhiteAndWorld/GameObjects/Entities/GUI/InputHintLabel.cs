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

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class InputHintLabel : Entity
    {
        /// <summary>
        /// Lista de constantes de botones del gamepad de XBox360.
        /// </summary>
        public enum GamepadButtonChar
        {
            X,
            Y,
            A,
            B,
            RightTrigger,
            LeftTrigger,
            Back,
            DPad,
            Guide,
            LeftShoulder,
            LeftThumb,
            RightShoulder,
            RightThumb,
            Start
        }
                
        private string GetButtonChar(GamepadButtonChar button)
        {
            switch (button)
            {
                case GamepadButtonChar.X: { spriteButton.Scale = 0.4f; return "&"; }
                case GamepadButtonChar.Y: { spriteButton.Scale = 0.4f; return "("; }
                case GamepadButtonChar.A: { spriteButton.Scale = 0.4f; return "'"; }
                case GamepadButtonChar.B: { spriteButton.Scale = 0.4f; return ")"; }
                case GamepadButtonChar.RightTrigger: { spriteButton.Scale = 0.4f; return "+"; }
                case GamepadButtonChar.LeftTrigger: { spriteButton.Scale = 0.4f; return ","; }
                case GamepadButtonChar.Back: { spriteButton.Scale = 0.4f; return "#"; }
                case GamepadButtonChar.DPad: { spriteButton.Scale = 0.4f; return "!"; }
                case GamepadButtonChar.Guide: { spriteButton.Scale = 0.3f; return "$"; }
                case GamepadButtonChar.LeftShoulder: { spriteButton.Scale = 0.3f; return "-"; }
                case GamepadButtonChar.LeftThumb: { spriteButton.Scale = 0.3f; return " "; }
                case GamepadButtonChar.RightShoulder: { spriteButton.Scale = 0.3f; return "*"; }
                case GamepadButtonChar.RightThumb: { spriteButton.Scale = 0.3f; return "\""; }
                case GamepadButtonChar.Start: { spriteButton.Scale = 0.4f; return "%"; }
                default: return "";
            }
        }

        private TextLabel spriteButton, caption, shadow;

        private string _buttonChar;
        private GamepadButtonChar _button;
        public GamepadButtonChar Button 
        {
            get { return _button; }
            set
            {
                _button = value;
                _buttonChar = GetButtonChar(value);
                if (spriteButton != null)
                    spriteButton.Text = _buttonChar;
            }
        }

        public override Vector2 Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                if (caption != null)
                {
                    caption.Location = value;
                    shadow.Location = new Vector2(value.X + 2, value.Y + 2);
                    spriteButton.Location = new Vector2(caption.Bounds.Left - 32, value.Y + 8);
                }
            }
        }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set 
            { 
                _caption = value;
                if (caption != null)
                {
                    caption.Text = value;
                    shadow.Text = value;
                }
            }
        }


        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadFont(@"Fonts\xboxControllerSpriteFont");
            Manager.Graphics.LoadFont(@"Fonts\androidNation14");

            spriteButton = new TextLabel(@"Fonts\xboxControllerSpriteFont");
            spriteButton.Center = true;
            spriteButton.Fixed = true;

            caption = new TextLabel(@"Fonts\androidNation14");
            caption.Fixed = true;

            shadow = new TextLabel(@"Fonts\androidNation14");
            shadow.Color = Color.Black;
            shadow.Fixed = true;
        }

        public override void Draw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            caption.Draw(gameTime);
            spriteButton.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

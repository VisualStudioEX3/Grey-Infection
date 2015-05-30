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

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public enum ButtonsShow
    {
        A_Button, B_Button, AB_Buttons, Start_Button
    }

    public class InputInfoFooter : Entity 
    {
        private Sprite box;

        private ButtonsShow button;
        public ButtonsShow Buttons 
        {
            get { return button; }
            set
            {
                button = value;
                switch (button)
                {
                    case ButtonsShow.A_Button:
                        box.TextureName = @"GameUI\a_option_footer";
                        break;
                    case ButtonsShow.B_Button:
                        box.TextureName = @"GameUI\b_option_footer";
                        break;
                    case ButtonsShow.AB_Buttons:
                        box.TextureName = @"GameUI\ab_option_footer";
                        break;
                    case ButtonsShow.Start_Button:
                        box.TextureName = @"GameUI\start_option_footer";
                        break;
                }
            }
        }

        public override Vector2 Location
        {
            get
            {
                return box != null ? box.Location : Vector2.Zero;
            }
            set
            {
                if (box != null) box.Location = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture(@"GameUI\a_option_footer");
            Manager.Graphics.LoadTexture(@"GameUI\b_option_footer");
            Manager.Graphics.LoadTexture(@"GameUI\ab_option_footer");
            Manager.Graphics.LoadTexture(@"GameUI\start_option_footer");

            box = new Sprite();
            box.Location = Vector2.Zero;
            box.Center = true;
            box.Fixed = true;

            Buttons = ButtonsShow.A_Button;
        }

        public override void Draw(GameTime gameTime)
        {
            box.Draw(gameTime);
        }
    }
}

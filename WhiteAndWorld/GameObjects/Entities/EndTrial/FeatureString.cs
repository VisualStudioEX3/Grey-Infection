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

namespace WhiteAndWorld.GameObjects.Entities.EndTrial
{
    public class FeatureString : Entity
    {
        private Sprite check;
        private Label label;

        public FeatureString(string text, Vector2 location)
        {
            check = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\EndTrial\checkbox"));
            check.Center = true;
            check.Location = location;
            check.Scale = 0.5f;

            label = new Label();
            label.Initialize();
            label.SmallFont = true;
            label.Caption = text;
            label.Location = new Vector2(location.X + 30, location.Y - 5);
            label.Scale = 0.7f;
        }

        public override void Draw(GameTime gameTime)
        {
            check.Draw(gameTime);
            label.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

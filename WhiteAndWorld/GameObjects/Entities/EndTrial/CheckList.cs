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
    public class CheckList : Entity
    {
        private int height = 32;
        private List<FeatureString> list;

        public override void Initialize()
        {
            base.Initialize();
            list = new List<FeatureString>();

            Location = new Vector2(550, 510);
            
            for (int i = 0; i < 5; i++)
                list.Add(new FeatureString(Session.Strings["endtrial_check" + i], new Vector2(Location.X, Location.Y + (height * i))));
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (FeatureString f in list)
                f.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

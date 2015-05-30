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
    public class Thumbnail : Entity
    {
        private Sprite thumbnail;
        private Timer timer;
        private List<Texture2D> shoots;

        private int _index;
        public int Index 
        {
            get { return _index; }
            set 
            { 
                _index = value;
                if (_index > 3) _index = 0;
                if (thumbnail != null) thumbnail.Texture = shoots[_index];
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
                if (thumbnail != null) thumbnail.Location = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            shoots = new List<Texture2D>();
            for (int i = 0; i < 4; i++)
                shoots.Add(Manager.Graphics.LoadTexture(@"GameUI\EndTrial\shoots\" + i));

            thumbnail = new Sprite();
            thumbnail.Scale = 0.3f;

            timer = new Timer();
        }

        public override void Update(GameTime gameTime)
        {
            if (timer.Value > 2500)
            {
                timer.Reset();
                Index++;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            thumbnail.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}

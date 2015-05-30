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

namespace WhiteAndWorld.GameObjects.Entities
{
    public class Label : Entity
    {
        private const string big = @"Fonts\androidNation20b";
        private const string small = @"Fonts\androidNation14";

        private TextLabel title, shadow;

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set 
            { 
                _caption = value;
                if (title != null)
                {
                    title.Text = value;
                    shadow.Text = value; 
                }
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
                if (title != null)
                {
                    title.Location = value;
                    shadow.Location = new Vector2(value.X + 2, value.Y + 2);
                }
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return title != null ? title.Bounds : base.Bounds;
            }
            set
            {
                base.Bounds = value;
            }
        }

        private bool _smallFont;

        public bool SmallFont
        {
            get { return _smallFont; }
            set 
            { 
                _smallFont = value;
                if (title != null)
                {
                    title.FontName = value ? small : big;
                    shadow.FontName = title.FontName;
                }
            }
        }

        private bool _center;

        public bool Center
        {
            get { return _center; }
            set 
            { 
                _center = value;
                if (title != null)
                {
                    title.Center = value;
                    shadow.Center = value;
                }
            }
        }

        public bool Fixed 
        {
            get { return title != null ? title.Fixed : false; }
            set
            {
                if (title != null)
                {
                    title.Fixed = value;
                    shadow.Fixed = value;
                }
            }
        }

        public float Scale
        {
            get { return (title != null) ? title.Scale : 0; }
            set
            {
                if (title != null)
                {
                    title.Scale = value;
                    shadow.Scale = value;
                }
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            Manager.Graphics.LoadFont(big);
            Manager.Graphics.LoadFont(small);

            title = new TextLabel(big);
            title.Fixed = true;
            shadow = new TextLabel(big);
            shadow.Fixed = true;
            shadow.Color = Color.Black;

            _smallFont = false;

            ZOrder = -999999;
        }

        public override void Draw(GameTime gameTime)
        {
            shadow.Draw(gameTime);
            title.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
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

namespace WhiteAndWorld.GameObjects.Entities
{
    public class Clue : Entity
    {
        private Sprite clue;

        // Solo se podran usar texturas que esten dentro del directorio Clues del Content.
        // Se deberan pasar solo el nombre del asset de la textura, sin la ruta "Clues\".
        public String Texture
        {
            get { if (clue == null) return String.Empty; else return clue.TextureName.Replace(@"Clues\", ""); }
            set
            {
                string textureName = value;
                // Averiguamos si se trata de una pista de texto:
                if (value.StartsWith("clue_txt_"))
                {
                    // Obtenemos la pista en el idioma de la cultura del sistema:
                    textureName += "_" + Session.Culture.ToLower();
                }

                // Inicializacion de propiedades en base a la textura seleccionada:
                clue = new Sprite(@"Clues\" + textureName);
                
                // Buscamos si existe definicion del tile para indicar animacion:
                try
                {
                    GameDefinitions.ClueInfo info = TLSA.Engine.Tools.XML.Serializers.DeserializeFromTitleStorage<GameDefinitions.ClueInfo>("GameData/ClueInfo/" + textureName + ".xml");
                    this.Bounds = new Rectangle(0, 0, info.Width, info.Height);
                    clue.Animations.AddSecuence("default", new Rectangle(0, 0, info.Width, info.Height), info.Frames, info.Delay, true);
                    clue.Animations.Play("default");
                    clue.Update(new GameTime());
                }
                catch (Exception)
                {
                    this.Bounds = clue.Texture.Bounds;
                }

                this.ZOrder = value.StartsWith("clue_") ? 50 : 100;
            }
        }

        public override Vector2 Location
        {
            get
            {
                return clue == null ? Vector2.Zero : clue.Location;
            }
            set
            {
                if (clue != null) clue.Location = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            base.ZOrder = 100;
        }

        public override void Update(GameTime gameTime)
        {
            if (clue != null) clue.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (clue != null) clue.Draw(gameTime);
        }
    }
}
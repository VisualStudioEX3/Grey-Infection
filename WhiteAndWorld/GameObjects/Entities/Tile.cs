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
    public class Tile : Entity
    {
        private Sprite tile;
        private Body box;

        private Vector2 size;

        private Timer ghostTimer;
        private int ghostTime;

        private bool setInitialState = false;
        private bool initialState = true;

        private const int passableTimeDelay = 10; // Tiempo, en segundos, maximo que el tile sera pasable.

        private Color SemiTransparent = new Color(255, 255, 255, 128);

        private bool safeInitialize = false; // Esta variable controlara que la caja de muerte del tile no este activa al recien crearse el objeto.

        // Cronometro y cuerpo para el tiempo de muerte al reaparecer un tile.
        // Se usara para matar al jugador en caso de que se aparezca estando este en su ubicacion:
        private Body deathBox;
        private Timer deathTimer;

        public override Vector2 Location
        {
            get { return box.Location - this.size / 2; }
            set
            {
                if (box != null)
                {
                    box.Location = value + this.size / 2;
                    tile.Location = box.Location;

                    Rectangle deathBounds = box.Bounds;
                    deathBounds.Y += deathBounds.Height / 2;
                    deathBounds.X -= 1;
                    deathBounds.Width += 2;
                    deathBounds.Height = (deathBounds.Height / 2) + 1;
                    deathBox.Bounds = deathBounds;
                }
            }
        }

        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;
                if (this.box != null)
                {
                    this.box.Enabled = value;
                    if (!this.setInitialState)
                    {
                        this.setInitialState = true;
                        this.initialState = value;
                    }
                }
            }
        }

        // Solo se podran usar texturas que esten dentro del directorio Tiles del Content.
        // Se deberan pasar solo el nombre del asset de la textura, sin la ruta "Tiles\".
        public String Texture
        {
            get { if (tile == null) return String.Empty; else return tile.TextureName.Replace(@"Tiles\", ""); }
            set
            {
                tile = new Sprite(@"Tiles\" + value);
                tile.Center = true;

                // Buscamos si existe definicion del tile para indicar animacion:
                try
                {
                    GameDefinitions.TileInfo info = TLSA.Engine.Tools.XML.Serializers.DeserializeFromTitleStorage<GameDefinitions.TileInfo>("GameData/TileInfo/" + value + ".xml");
                    this.Bounds = new Rectangle(0, 0, info.Width, info.Height);
                    tile.Animations.AddSecuence("default", new Rectangle(0, 0, info.Width, info.Height), info.Frames, info.Delay, true);
                    tile.Animations.Play("default");
                    tile.Update(new GameTime());
                }
                catch (Exception)
                {
                    this.Bounds = tile.Texture.Bounds;
                }

                this.size = new Vector2(this.Bounds.Width, this.Bounds.Height);
                this.box.Size = this.size;
            }
        }

        /// <summary>
        /// Indica si el tile podra ser pasable temporalmente al matar cierto tipo de enemigos.
        /// </summary>
        public bool Passable { get; set; }

        /// <summary>
        /// Indica si el tile es pasable.
        /// </summary>
        /// <remarks>Solo devolvera verdadero si es pasable y esta activo/visible.</remarks>
        public bool IsPassable { get { return this.Visible && this.ghostTime > 0; } }

        public override void Initialize()
        {
            base.Initialize();

            this.Bounds = Rectangle.Empty;
            this.size = Vector2.Zero;

            box = new Body(Manager.PhysicEngine, Rectangle.Empty, 1, true);
            Manager.PhysicEngine.Bodies.Add(box);
            
            // Creamos la caja de muerte para cuando el tile se active por accion mate al jugador si se encuentra en su ubicacion:
            deathBox = new Body(Manager.PhysicEngine, Rectangle.Empty, 0, true);
            deathBox.Tag = "enemy";
            deathBox.Enabled = false;
            Manager.PhysicEngine.Bodies.Add(deathBox);

            deathTimer = new Timer();

            this.ghostTimer = new Timer();
            this.ghostTime = 0;

            this.ZOrder = -9999;
        }

        public override void Terminate()
        {
            Manager.PhysicEngine.Bodies.Remove(box);
            Manager.PhysicEngine.Bodies.Remove(deathBox);
            base.Terminate();
        }

        public override void ReciveMessage(string message, params object[] value)
        {
            switch (message)
            {
                case "hit": this.Visible = (bool)value[0]; if ((bool)value[0]) deathTimer.Reset(); break;
                case "reset": this.Visible = this.initialState; break;
                case "ghost":
                    if (this.Passable && this.Visible)
                    {
                        this.ghostTimer.Reset();
                        this.ghostTime = passableTimeDelay;
                    }
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Controlamos el tiempo de traspasar paredes incluso habiendo pausa:
            if (this.ghostTimer.Value >= 1000 && this.ghostTime > 0)
            {
                this.ghostTimer.Reset();
                this.ghostTime--;
            }

            if (safeInitialize)
            {
                // Mantenemos activo la caja de muerte durante 10 milisegundos:
                deathBox.Enabled = deathTimer.Value < 10;
            }
            else
                safeInitialize = deathTimer.Value >= 1000; // Al pasar un segundo activamos el flag de seguridad.
                
            if (tile != null) tile.Location = box.Location;

            if (this.Passable)
            {
                if (this.ghostTime > 0)
                {
                    this.box.Enabled = false;
                    this.tile.Color = SemiTransparent;
                }
                else
                {
                    this.box.Enabled = true;
                    this.tile.Color = Color.White;
                } 
            }

            if (tile != null) tile.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (tile != null) tile.Draw(gameTime);
        }
    }
}
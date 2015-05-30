using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects
{
    public class EnemyAnimationEngine : IDrawableAndUpdateable 
    {
        private Sprite enemy;
        private Timer timer;
        private Vector2 halfSize = new Vector2(16, 16);

        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public Vector2 Location
        {
            get { return enemy.Location; }
            set
            {
                enemy.Location = value; // type > 2 ? value + halfSize : value;
            }
        }

        private int type;
        public int Type
        {
            get { return type; }
            set
            {
                type = value > 0 && value < 12 ? value : 0;
                switch (type)
                {
                    case 7: type = 5; break;
                    case 8: type = 6; break;
                }

                // Inicializamos la animacion en un fotograma aleatorio:                
                enemy.Animations.Play(type.ToString());
                enemy.Animations.CurrentFrameIndex = new Random().Next(0, 5);
            }
        }

        public bool Flag { get; set; }

        public EnemyAnimationEngine(string TextureName)
        {
            enemy = new Sprite(Manager.Graphics.LoadTexture(TextureName));

            // Cargamos secuencialmente todas las animaciones del spritesheet:
            for (int i = 0; i < 6; i++)
            {
                // La secuencia con indice 4 solo tiene 4 fotogramas.
                // Todas las secuencias se cargan con un tiempo entre fotogramas de 100 milisegundos y en modo ciclico:
                enemy.Animations.AddSecuence(i.ToString(), new Rectangle(0, 32 * i, 32, 32), 6, 64, true);
            }

            /* Lista de acciones por indice de animacion en la textura:
                0 - No hace nada
                1 - Muestra tile/s
                2 - Oculta tile/s
                3 - Reinicia estado del nivel
                4 - Mata a Joss
                5 - Activa temporalmente el modo fantasma de Joss (no esta implementado)
                6 - Activa temporalmente los tiles del escenario (oculta o muestra según estado inicial de los tiles) (no esta probado)
                7 - Añade munición al CMYK
                8 - Resta munición al CMYK
                9 - Añade tiempo al cronometro/detiene temporalmente el cronometro
                10 - Resta tiempo al cronometro
                11 - Activa temporalmente el modo invencible de Joss
             */

            // Seleccionamos por defecto el tipo 0:
            this.Type = 0;

            enemy.Update(new GameTime());

            timer = new Timer();

            this.Visible = true;
            this.Enabled = true;
        }

        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                //if (type > 2)
                //{
                //    if (timer.Value >= 5)
                //    {
                //        timer.Reset();
                //        enemy.Angle++;
                //    }
                //}
                //else
                //    enemy.Angle = 0;

                //if (Flag)
                //{
                //    //enemy.Color = new Color(196, 196, 196, 196);
                //    enemy.BlendingEffect = BlendFilters.XOR;
                //}
                //else
                //{
                //    //enemy.Color = Color.White;            
                //    enemy.BlendingEffect = BlendFilters.AlphaBlending;
                //}

                enemy.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (Visible) enemy.Draw(gameTime);
        }
    }
}
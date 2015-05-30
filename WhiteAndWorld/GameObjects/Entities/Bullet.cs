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
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities
{
    public class Bullet : Entity
    {
        private Sprite RGB_bullet;
        private Sprite CMYK_bullet;
        private Body box;

        public override Vector2 Location
        {
            get
            {
                return box.Location;
            }
            set
            {
                if (box != null) box.Location = value;
            }
        }

        public override bool Enabled
        {
            get
            {
                return box.Enabled;
            }
            set
            {
                if (box != null) box.Enabled = value;
            }
        }

        public override string Tag
        {
            get
            {
                return (string)box.Tag;
            }
            set
            {
                if (box != null)
                {
                    box.Tag = value;
                }
            }
        }

        public SpriteEffects Direction { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            RGB_bullet = new Sprite(Manager.Graphics.LoadTexture("rgb_bullet"));
            RGB_bullet.Center = true;

            CMYK_bullet = new Sprite(Manager.Graphics.LoadTexture("cmyk_bullet"));
            CMYK_bullet.Animations.AddSecuence("default", new Rectangle(0, 0, 16, 8), 2, 200, true);
            CMYK_bullet.Animations.Play("default");
            CMYK_bullet.Center = true;

            this.Bounds = new Rectangle(0, 0, 16, 8);

            box = new Body(Manager.PhysicEngine, this.Bounds, 1, true);
            box.OnCollision += this.OnCollision;
            Manager.PhysicEngine.Bodies.Add(box);

            Manager.Vars["shoots"] = (int)Manager.Vars["shoots"] + 1;

            this.Visible = false; // Evitamos que se muestre un fotograma el objeto en la posicion 0,0 si no se ha establecido su posicion todavia.   
        }

        private void OnCollision(Body b, Vector2 force, float direction)
        {
            if ((string)b.Tag != "Exit")
            {
                // Creamos una entidad de animacion de muerte y la agregamos a la escena:
                Bullet_Death bullet_death = new Bullet_Death();
                Manager.Scene.AddEntity(bullet_death);
                bullet_death.Location = this.Location - new Vector2(32, 32) / 2;

                this.Kill();
            }
        }

        public override void Terminate()
        {
            Manager.Vars["shoots"] = (int)Manager.Vars["shoots"] - 1;
            Manager.PhysicEngine.Bodies.Remove(box);
            base.Terminate();
        }

        public override void Update(GameTime gameTime)
        {
            if ((bool)Manager.Vars["showMessagePause"] == false)
            {
                box.Move(12, (this.Direction == SpriteEffects.None ? 0f : 180f));
                this.Visible = true;

                if (!Manager.PhysicEngine.WorkArea.Intersects(box.Bounds))
                    this.Kill();

                if (Tag != "RGB_Bullet")
                {
                    CMYK_bullet.Location = box.Location;
                    CMYK_bullet.Update(gameTime);
                }
                else
                    RGB_bullet.Location = box.Location;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Tag == "RGB_Bullet")
                RGB_bullet.Draw(gameTime);
            else
                CMYK_bullet.Draw(gameTime);
        }
    }
}
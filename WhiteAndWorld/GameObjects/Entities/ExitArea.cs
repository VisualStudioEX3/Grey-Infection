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
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities
{
    public class ExitArea : Entity
    {
        private Sprite clue;
        private Body body;
        private GlitchColor glitch;
        private bool glitched = false;

        private Color semi = new Color(1, 1, 1, 0.3f);
        private Timer timer;

        public override Vector2 Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;

                if (body != null)
                {
                    body.Location = value - new Vector2(16, 16);
                    clue.Location = body.Location;
                }
            }
        }

        public bool InvertClue 
        {
            get { return clue.Mirror == SpriteEffects.None; }
            set { clue.Mirror = value ? SpriteEffects.FlipHorizontally : SpriteEffects.None; }
        }

        public override void Initialize()
        {           
            base.Initialize();
            clue = new Sprite(@"Clues\clue_exit");
            clue.Center = true;

            body = new Body(Manager.PhysicEngine, new Rectangle(0, 0, 32, 32), 0);
            body.Trigger = true;
            body.Tag = "Exit";
            body.OnCollision += this.OnCollisionExitZone;
            body.Enabled = false;
            Manager.PhysicEngine.Bodies.Add(body);

            glitch = new GlitchColor();
            Manager.Scene.AddEntity(glitch);
            glitch.Manual = true;

            timer = new Timer();

            this.ZOrder = 50; // Por encima de la niebla.
        }

        public override void Update(GameTime gameTime)
        {
            body.Enabled = ((int)Manager.Vars["enemiesLeft"] <= 0);

            if (body.Enabled && Manager.Graphics.IsShaderActive)
            {
                if (!glitched)
                {
                    glitch.Now();
                    glitched = true;
                }
                Manager.Graphics.EndShader();
            }

            if (body.Enabled)
            {
                if (timer.Value > 250)
                {
                    timer.Reset();
                    if (clue.Color == Color.White) clue.Color = semi; else clue.Color = Color.White;
                }
            }
            else
                clue.Color = semi;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            clue.Draw(gameTime);
            base.Draw(gameTime);
        }

        // Codigo de colision del trigger del area de salida:
        private void OnCollisionExitZone(Body b, Vector2 force, float direction)
        {
            if ((string)b.Tag == "player")
            {
                Manager.Scene.FindByName("player").Enabled = false;
                b.Fixed = true;

                Manager.Vars["currentLevel"] = (int)Manager.Vars["currentLevel"] + 1;
                
                if ((int)Manager.Vars["currentLevel"] > (int)Manager.Vars["lastLevel"])
                    Manager.Vars["lastLevel"] = (int)Manager.Vars["currentLevel"];

                Manager.Vars["score"] = (int)Manager.Vars["score"] + 100;
                Manager.Vars["prev_CMYK_Ammo"] = (int)Manager.Vars["CMYK_Ammo"];
                Manager.Vars["exitLevel"] = true;
                                
                // Guardamos los progresos:
                Session.GameProgress.CurrentLevel = (int)Manager.Vars["lastLevel"];
                Session.GameProgress.CurrentCMYKAmmo = (int)Manager.Vars["CMYK_Ammo"];
                Session.GameProgress.CurrentScore = (int)Manager.Vars["score"];

                if (!Guide.IsTrialMode)
                {
                    StorageSession.SaveData(Session.SaveGameFilename, Session.GameProgress);
                }

                Kill();
            }
        }
    }
}

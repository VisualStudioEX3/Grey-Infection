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
    /// <summary>
    /// Estados de Joss Stress.
    /// </summary>
    /// <remarks>Los estados permiten configurar animaciones completas o parciales del personaje.</remarks>
    public enum JossStressStates
    {
        Idle, Run, Jump, ShootRGBGun, ShootCMYKGun, SetCMYKGun, SetRGBGun, Dead, Crouch, Crouching, UnCrouching, RunCrouch
    }

    /// <summary>
    /// Motor de animaciones procedural de Joss Stress.
    /// </summary>
    /// <remarks>Sistema que genera las diferentes animaciones del personaje segun estados.</remarks>
    public class JossStressAnimationEngine : TLSA.Engine.IDrawableAndUpdateable
    {
        #region Miembros privados
        private Vector2 location = Vector2.Zero;
        private SpriteEffects mirror = SpriteEffects.None;
        private BlendFilters effect = BlendFilters.AlphaBlending;
        private Color color = Color.White;

        // Sprites que forman a Joss:
        private Sprite body, arms;

        private Timer timer;

        private bool cmykGunSelected;

        private bool isCrouch;
        private bool isRunning;
        private Vector2 idleOffset, CrouchOffset, left_gunOffset, right_gunOffset;

        private string prevBodyAnimation = "";
        #endregion
        
        #region Propiedades
        /// <summary>
        /// Posicion de Joss.
        /// </summary>
        /// <remarks>Define su centro.</remarks>
        public Vector2 Location 
        {
            get { return location; }
            set
            {
                location = value;
                body.Location = value - (isCrouch ? CrouchOffset : Vector2.Zero);
                if (isRunning)
                {
                    arms.Center = true;
                    arms.Location = value;
                }
                else
                {
                    arms.Center = false;
                    arms.Location = value - (mirror == SpriteEffects.None ? right_gunOffset : left_gunOffset);
                }
            }
        }

        /// <summary>
        /// Orientacion de Joss.
        /// </summary>
        public SpriteEffects Mirror 
        {
            get { return mirror; }
            set
            {
                mirror = value;
                arms.Mirror = mirror;
                body.Mirror = mirror;                
            }
        }

        /// <summary>
        /// Efecto de mezclado de colores de Joss.
        /// </summary>
        public BlendFilters BlendingEffect 
        {
            get { return effect; }
            set
            {
                effect = value;
                body.BlendingEffect = effect;
                arms.BlendingEffect = effect;
            }
        }

        /// <summary>
        /// Color de tintado de Joss.
        /// </summary>
        public Color Color
        { 
            get { return color; }
            set
            {
                color = value;
                body.Color = color;
                arms.Color = color;
            }
        }

        public bool Enabled { get; set; }
        public bool Visible { get; set; } 
        #endregion

        #region Constructor de la clase
        public JossStressAnimationEngine(string Texture, string AnimationSet)
        {
            // Importamos la textura de JossStress:
            Manager.Graphics.LoadTexture(Texture);

            body = new Sprite(Texture);
            body.Center = true;
            body.Scale = 0.9f;
            body.Animations.AddSecuence("run", new Rectangle(0, 0, 70, 97), 12, 55, true);
            body.Animations.AddSecuence("run_crouch", new Rectangle(70, 291, 70, 97), 8, 140, true);
            body.Animations.AddSecuence("jump", new Rectangle(0, 388, 70, 97), 4, 100, false);
            body.Animations.AddSecuence("death", new Rectangle(0, 485, 70, 97), 10, 120, false);
            body.Animations.AddSecuence("idle", new Rectangle(0, 582, 70, 97), 2, 1000, true);
            body.Animations.AddSecuence("idle_crouch", new Rectangle(0, 679, 70, 97), 1, 0, false);
            body.Animations.Play("idle");

            arms = new Sprite(Texture);
            //arms.Center = true;
            arms.Scale = 0.9f;
            arms.Animations.AddSecuence("rgbgun_idle", new Rectangle(0, 776, 70, 41), 1, 0, false);
            arms.Animations.AddSecuence("cmykgun_idle", new Rectangle(0, 817, 70, 41), 1, 0, false);
            arms.Animations.AddSecuence("rgbgun_run", new Rectangle(2, 97, 70, 97), 12, 70, true);
            arms.Animations.AddSecuence("cmykgun_run", new Rectangle(0, 194, 70, 95), 12, 70, true);
            arms.Animations.AddSecuence("rgbgun_shoot", new Rectangle(0, 776, 70, 41), 9, 70, false);
            arms.Animations.AddSecuence("cmykgun_shoot", new Rectangle(0, 817, 70, 41), 9, 70, false);
            arms.Animations.Play("rgbgun_shoot");

            // Orientacion inicial a la derecha:
            Mirror = SpriteEffects.None;
            Enabled = true;
            Visible = true;

            cmykGunSelected = false;

            isCrouch = false;
            isRunning = false;

            idleOffset = new Vector2(0, 16);
            CrouchOffset = new Vector2(0, 16);

            right_gunOffset = new Vector2(18, 32);
            left_gunOffset = new Vector2(46, 32);

            timer = new Timer();
        } 
        #endregion

        #region Metodos y funciones publicos
        /// <summary>
        /// Actualizamos los estados de los sprites.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                // Corrige el angulo de los brazos al realizar los cambios de arma:
                if (arms.Angle != 0)
                {
                    if (mirror == SpriteEffects.None)
                        arms.Angle -= 5;
                    else
                        arms.Angle += 5;
                }

                // Actualizamos los estados de cada spritie:
                body.Update(gameTime);
                arms.Update(gameTime);
            }
        }

        /// <summary>
        /// Dibujamos los sprites.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                body.Draw(gameTime);
                arms.Draw(gameTime);
            }
        }

        /// <summary>
        /// Establece un estado de Joss.
        /// </summary>
        /// <param name="State">Estado a establecer.</param>
        /// <remarks>Los estados pueden definir diferentes animaciones de cada parte de Joss de forma independiente.</remarks>
        public void SetState(JossStressStates State)
        {
            switch (State)
            {
                case JossStressStates.Idle:
                    body.Animations.Play("idle");
                    if ((arms.Animations.CurrentSecuence == "rgbgun_run" || arms.Animations.CurrentSecuence == "cmykgun_run") ||
                        ((arms.Animations.CurrentSecuence == "rgbgun_shoot" || arms.Animations.CurrentSecuence == "cmykgun_shoot")
                        && arms.Animations.IsEnded))
                        arms.Animations.Play(cmykGunSelected ? "cmykgun_idle" : "rgbgun_idle");
                    isCrouch = false;
                    isRunning = false;
                    break;
                case JossStressStates.Run:
                    body.Animations.Play("run");
                    if (arms.Animations.CurrentSecuence != "rgbgun_shoot" && arms.Animations.CurrentSecuence != "cmykgun_shoot")
                    {
                        isRunning = true;
                        arms.Animations.Play(cmykGunSelected ? "cmykgun_run" : "rgbgun_run");
                    }
                    else
                        isRunning = false;

                    isCrouch = false;
                    break;
                case JossStressStates.Crouching:
                    body.Animations.Play("idle_crouch");
                    isCrouch = true;
                    isRunning = false;
                    break;
                case JossStressStates.RunCrouch:
                    body.Animations.Play("run_crouch");
                    //if (prevBodyAnimation != body.Animations.CurrentSecuence)
                    //    body.Animations.CurrentFrameIndex = 4;
                    if ((arms.Animations.CurrentSecuence == "rgbgun_run" || arms.Animations.CurrentSecuence == "cmykgun_run") ||
                        ((arms.Animations.CurrentSecuence == "rgbgun_shoot" || arms.Animations.CurrentSecuence == "cmykgun_shoot")
                        && arms.Animations.IsEnded))
                        arms.Animations.Play(cmykGunSelected ? "cmykgun_idle" : "rgbgun_idle");

                    isCrouch = true;
                    isRunning = false;
                    break;
                case JossStressStates.Crouch:
                    body.Animations.Play("idle_crouch");
                    //if ((arms.Animations.CurrentSecuence == "rgbgun_shoot" || arms.Animations.CurrentSecuence == "cmykgun_shoot")
                    //    && arms.Animations.IsEnded)
                    if ((arms.Animations.CurrentSecuence == "rgbgun_run" || arms.Animations.CurrentSecuence == "cmykgun_run") ||
                        ((arms.Animations.CurrentSecuence == "rgbgun_shoot" || arms.Animations.CurrentSecuence == "cmykgun_shoot")
                        && arms.Animations.IsEnded))
                        arms.Animations.Play(cmykGunSelected ? "cmykgun_idle" : "rgbgun_idle");
                    isCrouch = true;
                    isRunning = false;
                    break;
                case JossStressStates.Dead:
                    arms.Visible = false;
                    body.Animations.Play("death");
                    break;
                case JossStressStates.SetRGBGun:
                    arms.Animations.Play("rgbgun_shoot");
                    cmykGunSelected = false;
                    //if (mirror == SpriteEffects.None)
                    //    arms.Angle = 45;
                    //else
                    //    arms.Angle = -45;
                    break;
                case JossStressStates.SetCMYKGun:
                    cmykGunSelected = true;
                    arms.Animations.Play("cmykgun_shoot");
                    //if (mirror == SpriteEffects.None)
                    //    arms.Angle = 45;
                    //else
                    //    arms.Angle = -45;
                    break;
                case JossStressStates.Jump:
                    if (body.Animations.CurrentSecuence != "jump") body.Animations.Play("jump");

                    if (arms.Animations.CurrentSecuence != "rgbgun_shoot" && arms.Animations.CurrentSecuence != "cmykgun_shoot")
                    {
                        isRunning = true;
                        arms.Animations.Play(cmykGunSelected ? "cmykgun_run" : "rgbgun_run");
                    }
                    else
                        isRunning = false;

                    break;
                case JossStressStates.ShootRGBGun:
                    arms.Animations.Play("rgbgun_shoot");
                    break;
                case JossStressStates.ShootCMYKGun:
                    arms.Animations.Play("cmykgun_shoot");
                    break;
            }
            prevBodyAnimation = body.Animations.CurrentSecuence;
        }
        #endregion
    }
}
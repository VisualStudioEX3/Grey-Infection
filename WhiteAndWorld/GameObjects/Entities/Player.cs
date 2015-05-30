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
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities
{
    /// <summary>
    /// Clase del Jugador.
    /// </summary>
    /// <remarks>Implementa el motor de animaciones de JossStress.</remarks>
    public class Player : Entity
    {
        private JossStressAnimationEngine joss;
        private Body headBox, legsBox, footsBox;

        private bool isCrouch = false;
        private bool isOnAir = false;
        private bool canUnCrouch = true;

        public bool IsDead { get; set; }
        public bool IsReady { get; set; }

        private Vector2 idleSize = new Vector2(42, 84);
        private Vector2 crouchSize = new Vector2(42, 56);
        
        // Variables auxiliares (las utilizaremos para ahorrar news):
        private Rectangle box = Rectangle.Empty;
        private Vector2 loc = Vector2.Zero;
        private bool resizeBox = true; // Controla cuando se reescala la caja de colision segun la posicion del personaje.

        private Timer startInput;
        private bool startInputOn = false;

        private SoundPlayer selectRGBGun, selectCMYKGun, shootRGBGun, shootCMYKGun, death, noammo;

        public override Vector2 Location
        {
            get
            {
                if (base.IsInitialized) 
                {
                    loc.X = legsBox.Bounds.X;
                    loc.Y = legsBox.Bounds.Y;
                    return loc;
                }
                else
                    return Vector2.Zero;
            }
            set
            {
                if (base.IsInitialized)
                {
                    box.X = (int)value.X;
                    box.Y = (int)value.Y;
                    if (isCrouch)
                    {
                        box.Width = (int)crouchSize.X;
                        box.Height = (int)crouchSize.Y;
                    }
                    else
                    {
                        box.Width = (int)idleSize.X;
                        box.Height = (int)idleSize.Y;
                    }

                    legsBox.Bounds = box;

                    UpdateLocation();
                }
            }
        }

        public override void Initialize()
        {
            // Inicializamos el motor de animaciones procedurales de Joss Stress:
            joss = new JossStressAnimationEngine("SpritesJoss", "");
            joss.Update(new GameTime());

            // Caja fisica para la cabeza. Se utilizara para detectar el techo cuando el jugador
            // este agachado para determinar si se puede levantar o no:
            headBox = new Body(Manager.PhysicEngine, new Rectangle(0, 0, 42, 28), 1, false);
            headBox.Tag = "playerHeadSensor";
            headBox.Trigger = true;
            headBox.OnCollision += HeadSensorCollision;
            Manager.PhysicEngine.Bodies.Add(headBox);

            // Caja fisica principal. Esta determina la posicion del jugador:
            legsBox = new Body(Manager.PhysicEngine, new Rectangle(0, 0, 42, 84), 1, false);
            legsBox.Tag = "player";
            legsBox.OnCollision += OnCollision;
            Manager.PhysicEngine.Bodies.Add(legsBox);

            // Sensor de suelo:
            footsBox = new Body(Manager.PhysicEngine, new Rectangle(0, 0, 36, 2), 0, false);
            footsBox.Tag = "playerFoots";
            footsBox.Trigger = true;
            footsBox.OnCollision += JumpSensorCollision;
            footsBox.OnCollision += OnCollision;
            Manager.PhysicEngine.Bodies.Add(footsBox);

            if ((bool)Manager.Vars["playerSelectCMYKGun"] == true)
                joss.SetState(JossStressStates.SetCMYKGun);

            startInput = new Timer();

            // Cargamos los sonidos:
            selectRGBGun = new SoundPlayer(@"Audio\FX\rgb_select");
            selectCMYKGun = new SoundPlayer(@"Audio\FX\cmyk_select");
            shootRGBGun = new SoundPlayer(@"Audio\FX\rgb_shoot");
            shootCMYKGun = new SoundPlayer(@"Audio\FX\cmyk_shoot");
            death = new SoundPlayer(@"Audio\FX\joss_death");
            noammo = new SoundPlayer(@"Audio\FX\noammo");

            this.Tag = "player";

            this.IsDead = false;
            this.IsReady = false;

            this.Priority = 9999;   // Se ejecutara en primer lugar antes que el resto de componentes.
            this.ZOrder = -1;       // Se dibujara por detras de los tiles y por encima de los enemigos.

            base.Initialize();
        }

        public override void Terminate()
        {
            Manager.PhysicEngine.Bodies.Remove(headBox);
            Manager.PhysicEngine.Bodies.Remove(legsBox);
            Manager.PhysicEngine.Bodies.Remove(footsBox);
            base.Terminate();
        }

        // Codigo con la logica de muerte del jugador:
        private void KillPlayer(bool disablePhysics)
        {
            death.Play();

            // Reproducimos la animacion de muerte de Joss Stress:
            if (disablePhysics) joss.SetState(JossStressStates.Dead);
            this.IsDead = true;

            // Desactivamos sus colisiones:
            headBox.Enabled = !disablePhysics;
            legsBox.Enabled = !disablePhysics;
            footsBox.Enabled = !disablePhysics;

            // Descontamos los puntos ganados en este nivel:
            Manager.Vars["score"] = (int)Manager.Vars["score"] - (int)Manager.Vars["currentLevelScore"];

            // Creamos una instancia del componente de reinicio de nivel:
            Manager.Scene.AddEntity(new ResetLevel());
        }

        // Evento de colision principal del jugador:
        private void OnCollision(Body b, Vector2 force, float direction)
        {
            if (!this.IsDead && ((string)b.Tag == "enemy" && !b.Trigger))
                this.KillPlayer(true);
            else if (!this.IsDead && (string)b.Tag == "deathZone")
                this.KillPlayer(true);
        }

        private void JumpSensorCollision(Body b, Vector2 force, float direction)
        {
            if (b.Id != legsBox.Id && b.Id != headBox.Id && b.Id != footsBox.Id && this.canUnCrouch) 
                isOnAir = false;
        }

        private void HeadSensorCollision(Body b, Vector2 force, float direction)
        {
            if (b.Id != legsBox.Id && (string)b.Tag != "Exit") this.canUnCrouch = false;
        }

        public override void ReciveMessage(string message, params object[] values)
        {
            switch (message)
            {
                case "ready": this.IsReady = true; break;
                case "dead":  this.KillPlayer((bool)values[0]); break;
            }
        }

        /// <summary>
        /// Actualiza todos los estados del jugador.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if ((bool)Manager.Vars["showMessagePause"] == true) return;

            if (!startInputOn && startInput.Value >= 500) startInputOn = true; 
            
            if (startInputOn) UpdateInput();

            if (resizeBox)
            {
                if (isCrouch)
                {
                    box.Width = (int)crouchSize.X;
                    box.Height = (int)crouchSize.Y;
                    box.X = (int)legsBox.Bounds.X;
                    box.Y = (int)legsBox.Location.Y - box.Height / 2;
                }
                else
                {
                    box.Width = (int)idleSize.X;
                    box.Height = (int)idleSize.Y;
                    box.X = (int)legsBox.Bounds.X;
                    box.Y = (int)legsBox.Location.Y - box.Height / 2;
                }
                legsBox.Bounds = box;
                resizeBox = false;
            }

            UpdateLocation();

            Manager.GameInstance.Window.Title = Location.ToString();

            joss.Update(gameTime);

            headBox.Enabled = isCrouch;

            canUnCrouch = true;
            isOnAir = true;
        }

        // Actualiza las posiciones de los cuerpos fisicos y sprites del jugador:
        private void UpdateLocation()
        {
            loc.X = (int)legsBox.Location.X;
            loc.Y = (int)legsBox.Location.Y - (isCrouch ? 42 : 28);
            headBox.Location = loc;

            loc.X = (int)legsBox.Location.X;
            loc.Y = (int)legsBox.Bounds.Bottom;
            footsBox.Location = loc;

            loc.X = (int)(this.joss.Mirror == SpriteEffects.None ? legsBox.Location.X - 2 : legsBox.Location.X + 2);
            loc.Y = (int)(this.isCrouch ? legsBox.Location.Y + 2 : legsBox.Location.Y - 2);

            joss.Location = loc;
        }

        // Actualiza los estados de input del jugador:
        private void UpdateInput()
        {
            // Actualizamos estados si no esta mostrado el mensaje de pausa, no esta muerto el jugador y esta preparado:
            if ((bool)Manager.Vars["showMessagePause"] == false && (bool)Manager.Vars["showMessageReady"] == false && !IsDead && IsReady)
            {
                if (Manager.UIInput.Hit("pause"))
                {
                    Manager.Vars["showMessagePause"] = true;
                }

                if (Manager.UIInput.Press("crouch") && !isOnAir)
                {
                    isCrouch = true;
                    resizeBox = true;
                }
                else if (Manager.UIInput.Release("crouch") && canUnCrouch)
                {
                    isCrouch = false;
                    resizeBox = true;
                }

                if (Manager.UIInput.Press("right"))
                {
                    joss.Mirror = SpriteEffects.None;
                    if (isCrouch)
                    {
                        legsBox.Move(1, 0);
                        joss.SetState(JossStressStates.RunCrouch);
                    }
                    else 
                    {
                        legsBox.Move(3, 0);
                        if (!isOnAir) joss.SetState(JossStressStates.Run); 
                    }
                }
                else if (Manager.UIInput.Press("left"))
                {
                    joss.Mirror = SpriteEffects.FlipHorizontally;
                    if (isCrouch)
                    {
                        legsBox.Move(1, 180);
                        joss.SetState(JossStressStates.RunCrouch);
                    }
                    else
                    {
                        legsBox.Move(3, 180);
                        if (!isOnAir) joss.SetState(JossStressStates.Run);
                    }
                }
                else
                {
                    if (isCrouch)
                        joss.SetState(JossStressStates.Crouch);
                    else 
                        if (!isOnAir) joss.SetState(JossStressStates.Idle);
                }

                if (isOnAir && canUnCrouch)
                    joss.SetState(JossStressStates.Jump);
                
                if (Manager.UIInput.Hit("jump") && !isOnAir && !isCrouch)
                {
                    joss.SetState(JossStressStates.Jump);
                    legsBox.ApplyForce(16, 90);
                }
                
                if (Manager.UIInput.Hit("shoot"))
                {
                    if ((int)Manager.Vars["shoots"] < (int)Manager.Vars["bullets"])
                    {
                        if ((bool)Manager.Vars["playerSelectCMYKGun"] == false ||
                            ((bool)Manager.Vars["playerSelectCMYKGun"] == true && (int)Manager.Vars["CMYK_Ammo"] > 0))
                        {
                            Bullet bullet = new Bullet();
                            Manager.Scene.AddEntity(bullet);
                            bullet.Location = new Vector2(joss.Location.X + (joss.Mirror == SpriteEffects.None ? 40 : -40), 
                                                          joss.Location.Y - 16);
                            bullet.Direction = joss.Mirror;
                            if ((bool)Manager.Vars["playerSelectCMYKGun"] == true)
                            {
                                shootCMYKGun.Play();
                                joss.SetState(JossStressStates.ShootCMYKGun);
                                bullet.Tag = "CMYK_Bullet";
                                Manager.Vars["CMYK_Ammo"] = (int)Manager.Vars["CMYK_Ammo"] - 1;
                            }
                            else
                            {
                                shootRGBGun.Play();
                                joss.SetState(JossStressStates.ShootRGBGun);
                                bullet.Tag = "RGB_Bullet";
                            }
                        }
                        else if ((bool)Manager.Vars["playerSelectCMYKGun"] == true && (int)Manager.Vars["CMYK_Ammo"] == 0)
                        {
                            noammo.Play();
                            selectRGBGun.Play();
                            Manager.Vars["playerSelectCMYKGun"] = false;
                            joss.SetState(JossStressStates.SetRGBGun);
                        }
                    }
                }

                if (Manager.UIInput.Hit("switch"))
                {
                    Manager.Vars["playerSelectCMYKGun"] = !(bool)Manager.Vars["playerSelectCMYKGun"];
                    if ((bool)Manager.Vars["playerSelectCMYKGun"] == true)
                    {
                        selectCMYKGun.Play();
                        joss.SetState(JossStressStates.SetCMYKGun);
                    }
                    else
                    {
                        selectRGBGun.Play();
                        joss.SetState(JossStressStates.SetRGBGun);
                    }
                }
            }
        }

        /// <summary>
        /// Dibuja al jugador.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            joss.Draw(gameTime);
        }
    }
}
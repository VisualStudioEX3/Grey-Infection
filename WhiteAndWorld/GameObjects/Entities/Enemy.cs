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
    public class Enemy : Entity
    {
        private RectangleOverlay enemyBox;
        private LineOverlay linePath;
        private Body box;
        private Timer respawnTimer;
        private int respawnTime;
        private Timer invincibleTimer;
        private int invincibleTime;         // Para evitar que se desincronice el tiempo de invencibilidad al realizar
        // la pausa desde el menu usaremos un contador de unidades.
        private const int invincibleTimeDelay = 10; // Tiempo, en segundos, maximo de invencibilidad.

        private float direction;            // Angulo que define la direccion a mover el enemigo en la actualizacion.
        private Vector2 a, b;               // Puntos para definir las rutas y su direccion.

        private EnemyAnimationEngine enemy;
        private Sprite shield;

        private GlitchColor glitch;

        // La propiedad nos devolvera la posicion de la esquina izquierda superior y el valor a establecer sera la
        // misma esquina:
        public override Vector2 Location
        {
            get { return box.Location - this.box.Size / 2; }
            set
            {
                if (box != null)
                {
                    box.Location = value + this.box.Size / 2;
                    enemy.Location = box.Location;
                }
            }
        }

        public override string Tag
        {
            get { return (string)this.box.Tag; }
            set { if (this.box != null) this.box.Tag = value; }
        }

        #region Parametros de configuracion del enemigo
        private int type = 0;
        public int Type                                 // Tipo del enemigo.
        {
            get { return this.type; }
            set
            {
                this.type = value;
                this.isSetup = false;
            }
        }

        private int behavior = 0;
        public int Behavior                             // Comportamiento/movimiento del enemigo.
        {
            get { return this.behavior; }
            set
            {
                this.behavior = value;
                this.isSetup = false;
            }
        }
        
        private int pathLength = 0;
        public int PathLength                           // Distancia que recorrera en su ruta.
        {
            get { return this.pathLength; }
            set
            {
                this.pathLength = value;
                this.isSetup = false;
            }
        }             
        public string Target { get; set; }              // Tag de los elementos que activa al morir.
        public bool Respawn { get; set; }               // Indica si el enemigo puede resucitar tras un tiempo de espera.
        public int RespawnDelay { get; set; }           // Indica el tiempo de espera tras la muerte para resucitar.
        
        private bool reversePathAtStart = false;
        public bool ReversePathAtStart                  // Indica si el enemigo comienza su ruta a la inversa.
        {
            get { return this.reversePathAtStart; }
            set
            {
                this.reversePathAtStart = value;
                this.isSetup = false;
            }
        }

        private int action = 0;
        public int Action // Accion que aplicara a los tiles objetivo.
        {
            get { return action; }
            set
            {
                this.action = value;
                this.enemy.Type = value;
            }
        }                 
        public int Step { get; set; }                   // Distancia que recorrera el objeto en cada actualizacion.
        public bool Invincible { get; set; }            // Indica si puede matarse con disparos normales.

        private bool isSetup = false;                   // Lo usaremos para inicializar la logica del enemigo una vez establezcamos los parametros.
        #endregion

        public bool IsDead                              // Indica si el enemigo esta muerto.
        {
            get { return !this.box.Enabled; }
            set { this.box.Enabled = !value; this.Visible = !value; }
        }

        public bool IsReady { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            enemyBox = new RectangleOverlay(new Rectangle(0, 0, 32, 32));
            enemyBox.Visible = false;

            // Creamos una instancia del motor de animaciones de enemigos:
            enemy = new EnemyAnimationEngine("enemy");

            // Cargamos la textura que hara de escudo si la particula es invencible y la configuramos:
            shield = new Sprite(Manager.Graphics.LoadTexture("enemy_shield"));
            shield.Animations.AddSecuence("default", new Rectangle(0, 0, 48, 48), 2, 1250, true);
            shield.Animations.Play("default");
            shield.Center = true;
            shield.Visible = false;
            shield.Enabled = false;

            this.Bounds = enemyBox.Bounds;
            
            box = new Body(Manager.PhysicEngine, this.Bounds, 1, true);
            box.OnCollision += this.OnCollision;
            Manager.PhysicEngine.Bodies.Add(box);

            this.Tag = "enemy";
            this.Invincible = false;
            this.IsDead = false;
            this.IsReady = false;

            this.respawnTimer = new Timer();
            this.respawnTime = 0;

            this.invincibleTimer = new Timer();
            this.invincibleTime = 0;

            this.ZOrder = -1;
        }

        private void OnCollision(Body b, Vector2 force, float direction)
        {
            if (((string)b.Tag == "RGB_Bullet" && !this.Invincible) || (string)b.Tag == "CMYK_Bullet")
            {
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
                switch (this.Action)
                {
                    case 1: Manager.Messages.SendMessage(this.Target, "hit", true); break;
                    case 2: Manager.Messages.SendMessage(this.Target, "hit", false); break;
                    case 3: 
                        Manager.Messages.SendMessage("", "reset");
                        Manager.Vars["CMYK_Ammo"] = (int)Manager.Vars["prev_CMYK_Ammo"];
                        break;
                    case 4: Manager.Messages.SendMessage("player", "dead", true); break;
                    case 5: Manager.Messages.SendMessage("", "ghost"); break; // Se envia un mensaje a toda la cola.
                    case 6: Manager.Messages.SendMessage("tileGroupQuery", "show"); break;
                    case 7: Manager.Vars["CMYK_Ammo"] = (int)Manager.Vars["CMYK_Ammo"] + 3; break;
                    case 8:
                        if ((int)Manager.Vars["CMYK_Ammo"] > 0)
                        {
                            Manager.Vars["CMYK_Ammo"] = (int)Manager.Vars["CMYK_Ammo"] - 3;
                            if ((int)Manager.Vars["CMYK_Ammo"] < 0) Manager.Vars["CMYK_Ammo"] = 0;
                        }
                        break;
                    case 9: break;
                    case 10: break;
                    case 11: Manager.Messages.SendMessage("enemy", "playerIsInvincible"); break;
                    default: break;
                }

                this.IsDead = true;

                // Creamos una entidad de animacion de muerte y la agregamos a la escena:
                Enemy_Death enemy_death = new Enemy_Death();
                Manager.Scene.AddEntity(enemy_death);
                enemy_death.Location = this.Location;

                if (this.Respawn)
                {
                    this.respawnTimer.Reset();
                    this.respawnTime = this.RespawnDelay;
                }

                // Agregamos 20 puntos al marcador:
                Manager.Vars["score"] = (int)Manager.Vars["score"] + 20;
                Manager.Vars["currentLevelScore"] = (int)Manager.Vars["currentLevelScore"] + 20;

                if (Action < 3) Manager.Vars["enemiesLeft"] = (int)Manager.Vars["enemiesLeft"] - 1;

            }
        }

        public override void Terminate()
        {
            Manager.PhysicEngine.Bodies.Remove(box);
            base.Terminate();
        }

        public override void ReciveMessage(string message, params object[] values)
        {
            switch (message)
            {
                case "respawn": case "reset":
                    this.IsDead = false;
                    break;
                case "ready":
                    this.IsReady = true;
                    break;
                case "playerIsInvincible": 
                    this.invincibleTimer.Reset();
                    this.invincibleTime = invincibleTimeDelay;
                    this.box.Trigger = true;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if ((bool)Manager.Vars["showMessagePause"] == true) return;

            // Controlamos el tiempo de invicibilidad incluso habiendo pausa:
            if (this.invincibleTimer.Value >= 1000 && this.invincibleTime > 0)
            {
                this.invincibleTimer.Reset();
                this.invincibleTime--;
                if (this.invincibleTime == 0) this.box.Trigger = false;
            }

            // Si se modifico algun parametro principal de la logica reconfiguramos el resto de valores:
            if (!this.isSetup)
            {
                switch (this.behavior)
                {
                    case 1: // Horizontal
                        this.direction = !this.reversePathAtStart ? 0 : 180;
                        break;
                    case 2: // Vertical
                        this.direction = !this.reversePathAtStart ? 90 : 270;
                        break;
                    case 3: // Diagonal 1 (\)
                        this.direction = !this.reversePathAtStart ? 45 : 225;
                        break;
                    case 4: // Diagonal 2 (/)
                        this.direction = !this.reversePathAtStart ? 135 : 315;
                        break;
                }
                this.a = this.Location;
                this.b = MathTools.Move(this.a, this.pathLength, this.direction);
                this.linePath = new LineOverlay(a, b, Color.Red); 
                this.isSetup = true;
            }

            if (this.IsReady)
            {
                // Si esta muerto y puede resucitar...
                // Controlamos el tiempo de resurreccion incluso habiendo pausa:
                if ((this.IsDead && this.Respawn) && (this.respawnTimer.Value >= 1000 && this.respawnTime > 0))
                {
                    this.respawnTimer.Reset();
                    this.respawnTime--;
                    if (this.respawnTime == 0) this.IsDead = false;
                }
                else if (this.behavior > 0) // Si no es estatico actualizamos su movimiento.
                {
                    // Cuando la distancia de {a - location} sea mayor o igual que {a - b} cambiamos invertimos la direccion:
                    if (Vector2.Distance(this.a, this.Location) >= Vector2.Distance(this.a, this.b))
                    {
                        Helper.Swap<Vector2>(ref this.a, ref this.b);
                        this.direction = MathTools.GetAngle(this.a, this.b);
                    }
                    this.box.Move(this.Step, this.direction);
                } 
            }

            this.enemy.Location = this.Location;
            this.enemy.Flag = this.Invincible;
            this.enemy.Update(gameTime);
            this.shield.Update(gameTime);

            if (Invincible)
            {
                shield.Visible = true;
                shield.Enabled = true;
                shield.Angle += 0.5f;
                if (shield.Angle > 360) shield.Angle = 0;
                shield.Location = this.Location + box.Size / 2;
            }
            else
            {
                shield.Visible = false;
                shield.Enabled = false;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            this.enemy.Draw(gameTime);
            this.shield.Draw(gameTime);
        }
    }
}
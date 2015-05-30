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

namespace WhiteAndWorld.GameObjects.States
{
    public class Game : StateComponent 
    {
        List<string> texturesTiles, texturesClues;

        public override void Initialize()
        {
            MusicPlayer.Play(MusicPlayer.LevelTheme);

            Manager.Scene.Enabled = true;
            Manager.Scene.Visible = true;

            // Definimos el origen de la escena de juego:
            Manager.Graphics.OffSet = new Vector2(128, 72);

            // Variables de escena:
            Manager.Vars.Create("shoots", 0);                   // Variable que contara los disparos activos del jugador.
            Manager.Vars.Create("bullets", 3);                  // Numero maximo de disparos en pantalla.
            Manager.Vars.Create("playerSelectCMYKGun", false);  // Indica si el jugador tiene seleccionada el arma CMYK.
            Manager.Vars.Create("currentLevelScore", 0);        // Contador de puntuacion del nivel, si muere el player se resta al marcador principal.
            Manager.Vars.Create("enemiesLeft", 0);              // Contador de enemigos vivos que quedan para activar la salida.
            Manager.Vars.Create("exitLevel", false);            // Indica si se carga el siguiente nivel.

            // Determinamos si cargamos progresos del jugador o no:
            if ((bool)Manager.Vars["loadSaveGame"] == true)
            {
                Manager.Vars["loadSaveGame"] = false;
            }
            else if ((int)Manager.Vars["currentLevel"] == 1)
            {
                // Primer nivel del juego y sin municion especial:
                Manager.Vars["CMYK_Ammo"] = 0;
                Manager.Vars["score"] = 0;
                Manager.Vars["firstGame"] = false;
            }

            Manager.Vars["CMYK_Ammo"] = (int)Manager.Vars["prev_CMYK_Ammo"];

            // Cargamos las texturas de tiles y clues en memoria:
            this.LoadTextureTiles();
            this.LoadTextureClues();

            // Cargamos el nivel seleccionado (si no se cargo progreso alguno se carga el primer nivel):
            this.LoadLevel();  
        }

        // Carga todas las texturas de tiles en memoria:
        private void LoadTextureTiles()
        {
            this.texturesTiles = new List<string>();

            string assetName;
            foreach (string file in System.IO.Directory.GetFiles(Manager.Content.RootDirectory + @"\Tiles\", "*.*"))
            {
                assetName = file.Replace(".xnb", "").Replace(Manager.Content.RootDirectory + @"\", "");
                this.texturesTiles.Add(assetName);
                Manager.Graphics.LoadTexture(assetName);
            }
        }

        // Carga todas las texturas de clues en memoria:
        private void LoadTextureClues()
        {
            this.texturesClues = new List<string>();

            string assetName;
            foreach (string file in System.IO.Directory.GetFiles(Manager.Content.RootDirectory + @"\Clues\", "*.*"))
            {
                assetName = file.Replace(".xnb", "").Replace(Manager.Content.RootDirectory + @"\", "");
                this.texturesClues.Add(assetName);
                Manager.Graphics.LoadTexture(assetName);
            }
        }

        // Carga el nivel indicado en la variable "currentLevel":
        private void LoadLevel()
        {// Comprobamos si hemos llegado al final del juego en modo trial:
            if (Guide.IsTrialMode && (int)Manager.Vars["currentLevel"] > 7)
            {
                Manager.Vars["purchaseAndReturnMenu"] = true;
                Manager.GameStates.ChangeState("endtrial");
            }
            // Comprobamos si hemos llegado al final del juego en modo completo:
            else if (!Guide.IsTrialMode && (int)Manager.Vars["currentLevel"] > 32)
            {
                Manager.GameStates.ChangeState("gameover");
            }
            // Cargamos el nivel:
            else
            {
                // Descargamaos previamente el nivel actual:
                UnloadLevel();

                // Area del escenario para definir los elementos:
                Rectangle boxArea = new Rectangle(0, 0, 1024, 576);

                // Marco del escenario para delimitar el area de accion del jugador:
                {
                    Manager.PhysicEngine.Bodies.Add(new Body(Manager.PhysicEngine, new Rectangle(boxArea.X, boxArea.Y - 64, boxArea.Width, 64), 1, true));
                    Manager.PhysicEngine.Bodies.Add(new Body(Manager.PhysicEngine, new Rectangle(boxArea.X - 64, 0, 64, boxArea.Height), 1, true));
                    Manager.PhysicEngine.Bodies.Add(new Body(Manager.PhysicEngine, new Rectangle(boxArea.Width, 0, 64, boxArea.Height), 1, true));
                }

                // Area de muerte: la zona inferior de la escena matara al jugador si hay colision:
                {
                    Body deathTrigger = new Body(Manager.PhysicEngine, new Rectangle(boxArea.X, boxArea.Height, boxArea.Width, 64), 0);
                    deathTrigger.Tag = "deathZone";
                    deathTrigger.Trigger = true;
                    deathTrigger.OnCollision += this.OnCollisionDeathZone;
                    Manager.PhysicEngine.Bodies.Add(deathTrigger);
                }

                // Cargamos los objetos de la escena:
                try
                {
                    GameDefinitions.LevelData level = Serializers.DeserializeFromTitleStorage<GameDefinitions.LevelData>("GameData/Levels/" + (Guide.IsTrialMode ? "Trial/" : "") + Manager.Vars["currentLevel"].ToString() + ".xml");
                    {
                        // Agregamos las pistas:
                        Clue clue;
                        foreach (GameDefinitions.Clue c in level.Clues)
                        {
                            if (c.Texture != "clue_exit" && !c.Texture.StartsWith("level_"))
                            {
                                clue = new Clue();
                                Manager.Scene.AddEntity(clue);
                                clue.Texture = c.Texture;
                                clue.Location = c.Location;
                                clue.Update(Manager.GameTime);
                            }
                        }

                        // Agregamos los tiles:
                        Tile tile;
                        foreach (GameDefinitions.Tile t in level.Tiles)
                        {
                            tile = new Tile();
                            Manager.Scene.AddEntity(tile);
                            tile.Texture = t.Texture;
                            tile.Location = t.Location;
                            tile.Tag = t.Tag;
                            tile.Visible = t.Active;
                            tile.Update(Manager.GameTime);
                        }

                        // Agregamos una entidad TileGroupQuery para el power-up de mostrar el camino completo:
                        TileGroupQuery tileGroupQuery = new TileGroupQuery();
                        Manager.Scene.AddEntity(tileGroupQuery, "tileGroupQuery");

                        // Agregamos los enemigos:
                        Enemy enemy;
                        foreach (GameDefinitions.Enemy e in level.Enemies)
                        {
                            enemy = new Enemy();
                            Manager.Scene.AddEntity(enemy);
                            enemy.Location = e.Location;
                            enemy.Invincible = e.Invincible;
                            enemy.Target = e.Target;
                            enemy.Action = e.Action;

                            // Si el target no esta vacio lo agregamos a la entidad TileGroupQuery:
                            if (enemy.Target != "" && (enemy.Action == 0 || enemy.Action == 1) && tileGroupQuery.Query.Keys.Contains<string>(enemy.Target))
                                tileGroupQuery.Query.Add(enemy.Target, enemy.Action == 0 ? false : true);

                            enemy.PathLength = e.PathLength;
                            enemy.Step = e.Step;
                            enemy.Behavior = e.Behavior;
                            enemy.Type = e.Type;
                            enemy.Respawn = e.Respawn;
                            enemy.RespawnDelay = e.RespawnDelay;
                            enemy.ReversePathAtStart = e.ReversePathAtStart;

                            enemy.Update(Manager.GameTime);

                            // Solo se cuentan las particulas que son sean items o especiales (0 - 2):
                            if (e.Action < 3) Manager.Vars["enemiesLeft"] = (int)Manager.Vars["enemiesLeft"] + 1;
                        }

                        // Agregamos al jugador:
                        Player player = new Player();
                        Manager.Scene.AddEntity(player, "player");
                        player.Location = level.Player.Location;
                        player.Update(Manager.GameTime);

                        // Agregamos la zona de salida del nivel:
                        ExitArea exit = new ExitArea();
                        Manager.Scene.AddEntity(exit);
                        exit.Location = level.ExitArea.Location;
                        exit.InvertClue = level.ExitArea.InvertClue;
                    }

                    // Reiniciamos los contadores:
                    Manager.Vars["shoots"] = 0;
                    Manager.Vars["currentLevelScore"] = 0;
                }
                catch (Exception ex)
                {
                    Manager.Vars["errorBoard"] = "*** Error al cargar el nivel #" + (int)Manager.Vars["currentLevel"] + ": \n" + ex.Message + "***";
                    Manager.GameStates.ChangeState("menu");
                    return;
                }
                Manager.Vars["errorBoard"] = "";

                // Agregamos el componente del HUD:
                Manager.Scene.AddEntity(new Entities.GUI.HUD(), "HUD");

                // Agregamos el componente de la ventana que hace marco de la escena:
                Manager.Scene.AddEntity(new Entities.GUI.MainWindowFrame());

                // Cargamos el fondo de escena:
                Manager.Scene.AddEntity(new Background());

                // Cargamos el efecto de escalineado de la pantalla:
                Manager.Scene.AddEntity(new ScanLines());

                // En caso contrario se manda el mensaje para que inicien su actividad todos los componentes del juego:
                Manager.Messages.SendMessage("", "ready");
                Manager.Vars["showMessageReady"] = false;
                Manager.Vars["showMessagePause"] = false;

                // Si el nivel ya se completo, se muestra en color:
                if ((int)Manager.Vars["currentLevel"] < Session.GameProgress.CurrentLevel)
                    Manager.Graphics.EndShader();
                else
                    Manager.Graphics.BeginShader();

                // Cargamos el efecto de glitch para el efecto color/bn al matar las particulas:
                GlitchColor glitch = new GlitchColor();
                Manager.Scene.AddEntity(glitch, "glitch");
                glitch.Manual = true;

                // Vigilamos que el usuario no cierra sesion. En caso de hacerlo mostramos aviso y lo mandamos a la pantalla de titulo:
                Manager.Scene.AddEntity(new PlayerSignedOut());
            }
        }            
        
        // Descarga el escenario actual:
        private void UnloadLevel()
        {
            Manager.Scene.Clear();  // Eliminamos todos los componentes de la escena.
            Manager.PhysicEngine.Bodies.Clear();    // Eliminamos todos los cuerpos fisicos.
            Manager.Vars["exitLevel"] = false;
        }

        public override void Terminate()
        {
            UnloadLevel();  // Descargamos los elementos del nivel.
            
            // Destruimos la variables globales de escena:
            Manager.Vars.Delete("shoots");
            Manager.Vars.Delete("bullets");
            Manager.Vars.Delete("playerSelectCMYKGun");
            Manager.Vars.Delete("currentLevelScore");
            Manager.Vars.Delete("enemiesLeft");
            Manager.Vars.Delete("exitLevel");

            Manager.Graphics.OffSet = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            // Obtenemos la instancia del componente del jugador:
            Player p = (Player)Manager.Scene.FindByName("player");

            if (!Manager.UIInput.IsConnected || Guide.IsVisible) Manager.Vars["showMessagePause"] = true;

            // Si el jugador esta vivo y se ha activado la pausa se muestra el dialogo correspondiente:
            if (!p.IsDead && (bool)Manager.Vars["showMessagePause"] == true)
            {
                Manager.PhysicEngine.Enabled = false;
                if (Manager.Scene.FindByName("msgBox") == null) Manager.Scene.AddEntity(new Entities.GUI.PauseMenu(), "msgBox");
            }
            else
                Manager.PhysicEngine.Enabled = true;

            if ((bool)Manager.Vars["exitLevel"])
            {
                ResetLevel reset = new ResetLevel();
                Manager.Scene.AddEntity(reset);
                reset.Inmediate = true;

                Manager.Vars["exitLevel"] = false;
            }
        }

        // Codigo de colision del trigger del area de muerte:
        private void OnCollisionDeathZone(Body b, Vector2 force, float direction)
        {
            if ((string)b.Tag == "player") 
                Manager.Messages.SendMessage("player", "dead", false);
        }
    }
}
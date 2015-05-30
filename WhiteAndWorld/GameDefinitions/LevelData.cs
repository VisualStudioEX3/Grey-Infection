using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WhiteAndWorld.GameDefinitions
{
    /// <summary>
    /// Propiedades del tile.
    /// </summary>
    public class Tile
    {
        public string Texture;
        public Vector2 Location;
        public bool Active;
        public string Tag;
        public bool Passable;

        public Tile()
        {
            Texture = string.Empty;
            Location = Vector2.Zero;
            Active = false;
            Tag = string.Empty;
            Passable = false;
        }
    }

    /// <summary>
    /// Propiedades de la pista.
    /// </summary>
    public class Clue
    {
        public string Texture;
        public Vector2 Location;        

        public Clue()
        {
            Texture = string.Empty;
            Location = Vector2.Zero;
        }
    }

    /// <summary>
    /// Propiedades del enemigo.
    /// </summary>
    public class Enemy
    {
        public int Type;
        public int Behavior;
        public bool Invincible;
        public Vector2 Location;
        public int PathLength;
        public int Step;
        public string Target;
        public int Action;
        public bool Respawn;
        public int RespawnDelay;
        public bool ReversePathAtStart;

        public Enemy()
        {
            Type = 0;
            Behavior = 0;
            Invincible = false;
            Location = Vector2.Zero;
            PathLength = 0;
            Step = 0;
            Target = string.Empty;
            Action = 0;
            Respawn = false;
            RespawnDelay = 0;
            ReversePathAtStart = false;
        }
    }

    /// <summary>
    /// Propiedades del jugador.
    /// </summary>
    public class Player
    {
        public Vector2 Location;

        public Player()
        {
            Location = Vector2.Zero;
        }
    }

    /// <summary>
    /// Propiedades del fondo de pantalla.
    /// </summary>
    public class Background
    {
        public string Texture;

        public Background()
        {
            Texture = string.Empty;
        }
    }

    /// <summary>
    /// Define el area de salida del nivel.
    /// </summary>
    public class ExitArea
    {
        public Vector2 Location;
        public bool InvertClue;

        public ExitArea()
        {
            Location = Vector2.Zero;
            InvertClue = false;
        }
    }

    /// <summary>
    /// Descripcion de nivel.
    /// </summary>
    public class LevelData
    {
        public List<Tile> Tiles;
        public List<Clue> Clues;
        public List<Enemy> Enemies;
        public Player Player;
        public Background Background;
        public ExitArea ExitArea;

        public LevelData()
        {
            Tiles = new List<Tile>();
            Clues = new List<Clue>();
            Enemies = new List<Enemy>();
            Player = new Player();
            Background = new Background();
            ExitArea = new ExitArea();
        }
    }
}
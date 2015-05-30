using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;

namespace WhiteAndWorld.GameObjects
{
    public static class MusicPlayer
    {
        public const string MenuTheme = @"Audio\Music\greytheme";
        public const string LevelTheme = @"Audio\Music\greylevel";

        private static string currentAsset = "";

        public static void Play(string Asset)
        {
            if (Asset != currentAsset)
            {
                MediaPlayer.Play(Manager.Content.Load<Song>(Asset));
                MediaPlayer.IsRepeating = true;
                currentAsset = Asset;
            }
        }

        public static float Volume 
        { 
            get { return MediaPlayer.Volume; } 
            set { MediaPlayer.Volume = value; } 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TLSA.Engine;
using WhiteAndWorld.GameDefinitions;

namespace WhiteAndWorld.GameObjects
{
    public static class Session
    {
        public const string SaveGameFilename = "savegame.sav";
        public const string SettingsFilename = "settings.cfg";

        /// <summary>
        /// Progresos del usuario.
        /// </summary>
        public static SaveGame GameProgress;

        /// <summary>
        /// Configuracion del juego.
        /// </summary>
        public static GameSettings Settings;

        /// <summary>
        /// Devuelve la cultura del sistema.
        /// </summary>
        /// <remarks>Soportado solo 'ES' y 'EN'. Si se devuelve una cultura distnta a estas se devuelve por defecto 'EN'.</remarks>
        public static string Culture 
        { 
            get 
            {
                //return "ES";
                string iso = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
                return (iso == "ES" ? "ES" : "EN");
            } 
        }

        /// <summary>
        /// Contiene todos los textos del juego localizados segun region.
        /// </summary>
        /// <remarks>El juego cargara el XML correspondiente a la cultura del sistema.</remarks>
        public static Dictionary<string, string> Strings;

        /// <summary>
        /// Aplica los valores de configuracion y progresos.
        /// </summary>
        public static void Apply()
        {
            Manager.Vars["lastLevel"] = Session.GameProgress.CurrentLevel;
            Manager.Vars["prev_CMYK_Ammo"] = Session.GameProgress.CurrentCMYKAmmo;
            MusicPlayer.Volume = Session.Settings.MusicVolume;
        }
    }
}
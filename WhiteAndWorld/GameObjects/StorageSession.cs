using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using TLSA.Engine;
using TLSA.Engine.Tools;
using System.IO;
using System.Xml.Serialization;

namespace WhiteAndWorld.GameObjects
{
    /// <summary>
    /// Funciones para entrada y salida de archivos del juego.
    /// </summary>
    /// <remarks>Permite guardar y cargar archivos desde el dispositivo seleccionado.</remarks>
    public static class StorageSession
    {
        private static IAsyncResult result;
        private static StorageDevice device;

        public static bool IsDeviceInitialize { get; internal set; }

        public static void Initialize()
        {
            IsDeviceInitialize = false;
            StorageDevice.DeviceChanged += new EventHandler<EventArgs>(DeviceChanged);
        }

        /// <summary>
        /// Evento para detectar si se ha  desconectado el dispositivo de almacenamiento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DeviceChanged(object sender, EventArgs e)
        {
            if (device != null)
            {
                if (!device.IsConnected)
                {
                    IsDeviceInitialize = false;

                    // Activamos el componente de error de dispositivo desconectado:
                    Manager.Scene.AddEntity(new WhiteAndWorld.GameObjects.Entities.StorageDeviceLost());
                }
            }
        }
        
        /// <summary>
        /// Muestra el selector de dispositivos.
        /// </summary>
        /// <remarks>Indice de jugador que realiza la peticion.</remarks>
        public static void SelectDevice(PlayerIndex player)
        {            
            if (!Guide.IsVisible)
            {
                IsDeviceInitialize = false;

                try
                {
                    result = StorageDevice.BeginShowSelector(player, GetDevice, null);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public static void SelectDevice()
        {
            SelectDevice(Manager.UIInput.Player);
        }

        /// <summary>
        /// En conjunto con SelectDevice, se encarga de enlazar con el dispositivo elegido.
        /// </summary>
        /// <param name="result"></param>
        private static void GetDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);

            // Si se selecciono dispositivo lo asociamos a la sesion:
            if (device != null && device.IsConnected)
            {
                IsDeviceInitialize = true;
            }
            else // Si no se selecciono volvemos a la ventana de splash:
            {
                if (Manager.GameStates.CurrerntState != "gametitle") Manager.GameStates.ChangeState("gametitle");
            }
        }

        /// <summary>
        /// Guarda la estructura en un archivo XML en el dispositivo.
        /// </summary>
        /// <param name="filename">Nombre del archivo a guardar.</param>
        /// <param name="data">Datos a guardar.</param>
        public static void SaveData(string filename, object data)
        {
            try
            {
                // Open a storage container.
                IAsyncResult result =
                    device.BeginOpenContainer("GreyInfection", null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                // Close the wait handle.
                result.AsyncWaitHandle.Close();

                // Check to see whether the save exists.
                if (container.FileExists(filename))
                    // Delete it so that we can create one fresh.
                    container.DeleteFile(filename);

                // Create the file.
                Stream stream = container.CreateFile(filename);

                // Convert the object to XML data and put it in the stream.
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(stream, data);

                // Close the file.
                stream.Close();

                // Dispose the container, to commit changes.
                container.Dispose();
            }
            catch (Exception)
            {   
            }
        }

        /// <summary>
        /// Lee el contenido de un archivo en el dispositivo.
        /// </summary>
        public static T LoadData<T>(string filename)
        {
            try
            {
                // Open a storage container.
                IAsyncResult result =
                    device.BeginOpenContainer("GreyInfection", null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                // Close the wait handle.
                result.AsyncWaitHandle.Close();

                // Check to see whether the save exists.
                if (!container.FileExists(filename))
                {
                    // If not, dispose of the container and return.
                    container.Dispose();
                    return default(T);
                }

                // Open the file.
                Stream stream = container.OpenFile(filename, FileMode.Open);

                // Read the data from the file.
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T data = (T)serializer.Deserialize(stream);

                // Close the file.
                stream.Close();

                // Dispose the container.
                container.Dispose();

                // Devolvemos el contenido del archivo:
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static bool FileExists(string filename)
        {
            try
            {
                // Open a storage container.
                IAsyncResult result =
                    device.BeginOpenContainer("GreyInfection", null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                // Close the wait handle.
                result.AsyncWaitHandle.Close();

                // Check to see whether the save exists.
                bool ret = container.FileExists(filename);

                container.Dispose();

                return ret;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Release()
        {
            device = null;
            IsDeviceInitialize = false;
        }
    }
}
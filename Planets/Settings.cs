using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;

namespace Planets
{
    public partial class Planets : DCL.Phone.Xna.PivotGame
    {
        //Loading settings
        protected override void OnActivated(object sender, EventArgs args)
        {
            if(!SettingsLoaded) LoadSettings();

            base.OnActivated(sender, args);
        }

        //Saving settings
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            SaveSettings();

            base.OnDeactivated(sender, args);
        }

        void LoadSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.TryGetValue<float>("Latitude", out Latitude))
                Latitude = 181;
            if (!settings.TryGetValue<float>("Longitude", out Longitude))
                Longitude = 181;
            if (!settings.TryGetValue<bool>("UseGPS", out UseGPS))
                Guide.BeginShowMessageBox("Privacy statement", "This application makes use of the built-in location services.\n\nYour location will be used ONLY to indicate your position on the globe.\n\nEnable the access to and use of location from the location services?", new string[] { "Enable", "Disable" }, 0, MessageBoxIcon.Alert, new AsyncCallback(OnGPSSettingsClosed), null);
            
            //Start connecting GPS
            if (UseGPS) InitWatcher();
            else gpsState = 0;

            SettingsLoaded = true;
        }

        void SaveSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings["Latitude"] = Latitude;
            settings["Longitude"] = Longitude;
            settings["UseGPS"] = UseGPS;
            settings.Save();
        }
    }
}
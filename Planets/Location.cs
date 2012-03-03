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
using Microsoft.Phone.Tasks;
using System.Device.Location;
using DCL.Maths;
using DCL.Phone.Xna;

namespace Planets
{
    public partial class Planets : DCL.Phone.Xna.PivotGame
    {
        #region Coordinates
        float Latitude=181, Longitude=181;
        #endregion


        void InitWatcher()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.MovementThreshold = 20;

            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            watcher.Start();
        }

        void UpdatePositionVisualisation()
        {
            if (SelectedIndex != 2) return; //Не надо обновлять текстуру, если мы успели переключиться!

            tLocation1 = "Lat: " + (Angle)Latitude;
            tLocation2 = "Lon: " + (Angle)Longitude;
            RenderTarget2D rt = new RenderTarget2D(GraphicsDevice, tEarth.Width, tEarth.Height);
            GraphicsDevice.SetRenderTarget(rt);
            SpriteBatch.Begin();
            SpriteBatch.Draw(tEarth, Vector2.Zero, Color.White);
                SpriteBatch.DrawString(ContentFont, ".", new Vector2((Longitude < 0 ? Longitude + 360 : Longitude) / 360 * tEarth.Width,
                                                                    tEarth.Height/2 - Latitude / 180 * tEarth.Height-20), Color.Red);
            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            if(!IsTrial) Planet.Texture = (Texture2D) rt;
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                case GeoPositionStatus.NoData:
                    gpsState = 0;
                    watcher.Stop();
                    if (Longitude <= 180)
                        UpdatePositionVisualisation();
                    break;

                case GeoPositionStatus.Initializing:
                    gpsState = 1;
                    break;

                case GeoPositionStatus.Ready:
                    gpsState = 2;
                    if (watcher.Position.Location.Longitude <= 180)
                    {
                        Latitude = (float)watcher.Position.Location.Latitude;
                        Longitude = (float)watcher.Position.Location.Longitude;
                        watcher.Stop();
                        UpdatePositionVisualisation();
                    }
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (watcher.Position.Location.Longitude <= 180)
            {
                Latitude = (float)watcher.Position.Location.Latitude;
                Longitude = (float)watcher.Position.Location.Longitude;
                watcher.Stop();
                UpdatePositionVisualisation();
            }
        }

        private void OnGPSSettingsClosed(IAsyncResult ar)
        {
            switch (Guide.EndShowMessageBox(ar))
            {
                case 0:
                    UseGPS = true;
                    InitWatcher();
                    break;

                case 1:
                    UseGPS = false;
                    gpsState = 0;
                    tLocation1 = tLocation2 = "";
                    Planet.Texture = tEarth;
                    break;
            }
        }
        private void OnBuyMessageClosed(IAsyncResult ar)
        {
            if (Guide.EndShowMessageBox(ar) == 0)
                //(new MarketplaceDetailTask() { ContentIdentifier = "6efbe3f4-8476-e011-81d2-78e7d1fa76f8" }).Show();
                Guide.ShowMarketplace(PlayerIndex.One);
        }
    }
}
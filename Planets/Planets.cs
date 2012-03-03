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
using My = DCL.Maths;
using DCL.Phone.Xna;

namespace Planets
{
    /// <summary>
    /// This is the main type of the application.
    /// Derives from DCL.Phone.Xna.PivotGame, which is
    /// an extending "wrapper" over the default Microsoft.Xna.Framework.Game
    /// with pivot functionality (UI based on switching between tabs).
    /// </summary>
    public partial class Planets : DCL.Phone.Xna.PivotGame
    {
        #region Fields

        #region Shapes
        Ellipsoid Planet, Moon, Moon2, Moon3, Moon4, Moon5, Moon6, Moon7;
        FlatRingSector[] SaturnRing = new FlatRingSector[100];
        #endregion

        #region Rotation&Quaternions
        My.Quaternion qAxis = My.Quaternion.j, qNew;
        float RotationAngle = 0;
        double touchesDistance;
        Random randomAngle = new Random();
        Vector3 MoonAxis;
        #endregion

        #region Textures
        Texture2D tMercury, tVenus, tEarth, tMoon, tMars, tJupiter, tSaturn, tUranus, tNeptune,
            tPhobos, tDeimos,
            tEuropa, tIo, tCallisto, tGanymede,
            tSaturnRing, tTitan, tDione, tEnceladus, tIapetus, tTethys, tMimas, tRhea,
            tTitania, tOberon, tAriel, tUmbriel, tMiranda,
            tTriton, tNereid, tProteus;
        #endregion

        #region Location
        GeoCoordinateWatcher watcher;
        string tLocation1 = "", tLocation2 = "";
        int gpsState = 1;
        bool UseGPS;
        bool SettingsLoaded = false;
        #endregion

        #region Text
        SpriteFont tinyFont, smallFont;
        #endregion

        bool IsTrial;

        #endregion

        #region Constants
        const int PRECISEMENT = 18;

        //The angle between the equator plane and the perpendicular to the orbit is 23° 30'
        //Obliquities that are less than 5 degrees are ignored
        const float EARTH_OBLIQUITY = 0.409f;
        const float MOON_EARTH_OBLIQUITY = 0.09f;

        //All other speeds depend on this value
        const float EARTH_ROTATION_SPEED = 0.003f;
        //The moon rotates arund the Earth in 27.3 days
        const float MOON_ROTATION_SPEED = EARTH_ROTATION_SPEED/27.3f;

        //Actually mercury's solat day lasts 176 earth's days
        const float MERCURY_ROTATION_SPEED = EARTH_ROTATION_SPEED / 10;

        //Venera rotates in the opposite direction; Actually it's solar day lasts 117 earth's days
        const float VENUS_ROTATION_SPEED = -EARTH_ROTATION_SPEED / 7;

        //Mars' day is nearly the same as on the earth
        const float MARS_ROTATION_SPEED = EARTH_ROTATION_SPEED;
        const float PHOBOS_ROTATION_SPEED = MARS_ROTATION_SPEED * 24 / 7.6f;
        const float DEIMOS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.26f;
        const float MARS_OBLIQUITY = 0.44f;

        const float JUPITER_ROTATION_SPEED = EARTH_ROTATION_SPEED / 2.4f;
        const float EUROPA_ROTATION_SPEED = EARTH_ROTATION_SPEED / 3.55f;
        const float IO_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.77f;
        const float GANYMEDE_ROTATION_SPEED = EARTH_ROTATION_SPEED / 7.15f;
        const float CALLISTO_ROTATION_SPEED = EARTH_ROTATION_SPEED / 16.69f;

        const float SATURN_ROTATION_SPEED = EARTH_ROTATION_SPEED / 2.2f;
        const float TITAN_ROTATION_SPEED = EARTH_ROTATION_SPEED / 15.94f;
        const float TETHYS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.89f;
        const float RHEA_ROTATION_SPEED = EARTH_ROTATION_SPEED / 4.5f;
        const float MIMAS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 0.94f;
        const float IAPETUS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 79.32f;
        const float IAPETUS_SATURN_OBLIQUITY = 0.27f;
        const float ENCELADUS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.37f;
        const float DIONE_ROTATION_SPEED = EARTH_ROTATION_SPEED / 2.77f;
        const float SATURN_OBLIQUITY = 0.49f;

        const float URANUS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.4f;
        const float TITANIA_ROTATION_SPEED = EARTH_ROTATION_SPEED / 8.71f;
        const float OBERON_ROTATION_SPEED = EARTH_ROTATION_SPEED / 13.76f;
        const float ARIEL_ROTATION_SPEED = EARTH_ROTATION_SPEED / 2.52f;
        const float UMBRIEL_ROTATION_SPEED = EARTH_ROTATION_SPEED / 4.14f;
        const float MIRANDA_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.41f;
        const float URANUS_OBLIQUITY = 1.7f;

        const float NEPTUNE_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.5f;
        const float TRITON_ROTATION_SPEED = EARTH_ROTATION_SPEED / 5.88f;
        const float TRITON_NEPTUNE_OBLIQUITY = 2.74f; //Triton rotates in the opposite direction
        const float NEREID_ROTATION_SPEED = EARTH_ROTATION_SPEED / 360.13f;
        const float PROTEUS_ROTATION_SPEED = EARTH_ROTATION_SPEED / 1.12f;
        const float NEPTUNE_OBLIQUITY = 0.52f;
        #endregion

        //Constructor
        public Planets()
        {
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            //Components.Add(new FrameRateCounter(this));
            //Guide.SimulateTrialMode = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            IsTrial = Guide.IsTrialMode;

            #region Pivot pages initialization
            this[0].Header = "Mercury";
            this[0].DrawScene += Mercury_DrawScene;
            this[0].DrawFrame += Mercury_DrawFrame;

            AddItem(new PivotGameItem());
            this[1].Header = "Venus";
            this[1].DrawScene += Venus_DrawScene;
            this[1].DrawFrame += Venus_DrawFrame;

            AddItem(new PivotGameItem());
            this[2].Header = "Earth";
            this[2].DrawScene += Earth_DrawScene;
            this[2].DrawFrame += Earth_DrawFrame;

            AddItem(new PivotGameItem());
            this[3].Header = "Mars";
            this[3].DrawScene += Mars_DrawScene;
            this[3].DrawFrame += Mars_DrawFrame;

            if (!IsTrial)
            {
                AddItem(new PivotGameItem());
                this[4].Header = "Jupiter";
                this[4].DrawScene += Jupiter_DrawScene;
                this[4].DrawFrame += Jupiter_DrawFrame;

                AddItem(new PivotGameItem());
                this[5].Header = "Saturn";
                this[5].DrawScene += Saturn_DrawScene;
                this[5].DrawFrame += Saturn_DrawFrame;

                AddItem(new PivotGameItem());
                this[6].Header = "Uranus";
                this[6].DrawScene += Uranus_DrawScene;
                this[6].DrawFrame += Uranus_DrawFrame;

                AddItem(new PivotGameItem());
                this[7].Header = "Neptune";
                this[7].DrawScene += Neptune_DrawScene;
                this[7].DrawFrame += Neptune_DrawFrame;
            }
            #endregion

            #region Pivot initialization
            SelectedIndex = 2;
            SelectionChanged += Planets_SelectionChanged;

            BackgroundColor = new Color(0,0,20);
            ForegroundColor = Color.SkyBlue;
            SceneBackgroundColor = new Color(0, 0, 10);
            SceneBackgroundTexture = Content.Load<Texture2D>("starfield");

            DrawingArea = new Rectangle(-480, 120, 1440, 480);

            SceneCenterTranslation = new Vector2(0, -0.5f);
            CameraScale = 8f; //8f
            #endregion

        }

        //Is called when user moves to another tabs; Here the shapes are being updated
        void Planets_SelectionChanged(object sender, EventArgs e)
        {
            switch (SelectedIndex)
            {
                case 0: //Mercury
                    Planet = new Sphere(Vector3.Zero, 1, PRECISEMENT, tMercury, GraphicsDevice);
                    Reset(); break;
                case 1: //Venus
                    Planet = new Sphere(Vector3.Zero, 1.2f, PRECISEMENT, tVenus, GraphicsDevice);
                    Reset(); break;
                case 2: //Earth
                    Earth_Init();
                    Reset(); break;
                case 3: //Mars
                    Planet = new Sphere(Vector3.Zero, 1.3f, PRECISEMENT, tMars, GraphicsDevice);
                    Planet.Rotate(-Vector3.UnitZ, MARS_OBLIQUITY); 
                    Moon = new Ellipsoid(new Vector3(0, 0, 2.5f), 0.1f, 1.2f, 0.83f, 1, PRECISEMENT/3+1, tPhobos, GraphicsDevice); //Phobos
                    Moon.RotateComposition(-Vector3.UnitZ, MARS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble()*MathHelper.Pi); 
                    Moon2 = new Ellipsoid(new Vector3(0, 0, 4), 0.07f, 1.22f, 1, 0.85f, PRECISEMENT/3+1, tDeimos, GraphicsDevice); //Deimos
                    Moon2.RotateComposition(-Vector3.UnitZ, MARS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi); 
                    Reset(); break;
                case 4: //Jupiter
                    Planet = new Ellipsoid(Vector3.Zero, 2f, 1.06f, 1, 1.06f, PRECISEMENT, tJupiter, GraphicsDevice);
                    Moon = new Sphere(new Vector3(0, 0, 3.5f), 0.1f, PRECISEMENT/3+1, tEuropa, GraphicsDevice); //Europa
                    Moon.Rotate(Planet.Axis, -(float)randomAngle.NextDouble()*MathHelper.Pi); 
                    Moon2 = new Sphere(new Vector3(0, 0, 3f), 0.11f, PRECISEMENT/3+1, tIo, GraphicsDevice); //Io
                    Moon2.Rotate(Planet.Axis, -(float)randomAngle.NextDouble()*MathHelper.Pi); 
                    Moon3 = new Sphere(new Vector3(0, 0, 4.3f), 0.17f, PRECISEMENT/3+2, tGanymede, GraphicsDevice); //Ganymede
                    Moon3.Rotate(Planet.Axis, -(float)randomAngle.NextDouble()*MathHelper.Pi); 
                    Moon4 = new Sphere(new Vector3(0, 0, 5.5f), 0.14f, PRECISEMENT/3+2, tCallisto, GraphicsDevice); //Callisto
                    Moon4.Rotate(Planet.Axis, -(float)randomAngle.NextDouble()*MathHelper.Pi); 
                    Reset(); break;
                case 5: //Saturn
                    Planet = new Ellipsoid(Vector3.Zero, 1.8f, 1.1f, 1, 1.1f, PRECISEMENT, tSaturn, GraphicsDevice);
                    Planet.Rotate(-Vector3.UnitZ, SATURN_OBLIQUITY);
                    for (int i = 0; i < SaturnRing.Length; i++)
                    {
                        SaturnRing[i] = new FlatRingSector(Planet.Center, 2.7f, 0.9f, 1, MathHelper.TwoPi / SaturnRing.Length, tSaturnRing, GraphicsDevice);
                        SaturnRing[i].RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, MathHelper.TwoPi / SaturnRing.Length * i);
                    }
                    Moon = new Sphere(new Vector3(0, 0, 4f), 0.15f, PRECISEMENT/3+3, tTitan, GraphicsDevice); //Titan
                    Moon.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon2 = new Sphere(new Vector3(0, 0, 3f), 0.03f, PRECISEMENT/6, tRhea, GraphicsDevice); //Rhea
                    Moon2.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon3 = new Sphere(new Vector3(0, 0, 2.6f), 0.027f, PRECISEMENT/6, tDione, GraphicsDevice); //Dione
                    Moon3.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon4 = new Sphere(new Vector3(0, 0, 2.45f), 0.023f, PRECISEMENT/6, tTethys, GraphicsDevice); //Tethys
                    Moon4.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon5 = new Sphere(new Vector3(0, 0, 7f), 0.06f, PRECISEMENT/3+3, tIapetus, GraphicsDevice); //Iapetus
                    MoonAxis = (Vector3)My.Quaternion.Rotate(Planet.Axis, Vector3.UnitZ, SATURN_OBLIQUITY - IAPETUS_SATURN_OBLIQUITY);
                    Moon5.RotateComposition(-Vector3.UnitZ, IAPETUS_SATURN_OBLIQUITY, MoonAxis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon6 = new Sphere(new Vector3(0, 0, 2.3f), 0.015f, PRECISEMENT/6, tEnceladus, GraphicsDevice); //Enceladus
                    Moon6.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon7 = new Sphere(new Vector3(0, 0, 2.1f), 0.01f, PRECISEMENT/6, tMimas, GraphicsDevice); //Mimas
                    Moon7.RotateComposition(-Vector3.UnitZ, SATURN_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);

                    //To see rings at once:
                    Planet.Rotate(Vector3.UnitX, 0.1f);Moon.Rotate(Vector3.UnitX, 0.1f);Moon2.Rotate(Vector3.UnitX, 0.1f);
                    Moon3.Rotate(Vector3.UnitX, 0.1f);Moon4.Rotate(Vector3.UnitX, 0.1f);Moon5.Rotate(Vector3.UnitX, 0.1f);
                    Moon6.Rotate(Vector3.UnitX, 0.1f);Moon7.Rotate(Vector3.UnitX, 0.1f);
                    foreach (FlatRingSector f in SaturnRing) f.Rotate(Vector3.UnitX, 0.1f);
                    Reset(); break;
                case 6: //Uranus
                    Planet = new Sphere(Vector3.Zero, 1.7f, PRECISEMENT, tUranus, GraphicsDevice);
                    Planet.Rotate(-Vector3.UnitZ, URANUS_OBLIQUITY); 

                    Moon = new Sphere(new Vector3(0, 0, 3.5f), 0.11f, PRECISEMENT/3+1, tTitania, GraphicsDevice); //Titania
                    Moon.RotateComposition(-Vector3.UnitZ, URANUS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon2 = new Sphere(new Vector3(0, 0, 3.8f), 0.11f, PRECISEMENT/3+1, tOberon, GraphicsDevice); //Oberon
                    Moon2.RotateComposition(-Vector3.UnitZ, URANUS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon3 = new Sphere(new Vector3(0, 0, 2.6f), 0.09f, PRECISEMENT/3+1, tAriel, GraphicsDevice); //Ariel
                    Moon3.RotateComposition(-Vector3.UnitZ, URANUS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon4 = new Sphere(new Vector3(0, 0, 3f), 0.09f, PRECISEMENT/3+1, tUmbriel, GraphicsDevice); //Umbriel
                    Moon4.RotateComposition(-Vector3.UnitZ, URANUS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon5 = new Sphere(new Vector3(0, 0, 2.4f), 0.05f, PRECISEMENT/3+1, tMiranda, GraphicsDevice); //Miranda
                    Moon5.RotateComposition(-Vector3.UnitZ, URANUS_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Reset(); break;
                case 7: //Neptune
                    Planet = new Sphere(Vector3.Zero, 1.6f, PRECISEMENT, tNeptune, GraphicsDevice);
                    Planet.Rotate(-Vector3.UnitZ, NEPTUNE_OBLIQUITY);

                    Moon = new Sphere(new Vector3(0, 0, 2.6f), 0.14f, PRECISEMENT/2-1, tTriton, GraphicsDevice); //Triton
                    MoonAxis = (Vector3)My.Quaternion.Rotate(Planet.Axis, Vector3.UnitZ, SATURN_OBLIQUITY - TRITON_NEPTUNE_OBLIQUITY);
                    Moon.RotateComposition(-Vector3.UnitZ, TRITON_NEPTUNE_OBLIQUITY, MoonAxis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon2 = new Ellipsoid(new Vector3(0, 0, 5f), 0.04f, 1, 1.1f, 1, PRECISEMENT/4, tNereid, GraphicsDevice); //Nereid
                    Moon2.RotateComposition(-Vector3.UnitZ, NEPTUNE_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    Moon3 = new Sphere(new Vector3(0, 0, 2f), 0.05f, PRECISEMENT/4, tProteus, GraphicsDevice); //Proteus
                    Moon3.RotateComposition(-Vector3.UnitZ, NEPTUNE_OBLIQUITY, Planet.Axis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
                    
                    Reset(); break;
            }
        }

        //Resetting rotation angles and all that
        void Reset()
        {
            RotationAngle = 0;
            Zoom = 1;
            qAxis = DCL.Maths.Quaternion.j;
            if(SelectedIndex==2 && Latitude<=180 && UseGPS)
                UpdatePositionVisualisation();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SplashScreenImage = Content.Load<Texture2D>("SplashScreenImage");

            base.LoadContent();

            #region Fonts
            //Defining our fonts (base.LoadContent() allows to use the standard ones)
            HeaderFont = Content.Load<SpriteFont>("Pericles44");
            TitleFont = Content.Load<SpriteFont>("Pericles16");
            ContentFont = Content.Load<SpriteFont>("Segoe16");

            tinyFont = Content.Load<SpriteFont>("Segoe8");
            smallFont = Content.Load<SpriteFont>("Segoe12");
            #endregion

            #region Text
            Title = "Planets";
            LoadTextContent();
            #endregion

            #region Textures
            //Loading all the textures at once (no delays during the run time)
            tMercury = Content.Load<Texture2D>("mercury");
            tVenus = Content.Load<Texture2D>("venus");
            tEarth = Content.Load<Texture2D>("earth");
                tMoon = Content.Load<Texture2D>("moon");
            tMars = Content.Load<Texture2D>("mars");
                tPhobos = Content.Load<Texture2D>("phobos");
                tDeimos = Content.Load<Texture2D>("deimos");
            tJupiter = Content.Load<Texture2D>("jupiter");
                tEuropa = Content.Load<Texture2D>("europa");
                tIo = Content.Load<Texture2D>("io");
                tGanymede = Content.Load<Texture2D>("ganymede");
                tCallisto = Content.Load<Texture2D>("callisto");
            tSaturn = Content.Load<Texture2D>("saturn");
                tSaturnRing = Content.Load<Texture2D>("sat_ring_color");
                tTitan = Content.Load<Texture2D>("titan");
                tIapetus = Content.Load<Texture2D>("iapetus");
                tEnceladus = Content.Load<Texture2D>("enceladus");
                tMimas = Content.Load<Texture2D>("mimas");
                tTethys = Content.Load<Texture2D>("tethys");
                tRhea = Content.Load<Texture2D>("rhea");
                tDione = Content.Load<Texture2D>("dione");
            tUranus = Content.Load<Texture2D>("uranus");
                tTitania = Content.Load<Texture2D>("titania");
                tOberon = Content.Load<Texture2D>("oberon");
                tAriel = Content.Load<Texture2D>("ariel");
                tUmbriel = Content.Load<Texture2D>("umbriel");
                tMiranda = Content.Load<Texture2D>("miranda");
             tNeptune = Content.Load<Texture2D>("neptune");
                tTriton = Content.Load<Texture2D>("triton");
                tNereid = Content.Load<Texture2D>("nereid");
                tProteus = Content.Load<Texture2D>("proteus");

            LoadIcons();
            #endregion

            Earth_Init();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /*protected override void UnloadContent()
        {

        }*/

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region Handling touches
            Touches = TouchPanel.GetState();
            if (Touches.Count == 1)
            {
                #region One touch => Rotate
                if (Touches[0].State == TouchLocationState.Moved &&
                       Touches[0].Position.X > this[SelectedIndex].DrawingArea.X &&
                       Touches[0].Position.X < this[SelectedIndex].DrawingArea.X + this[SelectedIndex].DrawingArea.Width &&
                       Touches[0].Position.Y > this[SelectedIndex].DrawingArea.Y &&
                       Touches[0].Position.Y < this[SelectedIndex].DrawingArea.Y + this[SelectedIndex].DrawingArea.Height)
                {
                    TouchLocation prevTouch;
                    if (Touches[0].TryGetPreviousLocation(out prevTouch))
                    {
                        float deltaX = Touches[0].Position.X - prevTouch.Position.X;
                        float deltaY = Touches[0].Position.Y - prevTouch.Position.Y;

                        if (Math.Abs(deltaX) > 0 || Math.Abs(deltaY) > 0)
                        {

                            qNew = (new My.Quaternion(0, deltaY, deltaX, 0)).Normalize().Approximate(0.5f);
                            qAxis = qNew;
                        }
                        RotationAngle = (qAxis.Y * deltaX + qAxis.X * deltaY) / 100;
                    }
                }
                else
                    RotationAngle = 0;
                #endregion

                #region One touch => Click on a Button

                if (IsTrial && Touches[0].State == TouchLocationState.Pressed &&
                       Touches[0].Position.X > 300 && Touches[0].Position.X < 430 &&
                       Touches[0].Position.Y > 0 && Touches[0].Position.Y < 40)
                    //(new MarketplaceDetailTask() { ContentIdentifier = "6efbe3f4-8476-e011-81d2-78e7d1fa76f8" }).Show();
                    Guide.ShowMarketplace(PlayerIndex.One);

                if (Touches[0].State == TouchLocationState.Pressed && SelectedIndex==2 &&
                       Touches[0].Position.X > 435 && Touches[0].Position.X < 480 &&
                       Touches[0].Position.Y > 0 && Touches[0].Position.Y < 40)
                    if (IsTrial)
                        Guide.BeginShowMessageBox("Trial version", "Location services are available only in the full version!", new string[] { "Buy now", "Buy later" }, 0, MessageBoxIcon.Alert, new AsyncCallback(OnBuyMessageClosed), null);
                    else
                        Guide.BeginShowMessageBox("Location settings", "This application can use the built-in location services.\n\nYour location will be used ONLY to indicate your position on the globe.\n\nEnable the access to and use of location from the location services?", new string[] { "Enable", "Disable" }, 0, MessageBoxIcon.Alert, new AsyncCallback(OnGPSSettingsClosed), null);
                #endregion
            }
            else if (Touches.Count > 1 && !ChangingPages)
            {
                #region Multitouch => Resize
                RotationAngle = 0;

                //first touch
                if (Touches[0].State == TouchLocationState.Pressed || Touches[1].State == TouchLocationState.Pressed)
                {
                    touchesDistance = Math.Sqrt((Touches[0].Position.X - Touches[1].Position.X) * (Touches[0].Position.X - Touches[1].Position.X) +
                                        (Touches[0].Position.Y - Touches[1].Position.Y) * (Touches[0].Position.Y - Touches[1].Position.Y));
                }
                //moving fingers
                else if (Touches[0].State == TouchLocationState.Moved || Touches[1].State == TouchLocationState.Moved)
                {
                    double newTouchesDistance = Math.Sqrt((Touches[0].Position.X - Touches[1].Position.X) * (Touches[0].Position.X - Touches[1].Position.X) +
                                        (Touches[0].Position.Y - Touches[1].Position.Y) * (Touches[0].Position.Y - Touches[1].Position.Y));
                    Zoom = (float)(newTouchesDistance / touchesDistance) * Zoom;
                    Zoom = Math.Max(Math.Min(3f/Planet.Radius, Zoom), 0.7f);
                    touchesDistance = newTouchesDistance;
                }
                #endregion
            }
            #endregion

            #region Rotation
            
            switch (SelectedIndex)
            {
                case 0: //Mercury
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, MERCURY_ROTATION_SPEED); 
                    break;
                case 1: //Venus
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, VENUS_ROTATION_SPEED); 
                    break;
                case 2: //Earth
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, EARTH_ROTATION_SPEED);

                    MoonAxis = (Vector3)My.Quaternion.Rotate(MoonAxis, qAxis, RotationAngle);
                    Moon.RotateComposition(qAxis, RotationAngle, MoonAxis, MOON_ROTATION_SPEED); 
                    break;
                case 3: //Mars
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, MARS_ROTATION_SPEED);

                    Moon.RotateComposition(qAxis, RotationAngle, Planet.Axis, PHOBOS_ROTATION_SPEED);
                    Moon2.RotateComposition(qAxis, RotationAngle, Planet.Axis, DEIMOS_ROTATION_SPEED); 
                    break;
                case 4: //Jupiter
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, JUPITER_ROTATION_SPEED); 

                    Moon.RotateComposition(qAxis, RotationAngle, Planet.Axis, EUROPA_ROTATION_SPEED); 
                    Moon2.RotateComposition(qAxis, RotationAngle, Planet.Axis, IO_ROTATION_SPEED); 
                    Moon3.RotateComposition(qAxis, RotationAngle, Planet.Axis, GANYMEDE_ROTATION_SPEED); 
                    Moon4.RotateComposition(qAxis, RotationAngle, Planet.Axis, CALLISTO_ROTATION_SPEED); 
                    break;
                case 5: //Saturn
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, SATURN_ROTATION_SPEED);

                    Moon.RotateComposition(qAxis, RotationAngle, Planet.Axis, TITAN_ROTATION_SPEED);
                    Moon2.RotateComposition(qAxis, RotationAngle, Planet.Axis, RHEA_ROTATION_SPEED);
                    Moon3.RotateComposition(qAxis, RotationAngle, Planet.Axis, DIONE_ROTATION_SPEED);
                    Moon4.RotateComposition(qAxis, RotationAngle, Planet.Axis, TETHYS_ROTATION_SPEED);
                    MoonAxis = (Vector3)My.Quaternion.Rotate(MoonAxis, qAxis, RotationAngle);
                    Moon5.RotateComposition(qAxis, RotationAngle, MoonAxis, IAPETUS_ROTATION_SPEED); //Iapetus has another axis
                    Moon6.RotateComposition(qAxis, RotationAngle, Planet.Axis, ENCELADUS_ROTATION_SPEED);
                    Moon7.RotateComposition(qAxis, RotationAngle, Planet.Axis, MIMAS_ROTATION_SPEED);

                    foreach (FlatRingSector f in SaturnRing)
                        f.Rotate(qAxis, RotationAngle);
                    break;
                case 6: //Uran
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, URANUS_ROTATION_SPEED); 

                    Moon.RotateComposition(qAxis, RotationAngle, Planet.Axis, TITANIA_ROTATION_SPEED);
                    Moon2.RotateComposition(qAxis, RotationAngle, Planet.Axis, OBERON_ROTATION_SPEED);
                    Moon3.RotateComposition(qAxis, RotationAngle, Planet.Axis, ARIEL_ROTATION_SPEED);
                    Moon4.RotateComposition(qAxis, RotationAngle, Planet.Axis, UMBRIEL_ROTATION_SPEED);
                    Moon5.RotateComposition(qAxis, RotationAngle, Planet.Axis, MIRANDA_ROTATION_SPEED);
                    break;
                case 7: //Neptun
                    Planet.RotateComposition(qAxis, RotationAngle, Planet.Axis, EARTH_ROTATION_SPEED); 

                    MoonAxis = (Vector3)My.Quaternion.Rotate(MoonAxis, qAxis, RotationAngle);
                    Moon.RotateComposition(qAxis, RotationAngle, MoonAxis, TRITON_ROTATION_SPEED);
                    Moon2.RotateComposition(qAxis, RotationAngle, Planet.Axis, NEREID_ROTATION_SPEED);
                    Moon3.RotateComposition(qAxis, RotationAngle, Planet.Axis, PROTEUS_ROTATION_SPEED);
                    break;
            }
            #endregion

            #region Updating texts
            //New Axis
            //tAxis = String.Format("Axis: ({0:G4}; {1:G4}; {2:G4})", qAxis.X, qAxis.Y, qAxis.Z);

            //ANGLES OF ROTATION
            //angAroundAxis += MathHelper.ToDegrees(0.025f); if (angAroundAxis > 360) angAroundAxis -= 360;
            //angAroundEarth += MathHelper.ToDegrees(0.005f); if (angAroundEarth > 360) angAroundEarth -= 360;

            //tAngleA = String.Format("Around axis:  {0}°", (int)angAroundAxis);
            //tAngleB = String.Format("Around earth: {0}°", (int)angAroundEarth);
            #endregion

            base.Update(gameTime);
        }

        #region Drawing code for each tab
        #region Mercury
        private void Mercury_DrawScene(object sender, EventArgs e)
        {
            Planet.Draw(BasicEffect, DrawMode.Textured);
        }
        private void Mercury_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, mercuryDiameter, plDiameter, Color.White);
            DrawString(ContentFont, mercuryMass, plMass, Color.White);
            DrawString(ContentFont, mercuryDistance, plDistance, Color.White);
            DrawString(ContentFont, mercuryRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, mercuryRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, mercuryTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, mercuryGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, mercuryMoons, plMoons, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Venus
        private void Venus_DrawScene(object sender, EventArgs e)
        {
            Planet.Draw(BasicEffect, DrawMode.Textured);
        }
        private void Venus_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, venusDiameter, plDiameter, Color.White);
            DrawString(ContentFont, venusMass, plMass, Color.White);
            DrawString(ContentFont, venusDistance, plDistance, Color.White);
            DrawString(ContentFont, venusRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, venusRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, venusTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, venusGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, venusMoons, plMoons, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Earth
        private void Earth_DrawScene(object sender, EventArgs e)
        {
            //There are only earth, moon and dot and we need them to be fast,
            //so we don't use Shapes.DrawScene.
            if (Moon.Center.Z > 0)
            {
                Planet.Draw(BasicEffect, DrawMode.Textured);
                Moon.Draw(BasicEffect, DrawMode.Textured);
            }
            else
            {
                Moon.Draw(BasicEffect, DrawMode.Textured);
                Planet.Draw(BasicEffect, DrawMode.Textured);
            }
        }
        private void Earth_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();

            if (!IsTrial)
            {
                SpriteBatch.DrawString(smallFont, tLocation1, coordPos1, Color.SkyBlue);
                SpriteBatch.DrawString(smallFont, tLocation2, coordPos2, Color.SkyBlue);
            }

            switch(gpsState)
            {
                default: SpriteBatch.Draw(icoGPSoff, coordGps, Color.White); break;
                case 1: SpriteBatch.Draw(icoGPSsearch, coordGps, Color.White); break;
                case 2: SpriteBatch.Draw(icoGPSon, coordGps, Color.White); break;
            }

            DrawString(ContentFont, earthDiameter, plDiameter, Color.White);
            DrawString(ContentFont, earthMass, plMass, Color.White);
            DrawString(ContentFont, earthDistance, plDistance, Color.White);
            DrawString(ContentFont, earthRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, earthRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, earthTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, earthGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, earthMoons, plMoons, Color.White);

            DrawSprite(icoMoon, posMoon, Color.White);
            DrawString(ContentFont, strMoon, coordMoon, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Mars
        private void Mars_DrawScene(object sender, EventArgs e)
        {
            Shape.DrawScene(BasicEffect, DrawMode.Textured, Planet, Moon, Moon2);
        }
        private void Mars_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, marsDiameter, plDiameter, Color.White);
            DrawString(ContentFont, marsMass, plMass, Color.White);
            DrawString(ContentFont, marsDistance, plDistance, Color.White);
            DrawString(ContentFont, marsRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, marsRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, marsTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, marsGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, marsMoons, plMoons, Color.White);

            DrawSprite(icoPhobos, posPhobos, Color.White);
            DrawString(ContentFont, strPhobos, coordPhobos, Color.White);
            DrawSprite(icoDeimos, posDeimos, Color.White);
            DrawString(ContentFont, strDeimos, coordDeimos, Color.White);

            SpriteBatch.End();
        }
        #endregion

        #region Jupiter
        private void Jupiter_DrawScene(object sender, EventArgs e)
        {
            //Shape.DrawScene is preferable when we have many moons
            Shape.DrawScene(BasicEffect, DrawMode.Textured, Planet, Moon, Moon2, Moon3, Moon4);
        }
        private void Jupiter_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, jupiterDiameter, plDiameter, Color.White);
            DrawString(ContentFont, jupiterMass, plMass, Color.White);
            DrawString(ContentFont, jupiterDistance, plDistance, Color.White);
            DrawString(ContentFont, jupiterRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, jupiterRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, jupiterTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, jupiterGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, jupiterMoons, plMoons, Color.White);

            DrawSprite(icoIo, posIo, Color.White);
            DrawString(ContentFont, strIo, coordIo, Color.White);
            DrawSprite(icoEuropa, posEuropa, Color.White);
            DrawString(ContentFont, strEuropa, coordEuropa, Color.White);
            DrawSprite(icoGanymede, posGanymede, Color.White);
            DrawString(ContentFont, strGanymede, coordGanymede, Color.White);
            DrawSprite(icoCallisto, posCallisto, Color.White);
            DrawString(ContentFont, strCallisto, coordCallisto, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Saturn
        private void Saturn_DrawScene(object sender, EventArgs e)
        {
            Shape.DrawScene(BasicEffect, DrawMode.Textured, SaturnRing, Planet, Moon, Moon2, Moon3, Moon4, Moon5, Moon6, Moon7);
        }
        private void Saturn_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, saturnDiameter, plDiameter, Color.White);
            DrawString(ContentFont, saturnMass, plMass, Color.White);
            DrawString(ContentFont, saturnDistance, plDistance, Color.White);
            DrawString(ContentFont, saturnRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, saturnRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, saturnTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, saturnGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, saturnMoons, plMoons, Color.White);

            DrawSprite(icoMimas, posMimas, Color.White);
            DrawString(ContentFont, strMimas, coordMimas, Color.White);
            DrawSprite(icoEnceladus, posEnceladus, Color.White);
            DrawString(ContentFont, strEnceladus, coordEnceladus, Color.White);
            DrawSprite(icoTethys, posTethys, Color.White);
            DrawString(ContentFont, strTethys, coordTethys, Color.White);
            DrawSprite(icoDione, posDione, Color.White);
            DrawString(ContentFont, strDione, coordDione, Color.White);
            DrawSprite(icoRhea, posRhea, Color.White);
            DrawString(ContentFont, strRhea, coordRhea, Color.White);
            DrawSprite(icoTitan, posTitan, Color.White);
            DrawString(ContentFont, strTitan, coordTitan, Color.White);
            DrawSprite(icoIapetus, posIapetus, Color.White);
            DrawString(ContentFont, strIapetus, coordIapetus, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Uranus
        private void Uranus_DrawScene(object sender, EventArgs e)
        {
            Shape.DrawScene(BasicEffect, DrawMode.Textured, Planet, Moon, Moon2, Moon3, Moon4, Moon5);
        }
        private void Uranus_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, uranusDiameter, plDiameter, Color.White);
            DrawString(ContentFont, uranusMass, plMass, Color.White);
            DrawString(ContentFont, uranusDistance, plDistance, Color.White);
            DrawString(ContentFont, uranusRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, uranusRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, uranusTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, uranusGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, uranusMoons, plMoons, Color.White);

            DrawSprite(icoMiranda, posMiranda, Color.White);
            DrawString(ContentFont, strMiranda, coordMiranda, Color.White);
            DrawSprite(icoAriel, posAriel, Color.White);
            DrawString(ContentFont, strAriel, coordAriel, Color.White);
            DrawSprite(icoUmbriel, posUmbriel, Color.White);
            DrawString(ContentFont, strUmbriel, coordUmbriel, Color.White);
            DrawSprite(icoTitania, posTitania, Color.White);
            DrawString(ContentFont, strTitania, coordTitania, Color.White);
            DrawSprite(icoOberon, posOberon, Color.White);
            DrawString(ContentFont, strOberon, coordOberon, Color.White);
            SpriteBatch.End();
        }
        #endregion

        #region Neptune
        private void Neptune_DrawScene(object sender, EventArgs e)
        {
            Shape.DrawScene(BasicEffect, DrawMode.Textured, Planet, Moon, Moon2, Moon3);
        }
        private void Neptune_DrawFrame(object sender, EventArgs e)
        {
            SpriteBatch.Begin();
            DrawString(ContentFont, neptuneDiameter, plDiameter, Color.White);
            DrawString(ContentFont, neptuneMass, plMass, Color.White);
            DrawString(ContentFont, neptuneDistance, plDistance, Color.White);
            DrawString(ContentFont, neptuneRotationPeriod, plRotationPeriod, Color.White);
            DrawString(ContentFont, neptuneRevolutionPeriod, plRevolutionPeriod, Color.White);
            DrawString(ContentFont, neptuneTemperatures, plTemperatures, Color.White);
            DrawString(ContentFont, neptuneGravitationalPull, plGravitationalPull, Color.White);
            DrawString(TitleFont, neptuneMoons, plMoons, Color.White);

            DrawSprite(icoProteus, posProteus, Color.White);
            DrawString(ContentFont, strProteus, coordProteus, Color.White);
            DrawSprite(icoTriton, posTriton, Color.White);
            DrawString(ContentFont, strTriton, coordTriton, Color.White);
            DrawSprite(icoNereid, posNereid, Color.White);
            DrawString(ContentFont, strNereid, coordNereid, Color.White);
            SpriteBatch.End();
        }
        #endregion
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin();

                DrawString(TitleFont, strCharacteristics, coordCharacteristics, Color.SkyBlue);
                DrawString(TitleFont, strMoons, coordMoons, Color.SkyBlue);
                DrawString(ContentFont, strDiameter, coordDiameter, Color.SkyBlue);
                DrawString(ContentFont, strMass, coordMass, Color.SkyBlue);
                DrawString(ContentFont, strDistance, coordDistance, Color.SkyBlue);
                DrawString(ContentFont, strRotationPeriod, coordRotationPeriod, Color.SkyBlue);
                DrawString(ContentFont, strRevolutionPeriod, coordRevolutionPeriod, Color.SkyBlue);
                DrawString(ContentFont, strTemperatures, coordTemperatures, Color.SkyBlue);
                DrawString(ContentFont, strGravitationalPull, coordGravitationalPull, Color.SkyBlue);
                DrawString(tinyFont, strRemark, coordNotation, Color.SkyBlue);

                DrawSprite(icoDiameter, spDiameter, Color.SkyBlue);
                DrawSprite(icoMass, spMass, Color.SkyBlue);
                DrawSprite(icoDistance, spDistance, Color.SkyBlue);
                DrawSprite(icoRotationPeriod, spRotationPeriod, Color.SkyBlue);
                DrawSprite(icoRevolutionPeriod, spRevolutionPeriod, Color.SkyBlue);
                DrawSprite(icoTemperatures, spTemperatures, Color.SkyBlue);
                DrawSprite(icoGravitationalPull, spGravitationalPull, Color.SkyBlue);

                if(IsTrial)
                    SpriteBatch.DrawString(TitleFont, strBuy, coordBuy, Color.White);

            SpriteBatch.End();
        }

        //Is called from two places in code
        void Earth_Init()
        {
            Planet = new Sphere(Vector3.Zero, 1.5f, PRECISEMENT, tEarth, GraphicsDevice);
            Planet.Rotate(-Vector3.UnitZ, EARTH_OBLIQUITY);
            Moon = new Sphere(new Vector3(3.5f, 0, 0), 0.25f, PRECISEMENT / 2+2, tMoon, GraphicsDevice);
            MoonAxis = (Vector3)My.Quaternion.Rotate(Planet.Axis, Vector3.UnitZ, EARTH_OBLIQUITY - MOON_EARTH_OBLIQUITY);
            Moon.RotateComposition(-Vector3.UnitZ, MOON_EARTH_OBLIQUITY, MoonAxis, -(float)randomAngle.NextDouble() * MathHelper.Pi);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DCL.Phone.Xna
{
    /// <summary>
    /// Provides an extending "wrapper" over the default Microsoft.Xna.Framework.Game
    /// with pivot functionality (pivot is a UI based on switching between tabs).
    /// XNA games that require the tab interface should derive from this class.
    /// </summary>
    public class PivotGame: Game
    {
        #region Fields
        int delta = 0; //Indicates the scrolling "distance"
        Vector2 deltaVector = Vector2.Zero; //For drawing texts
        int selInd = 0; //Selected tab index
        float cameraWidth=8, zoom=1; //Projection paramenters
        bool SelectedIndexSetFirstTime = true; //Selection changed event in not fired when loading the pivot
        bool SwitchingTabsNow = false; //We are changing the selection right now
        ContentManager FontLoader; //For providing default fonts from the resource file
        List<int> CallStack = new List<int>(); //Is needed for handling the "Back" button
        ProjectionType proj; //Projection for BasicEffect

        /// <summary>
        /// The standard font for drawing page headers that can be changed.
        /// </summary>
        protected internal SpriteFont HeaderFont;
        /// <summary>
        /// The font that is used to draw text on the page and can be changed.
        /// </summary>
        protected internal SpriteFont ContentFont;
        /// <summary>
        /// The font that is used to draw the application title and can be changed
        /// (the recommended font size is 16-20)
        /// </summary>
        protected internal SpriteFont TitleFont;
        #endregion

        #region Events
        /// <summary>
        /// The devegate for the SelectionChanged event handler.
        /// </summary>
        /// <param name="sender">object that fires the event.</param>
        /// <param name="e">Event arguments that contain the previous selected page index.</param>
        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
        /// <summary>
        /// Is fired when used changes the current pivot page by scrolling or tapping at the header.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        #region Properties
            #region Appearance
            /// <summary>
            /// Gets or sets the title of the application.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the background color of all the pivot pages.
            /// </summary>
            public Color BackgroundColor
            {
                get { return this[SelectedIndex].BackgroundColor; }
                set
                {
                    foreach (PivotGameItem pgi in Items)
                        pgi.BackgroundColor = value;
                }
            }

            /// <summary>
            /// Gets or sets the foreground (text) color of all the pivot pages.
            /// </summary>
            public Color ForegroundColor
            {
                get { return this[SelectedIndex].ForegroundColor; }
                                set
            {
                foreach (PivotGameItem pgi in Items)
                    pgi.ForegroundColor = value;
            }
            }

            /// <summary>
            /// Gets or sets the background color of scenes on all the pivot pages.
            /// </summary>
            public Color SceneBackgroundColor
            {
                get { return this[SelectedIndex].DrawingArea.BackgroundColor; }
                set
                {
                    foreach (PivotGameItem pgi in Items)
                        pgi.DrawingArea.BackgroundColor = value;
                }
            }

            /// <summary>
            /// Gets or sets the background texture of scenes on all the pivot pages.
            /// </summary>
            public Texture2D SceneBackgroundTexture
            {
                get { return this[SelectedIndex].DrawingArea.BackgroundTexture; }
                set
                {
                    foreach (PivotGameItem pgi in Items)
                        pgi.DrawingArea.BackgroundTexture = value;
                }
            }

            /// <summary>
            /// Gets or sets the rectangle for the scenes on all the pivot pages.
            /// </summary>
            public Rectangle DrawingArea
            {
                get
                {
                    return new Rectangle(this[SelectedIndex].DrawingArea.X,
                                          this[SelectedIndex].DrawingArea.Y,
                                          this[SelectedIndex].DrawingArea.Width,
                                          this[SelectedIndex].DrawingArea.Height);
                }
                set
                {
                    foreach (PivotGameItem pgi in Items)
                    {
                        pgi.DrawingArea.X = value.X;
                        pgi.DrawingArea.Y = value.Y;
                        pgi.DrawingArea.Width = value.Width;
                        pgi.DrawingArea.Height = value.Height;
                        pgi.Update();
                    }
                }
            }

            /// <summary>
            /// Gets or sets the image that is shown while loading the application.
            /// The property should be set in the LoadContent() method before calling base.LoadContent().
            /// </summary>
            public Texture2D SplashScreenImage { get; set; }
            #endregion

            #region Graphics
            /// <summary>
            /// Gets or sets the default GraphicsDeviceManager object of the game.
            /// </summary>
            protected internal GraphicsDeviceManager GraphicsDeviceManager { get; set; }

            /// <summary>
            /// Gets or sets the default SpriteBatch object that is used by the game to draw textures.
            /// </summary>
            protected internal SpriteBatch SpriteBatch { get; set; }

            /// <summary>
            /// Gets or sets the default BasicEffect object that is used by the game to draw 3D scenes.
            /// </summary>
            protected BasicEffect BasicEffect { get; set; }

            /// <summary>
            /// The projection type for BasicEffect.
            /// </summary>
            protected ProjectionType Projection
            {
                get { return proj; }
                set
                {
                    proj = value;
                    if (value == ProjectionType.Perspective)
                    {
                        BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, CameraScale / Zoom), new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, 0), Vector3.Up);
                        BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / Zoom);
                    }
                    else
                    {
                        BasicEffect.View = Matrix.Identity;
                        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-CameraScale / Zoom / 2 + SceneCenterTranslation.X,
                                                           CameraScale / Zoom / 2 + SceneCenterTranslation.X,
                                                           -CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                           CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                           -CameraScale, CameraScale);
                    }
                }
            }

            /// <summary>
            /// Gets or sets the coefficient that is used to calculate the camera's width, height and depth.
            /// </summary>
            public float CameraScale
            {
                get { return cameraWidth; }
                set
                {
                    cameraWidth = value;

                    if (Projection == ProjectionType.Perspective)
                    {
                        BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, value / Zoom), new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, 0), Vector3.Up);
                        BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * value / Zoom);
                    }
                    else
                    {
                        BasicEffect.View = Matrix.Identity;
                        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-value / Zoom / 2 + SceneCenterTranslation.X,
                                                           value / Zoom / 2 + SceneCenterTranslation.X,
                                                           -value * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                           value * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                           -value, value);
                    }
                }
            }

            /// <summary>
            /// Gets or sets the camera's zoom.
            /// </summary>
            public float Zoom
            {
                get { return zoom; }
                set
                {
                    zoom = value;

                    if (Projection == ProjectionType.Perspective)
                    {
                        BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, CameraScale / value), new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, 0), Vector3.Up);
                        BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / value);
                    }
                    else
                    {
                        BasicEffect.View = Matrix.Identity;
                        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-CameraScale / value / 2 + SceneCenterTranslation.X,
                                                        CameraScale / value / 2 + SceneCenterTranslation.X,
                                                        -CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / value / 2 + SceneCenterTranslation.Y,
                                                        CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / value / 2 + SceneCenterTranslation.Y,
                                                        -CameraScale, CameraScale);
                    }
                }
            }

            /// <summary>
            /// Gets or sets the vector by that the 3D scene's center is translated.
            /// </summary>
            protected Vector2 SceneCenterTranslation { set; get; }

            internal int Delta
            {
                get { return delta; }
                set { delta = value; deltaVector = new Vector2(delta, 0); }
            }
            #endregion

            #region Pages
            //The pages
            private List<PivotGameItem> Items { get; set; }

            /// <summary>
            /// Gets or sets the current page index. If the index changes, then the SelectionChanged event is fired.
            /// </summary>
            public int SelectedIndex
            {
                get { return selInd; }
                set
                {
                    int prev = selInd;
                    selInd = value;
                    if (!SelectedIndexSetFirstTime)
                    {
                        CallStack.Add(prev);
                        this[value].headerPosition = new Vector2(10, 40);
                        if (SelectionChanged != null && value != prev)
                            SelectionChanged(this, new SelectionChangedEventArgs(prev));

                        SwitchingTabsNow = true;
                        Delta = 480;
                        if ((prev > value && !(value == 0 && prev >= ItemsCount - 2)) || (prev < value && prev == 0 && value == ItemsCount - 1))
                            Delta = -480;
                    }
                    SelectedIndexSetFirstTime = false;
                }
            }

            /// <summary>
            /// The indexer that enables access to separate pivot pages.
            /// </summary>
            /// <param name="i">The index of page.</param>
            /// <returns>The PivotGameItem object that perpesents the page.</returns>
            public PivotGameItem this[int i]
            {
                get { return Items[i]; }
            }

            /// <summary>
            /// Gets the amount of pages in pivot.
            /// </summary>
            public int ItemsCount
            {
                get { return Items.Count; }
            }

            /// <summary>
            /// Indicates whether the pages are changing at the moment
            /// </summary>
            public bool ChangingPages
            {
                get { return Delta!=0;}
            }
            #endregion

            #region Touches
            /// <summary>
            /// Gets or sets the current state of the touch panel.
            /// It should be initialized before calling base.Update(gameTime).
            /// </summary>
            protected TouchCollection Touches { get; set; }
            #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// The constructor which initializes the GraphicsDeviceManager object.
        /// </summary>
        public PivotGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.PreferredBackBufferWidth = 480;
            GraphicsDeviceManager.PreferredBackBufferHeight = 800;
            GraphicsDeviceManager.IsFullScreen = true;

            FontLoader = new ResourceContentManager(this.Services, DCL.Phone.Xna.Fonts.ResourceManager);
        }
        #endregion

        #region Pivot Methods

            /// <summary>
        /// Adds a new item to the pivot pages collection.
        /// </summary>
        /// <param name="pgi">The item to add.</param>
        public void AddItem(PivotGameItem pgi)
        {
            Items.Add(pgi);
            pgi.Parent = this;
        }

        public void RemoveItem(PivotGameItem pgi)
        {
            Items.Remove(pgi);
        }

        public void RemoveItemAt(int index)
        {
            Items.RemoveAt(index);
        }

        //Used for handling the "Back" button.
        private int GetLastVisitedIndexFromCallStack()
        {
            if (CallStack.Count == 0) return -1;
            else
            {
                int i = CallStack[CallStack.Count - 1];
                CallStack.RemoveAt(CallStack.Count - 1);
                return i;
            }
        }
        #endregion 

        #region Standard Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Items = new List<PivotGameItem>();
            AddItem(new PivotGameItem());

            selInd = 0;

            #region BasicEffect
            BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.LightingEnabled = true;
            BasicEffect.PreferPerPixelLighting = true;
            //BasicEffect.EnableDefaultLighting();
            BasicEffect.VertexColorEnabled = false;

            BasicEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

            BasicEffect.SpecularColor = new Vector3(1, 1, 1);
            BasicEffect.SpecularPower = 60;

            // Set direction of light here, not position!
            BasicEffect.DirectionalLight0.Direction = new Vector3(-1, -1, -1);
            BasicEffect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
            BasicEffect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
            BasicEffect.DirectionalLight0.Enabled = true;

            BasicEffect.Alpha = 1;

            Projection = ProjectionType.Perspective;
            BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, CameraScale / Zoom), new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, 0), Vector3.Up);
            BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / Zoom);
            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #region Default fonts
            HeaderFont = FontLoader.Load<SpriteFont>("Segoe48Bold");
            TitleFont = FontLoader.Load<SpriteFont>("Segoe16Bold");
            ContentFont = FontLoader.Load<SpriteFont>("Segoe16");
            #endregion

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            #region Splash screen
            if (SplashScreenImage != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(SplashScreenImage, new Vector2(0, 0), Color.White);
                SpriteBatch.End();
                GraphicsDevice.Present();
            }
            #endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            #region BasicEffect
            if (BasicEffect != null)
            {
                BasicEffect.Dispose();
                BasicEffect = null;
            }
            #endregion
            #region SpriteBatch
            if (SpriteBatch != null)
            {
                SpriteBatch.Dispose();
                SpriteBatch = null;
            }
            #endregion
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region "Back" button
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                int prev = GetLastVisitedIndexFromCallStack();
                if (prev == -1)
                    this.Exit();
                else
                {
                    int temp = selInd;
                    selInd = prev; //To avoid adding the item to the call stack 
                    this[selInd].headerPosition = new Vector2(10, 40);
                    if (SelectionChanged != null)
                        SelectionChanged(this, new SelectionChangedEventArgs(temp));

                    SwitchingTabsNow = true;
                    Delta = -480;
                    if ((prev > temp && !(temp == 0 && prev >= ItemsCount - 2)) || (prev < temp && prev == 0 && temp == ItemsCount - 1))
                        Delta = 480;
                }
            }
            #endregion

            #region Touches
            if (Touches.Count == 0) Touches = TouchPanel.GetState();
            if (Touches.Count == 1 && !SwitchingTabsNow)
            {
                #region Switching tabs
                if (Touches[0].State == TouchLocationState.Pressed &&
                        Touches[0].Position.Y >= this[SelectedIndex].headerPosition.Y &&
                        Touches[0].Position.Y <= this[SelectedIndex].headerPosition.Y + HeaderFont.MeasureString(this[SelectedIndex].Header).Y)
                {
                    int ind = SelectedIndex;
                    float temp1, temp2 = this[ind].headerPosition.X + HeaderFont.MeasureString(this[ind].Header).X + 10;
                    
                    do
                    {
                        temp1 = temp2;
                        ind++;
                        if (ind == Items.Count) ind=0;
                        temp2 += HeaderFont.MeasureString(this[ind].Header).X + 10;
                        if (Touches[0].Position.X > temp1 && Touches[0].Position.X <= temp2)
                        {
                            SelectedIndex = ind;
                            break;
                        }
                    }while (temp1 < 480);
                }
                #endregion
                #region Moving finger beyond the scene area
                else if (Touches[0].State == TouchLocationState.Moved &&
                    !(Touches[0].Position.X - Delta > this[SelectedIndex].DrawingArea.X &&
                       Touches[0].Position.X - Delta < this[SelectedIndex].DrawingArea.X + this[SelectedIndex].DrawingArea.Width &&
                       //Touches[0].Position.Y > this[SelectedIndex].DrawingArea.Y &&
                       Touches[0].Position.Y < this[SelectedIndex].DrawingArea.Y + this[SelectedIndex].DrawingArea.Height))
                {
                    TouchLocation prevTouch;
                    if (Touches[0].TryGetPreviousLocation(out prevTouch))
                    {
                        Delta += (int)(Touches[0].Position.X - prevTouch.Position.X);
                        this[SelectedIndex].headerPosition = new Vector2
                                (10 + HeaderFont.MeasureString(this[SelectedIndex].Header).X * Delta / 480, 40);

                        if (Projection == ProjectionType.Perspective)
                        {
                            BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X - (float)Delta / 60, SceneCenterTranslation.Y, CameraScale / Zoom), new Vector3(SceneCenterTranslation.X - (float)Delta / 60, SceneCenterTranslation.Y, 0), Vector3.Up);
                            BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / Zoom);
                        }
                        else
                        {
                            BasicEffect.View = Matrix.Identity;
                            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-CameraScale / Zoom * (0.5f + (float)Delta / 480) + SceneCenterTranslation.X,
                                                          CameraScale / Zoom * (0.5f - (float)Delta / 480) + SceneCenterTranslation.X,
                                                          -CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                          CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                          -CameraScale, CameraScale);
                        }
                    }
                }
                #endregion
                #region Releasing finger - we may have to switch tabs
                else if (Touches[0].State == TouchLocationState.Released)
                {
                    if (Delta > 100)
                    {
                        Delta = 0;

                        SelectedIndex = (SelectedIndex == 0) ? (ItemsCount - 1) : (SelectedIndex - 1);
                    }
                    else if (Delta < -100)
                    {
                        Delta = 0;

                        SelectedIndex = (SelectedIndex == ItemsCount-1) ? (0) : (SelectedIndex + 1);
                    }
                    else
                    {
                        Delta = 0;
                    }
                    this[SelectedIndex].headerPosition = new Vector2(10, 40);

                    if (Projection == ProjectionType.Perspective)
                    {
                        BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, CameraScale / Zoom), new Vector3(SceneCenterTranslation.X, SceneCenterTranslation.Y, 0), Vector3.Up);
                        BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / Zoom);
                    }
                    else
                    {
                        BasicEffect.View = Matrix.Identity;
                        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-CameraScale / Zoom / 2 + SceneCenterTranslation.X,
                                                                CameraScale / Zoom / 2 + SceneCenterTranslation.X,
                                                                -CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                                CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                                -CameraScale, CameraScale);
                    }
                }
                #endregion
            }
            #endregion

            #region Switching tabs animation
            if (SwitchingTabsNow)
            {
                Delta += (Delta<0)?80:-80;
                if (Delta == 0) SwitchingTabsNow = false;

                if (Projection == ProjectionType.Perspective)
                {
                    BasicEffect.View = Matrix.CreateLookAt(new Vector3(SceneCenterTranslation.X - (float)Delta / 60, SceneCenterTranslation.Y, CameraScale / Zoom), new Vector3(SceneCenterTranslation.X - (float)Delta / 60, SceneCenterTranslation.Y, 0), Vector3.Up);
                    BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDeviceManager.PreferredBackBufferWidth / GraphicsDeviceManager.PreferredBackBufferHeight, 0.1f, 2 * CameraScale / Zoom);
                }
                else
                {
                    BasicEffect.View = Matrix.Identity;
                    BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(-CameraScale / Zoom * (0.5f + (float)Delta / 480) + SceneCenterTranslation.X,
                                                          CameraScale / Zoom * (0.5f - (float)Delta / 480) + SceneCenterTranslation.X,
                                                          -CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                          CameraScale * GraphicsDeviceManager.PreferredBackBufferHeight / GraphicsDeviceManager.PreferredBackBufferWidth / Zoom / 2 + SceneCenterTranslation.Y,
                                                          -CameraScale, CameraScale);
                }
            }
            #endregion

            this[SelectedIndex].Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Items[SelectedIndex].Draw();

            base.Draw(gameTime);
        }
        #endregion

        #region Other methods
        /// <summary>
        /// Draws a string (considers the possible page's displacement).
        /// </summary>
        /// <param name="font">Font of the text.</param>
        /// <param name="text">The text to draw</param>
        /// <param name="position">The position of the left upper corner of the text.</param>
        /// <param name="color">Color of the text.</param>
        public void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(font, text, position + deltaVector, color);
        }

        /// <summary>
        /// Renders a texture (considers the possible page's displacement).
        /// </summary>
        /// <param name="sprite">The texture to draw.</param>
        /// <param name="position">The position of the left upper corner of the texture.</param>
        /// <param name="color">Color of the texture.</param>
        public void DrawSprite(Texture2D sprite, Vector2 position, Color color)
        {
            SpriteBatch.Draw(sprite, position + deltaVector, color);
        }
        #endregion
    }



    /// <summary>
    /// Arguments for the SelectionChanged event that contain the previous selected page index.
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the page that was current before the selection changed.
        /// </summary>
        public int PreviousIndex { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="previousIndex">The index of the page that was current before the selection changed.</param>
        public SelectionChangedEventArgs(int previousIndex)
        {
            PreviousIndex = previousIndex;
        }
    }

    /// <summary>
    /// The projection type for BasicEffect
    /// </summary>
    public enum ProjectionType
    {
        /// <summary>
        /// Perspective projection.
        /// </summary>
        Perspective,

        /// <summary>
        /// Orthographic projection.
        /// </summary>
        Orthographic
    }
}

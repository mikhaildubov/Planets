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
    /// Represents single pivot pages that contain application title, tab header,
    /// a 3D scene and some additional graphics (e.g. text).
    /// A dynamic list of type PivotGameItem is stored in any class that derives from PivotGame.
    /// </summary>
    public class PivotGameItem
    {
        #region Fields
        /// <summary>
        /// The rectangle for a scene on the page.
        /// </summary>
        public DrawingArea DrawingArea;
        RenderTarget2D tinyTexture; //A texture of size 1x1 that can be colored in any way
        Rectangle rectDrawingArea, rectBackgr1, rectBackgr2, rectBackgr3, rectBackgr4; //Rectangles to draw
        PivotGame pg; //Parent
        internal Vector2 headerPosition, titlePosition; //Text positions
        Color bkgColor, frgColor, deactivatedColor; //Rectangle colors
        float temp; //used in Draw() method
        int ind; //used in Draw() method
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the background color of the page.
        /// </summary>
        public Color BackgroundColor
        {
            get { return bkgColor; }
            set
            {
                bkgColor = value;
                deactivatedColor = Color.FromNonPremultiplied((frgColor.R + 2*bkgColor.R) / 3,
                                            (frgColor.G + 2 * bkgColor.G) / 3, (frgColor.B + 2 * bkgColor.B) / 3, 255);
            }
        }

        /// <summary>
        /// Gets or sets the foreground (text) color of the page.
        /// </summary>
        public Color ForegroundColor
        {
            get { return frgColor; }
            set
            {
                frgColor = value;
                deactivatedColor = Color.FromNonPremultiplied((frgColor.R + 2 * bkgColor.R) / 3,
                                            (frgColor.G + 2 * bkgColor.G) / 3, (frgColor.B + 2 * bkgColor.B) / 3, 255);
            }
        }

        /// <summary>
        /// Gets or sets the tab header.
        /// </summary>
        public string Header { set; get; }

        /// <summary>
        /// Gets or sets the pivot the page belongs to.
        /// </summary>
        public PivotGame Parent
        {
            set
            {
                pg = value;
                tinyTexture = new RenderTarget2D(value.GraphicsDevice, 1, 1);
                value.GraphicsDevice.SetRenderTarget(tinyTexture);
                value.GraphicsDevice.Clear(Color.White);
                value.GraphicsDevice.SetRenderTarget(null);
            }
            get { return pg; }
        }
        #endregion

        #region Events
        /// <summary>
        /// An event that is fired each drawing cycle. 
        /// Its handler should have the code for drawing
        /// the 3D scene.
        /// </summary>
        public event EventHandler DrawScene;

        /// <summary>
        /// An event that is fired each drawing cycle. 
        /// Its handler should have the code for drawing
        /// additional graphics, e.g. texts.
        /// </summary>
        public event EventHandler DrawFrame;
        #endregion

        #region Constructors
        /// <summary>
        /// The page's constructor. Initializes the drawing area and the page colors by default values.
        /// </summary>
        public PivotGameItem()
        {
            DrawingArea.BackgroundColor = Color.DarkBlue;
            DrawingArea.X = 10;
            DrawingArea.Y = 200;
            DrawingArea.Width = 460;
            DrawingArea.Height = 300;

            headerPosition = new Vector2(10, 40);
            titlePosition = new Vector2(10, 0);

            BackgroundColor = Color.CornflowerBlue;
            ForegroundColor = Color.DarkGreen;
        }
        #endregion

        #region Methods
        //Updates the coordinates of the rectangle.
        internal void Update()
        {
            rectDrawingArea.X = DrawingArea.X + Parent.Delta;
            rectDrawingArea.Y = DrawingArea.Y;
            rectDrawingArea.Width = DrawingArea.Width;
            rectDrawingArea.Height = DrawingArea.Height;

            rectBackgr1.X = Parent.Delta;
            rectBackgr1.Y = 0;
            rectBackgr1.Width = 480;
            rectBackgr1.Height = rectDrawingArea.Y;

            rectBackgr2.X = Parent.Delta;
            rectBackgr2.Y = rectDrawingArea.Y;
            rectBackgr2.Width = rectDrawingArea.X - Parent.Delta;
            rectBackgr2.Height = rectDrawingArea.Height;

            rectBackgr3.X = rectDrawingArea.X + rectDrawingArea.Width;
            rectBackgr3.Y = rectDrawingArea.Y;
            rectBackgr3.Width = 480 - Parent.Delta - rectBackgr3.X;
            rectBackgr3.Height = rectDrawingArea.Height;

            rectBackgr4.X = Parent.Delta;
            rectBackgr4.Y = rectDrawingArea.Y + rectDrawingArea.Height;
            rectBackgr4.Width = 480;
            rectBackgr4.Height = 800 - rectBackgr4.Y;

        }

        //Is called from Draw(gameTime) methods from the PivotGame class.
        internal void Draw()
        {
            Parent.GraphicsDevice.Clear(BackgroundColor);

            Parent.SpriteBatch.Begin();
            if(DrawingArea.BackgroundTexture==null)
                Parent.SpriteBatch.Draw(tinyTexture, rectDrawingArea, DrawingArea.BackgroundColor);
            else
                Parent.SpriteBatch.Draw(DrawingArea.BackgroundTexture, rectDrawingArea, Color.White);
            Parent.SpriteBatch.End();

            if (DrawScene != null)
                DrawScene(this, EventArgs.Empty);
            Parent.SpriteBatch.Begin();
            Parent.SpriteBatch.Draw(tinyTexture, rectBackgr1, BackgroundColor);
            Parent.SpriteBatch.Draw(tinyTexture, rectBackgr2, BackgroundColor);
            Parent.SpriteBatch.Draw(tinyTexture, rectBackgr3, BackgroundColor);
            Parent.SpriteBatch.Draw(tinyTexture, rectBackgr4, BackgroundColor);

            Parent.SpriteBatch.DrawString(Parent.TitleFont, Parent.Title, titlePosition, ForegroundColor);
            Parent.SpriteBatch.DrawString(Parent.HeaderFont, Header, headerPosition, ForegroundColor);
            temp = headerPosition.X + Parent.HeaderFont.MeasureString(Header).X + 10;
            ind = Parent.SelectedIndex + 1;
            while (temp < 480)
            {
                if (ind == Parent.ItemsCount) ind = 0;
                Parent.SpriteBatch.DrawString(Parent.HeaderFont, Parent[ind].Header, new Vector2(temp, headerPosition.Y), deactivatedColor);
                temp += Parent.HeaderFont.MeasureString(Parent[ind].Header).X + 10;
                ind++;
            }
            ind = (Parent.SelectedIndex == 0) ? (Parent.ItemsCount - 1) : (Parent.SelectedIndex - 1);
            Parent.SpriteBatch.DrawString(Parent.HeaderFont, Parent[ind].Header, new Vector2(headerPosition.X - Parent.HeaderFont.MeasureString(Parent[ind].Header).X - 10, headerPosition.Y), deactivatedColor);
            Parent.SpriteBatch.End();

            if (DrawFrame != null)
                DrawFrame(this, EventArgs.Empty);

        }
        #endregion
    }

    /// <summary>
    /// A structure that represents the rectangle in which the 3D scene is being drawn.
    /// </summary>
    public struct DrawingArea
    {
        /// <summary>
        /// The X coordinate of the left upper corner of the rectangle.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y coordinate of the left upper corner of the rectangle.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The background color of the rectangle.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// The background texture of the rectangle.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }
    }
}

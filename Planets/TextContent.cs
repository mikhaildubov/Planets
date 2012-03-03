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
        #region Textures
        Texture2D icoDiameter, icoMass, icoDistance, icoRotationPeriod, icoRevolutionPeriod, icoGravitationalPull, icoTemperatures,
            icoMoon,
            icoPhobos, icoDeimos,
            icoEuropa, icoIo, icoCallisto, icoGanymede,
            icoTitan, icoDione, icoEnceladus, icoIapetus, icoTethys, icoMimas, icoRhea,
            icoTitania, icoOberon, icoAriel, icoUmbriel, icoMiranda,
            icoTriton, icoNereid, icoProteus,

            icoGPSon, icoGPSsearch, icoGPSoff;

            Vector2 posMoon = new Vector2(320, 630), posPhobos = new Vector2(320, 630), posDeimos = new Vector2(320, 680),
                posIo = new Vector2(320, 630), posEuropa = new Vector2(320, 670), posGanymede = new Vector2(320, 710),
                posCallisto = new Vector2(320, 750), posMimas = new Vector2(320, 630), posEnceladus = new Vector2(320, 653),
                posTethys = new Vector2(320, 676), posDione = new Vector2(320, 699), posRhea = new Vector2(320, 722),
                posTitan = new Vector2(320, 745), posIapetus = new Vector2(320, 768), posMiranda = new Vector2(320, 630),
                posAriel = new Vector2(320, 662), posUmbriel = new Vector2(320, 694), posTitania = new Vector2(320, 726),
                posOberon = new Vector2(320, 758), posProteus = new Vector2(320, 630), posTriton = new Vector2(320, 670),
                posNereid = new Vector2(320, 710);
        #endregion

        #region Texts
            Vector2 coordPos1 = new Vector2(280, 2), coordPos2 = new Vector2(280, 18);
            Vector2 coordGps = new Vector2(440, 0);

            string strCharacteristics = "Characteristics:", strDiameter = "Diameter:", strMass = "Mass:", strDistance = "Remoteness:",
                strRotationPeriod = "Day:", strRevolutionPeriod = "Year:",
                strGravitationalPull = "Gravitation:", strTemperatures = "Temp.:", strRemark = "*Measured relative to the Earth",
                strMoons = "Moons:",

                strDot,

                mercuryDiameter, mercuryMass, mercuryDistance, mercuryRotationPeriod, mercuryRevolutionPeriod, mercuryGravitationalPull, mercuryTemperatures, mercuryMoons,
                venusDiameter, venusMass, venusDistance, venusRotationPeriod, venusRevolutionPeriod, venusGravitationalPull, venusTemperatures, venusMoons,
                earthDiameter, earthMass, earthDistance, earthRotationPeriod, earthRevolutionPeriod, earthGravitationalPull, earthTemperatures, earthMoons,
                marsDiameter, marsMass, marsDistance, marsRotationPeriod, marsRevolutionPeriod, marsGravitationalPull, marsTemperatures, marsMoons,
                jupiterDiameter, jupiterMass, jupiterDistance, jupiterRotationPeriod, jupiterRevolutionPeriod, jupiterGravitationalPull, jupiterTemperatures, jupiterMoons,
                saturnDiameter, saturnMass, saturnDistance, saturnRotationPeriod, saturnRevolutionPeriod, saturnGravitationalPull, saturnTemperatures, saturnMoons,
                uranusDiameter, uranusMass, uranusDistance, uranusRotationPeriod, uranusRevolutionPeriod, uranusGravitationalPull, uranusTemperatures, uranusMoons,
                neptuneDiameter, neptuneMass, neptuneDistance, neptuneRotationPeriod, neptuneRevolutionPeriod, neptuneGravitationalPull, neptuneTemperatures, neptuneMoons,

                strMoon = "Moon", strPhobos = "Phobos", strDeimos = "Deimos", strIo = "Io", strEuropa = "Europa",
                strGanymede = "Ganymede", strCallisto = "Callisto", strTitan = "Titan", strDione = "Dione",
                strEnceladus = "Enceladus", strIapetus = "Iapetus", strTethys = "Tethys", strMimas = "Mimas", strRhea = "Rhea",
                strTitania = "Titania", strOberon = "Oberon", strAriel = "Ariel", strUmbriel = "Umbriel",
                strMiranda = "Miranda", strProteus = "Proteus", strTriton = "Triton", strNereid = "Nereid",

                strBuy = "Buy now!"; //Full version

            Vector2 coordCharacteristics = new Vector2(5, 600), coordDiameter = new Vector2(30, 625), coordMass = new Vector2(30, 647),
                coordDistance = new Vector2(30, 669), coordRotationPeriod = new Vector2(30, 691), coordRevolutionPeriod = new Vector2(30, 713),
                coordTemperatures = new Vector2(30, 735), coordGravitationalPull = new Vector2(30, 757), coordNotation = new Vector2(10, 787),
                coordMoons = new Vector2(320, 600),

                spDiameter = new Vector2(7, 631), spMass = new Vector2(7, 653),
                spDistance = new Vector2(7, 675), spRotationPeriod = new Vector2(7, 697), spRevolutionPeriod = new Vector2(7, 719),
                spTemperatures = new Vector2(7, 741), spGravitationalPull = new Vector2(7, 763),

                plDiameter = new Vector2(155, 625), plMass = new Vector2(100, 647),
                plDistance = new Vector2(178, 669), plRotationPeriod = new Vector2(85, 691), plRevolutionPeriod = new Vector2(100, 713),
                plTemperatures = new Vector2(110, 735), plGravitationalPull = new Vector2(190, 757),
                plMoons = new Vector2(415, 600),

                coordMoon = new Vector2(390, 640), coordPhobos = new Vector2(380, 640), coordDeimos = new Vector2(380, 690),
                coordIo = new Vector2(370, 635), coordEuropa = new Vector2(370, 675), coordGanymede = new Vector2(370, 715),
                coordCallisto = new Vector2(370, 755), coordMimas = new Vector2(355, 627), coordEnceladus = new Vector2(355, 650),
                coordTethys = new Vector2(355, 673), coordDione = new Vector2(355, 696), coordRhea = new Vector2(355, 719),
                coordTitan = new Vector2(355, 742), coordIapetus = new Vector2(355, 765), coordMiranda = new Vector2(362, 630),
                coordAriel = new Vector2(362, 662), coordUmbriel = new Vector2(362, 694), coordTitania = new Vector2(362, 726),
                coordOberon = new Vector2(362, 758), coordProteus = new Vector2(370, 635), coordTriton = new Vector2(370, 675),
                coordNereid = new Vector2(370, 715),

                coordBuy = new Vector2(320, 10); //290, 10
        #endregion


        void LoadTextContent()
        {
            strDot = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            mercuryDiameter = "0" + strDot + "382";
            mercuryMass = "0" + strDot + "06";
            mercuryDistance = "0" + strDot + "39";
            mercuryRotationPeriod = "58" + strDot + "64";
            mercuryRevolutionPeriod = "0" + strDot + "24";
            mercuryTemperatures = "-290/810 F°";
            mercuryGravitationalPull = "0" + strDot + "38";
            mercuryMoons = "0";

            venusDiameter = "0" + strDot + "949";
            venusMass = "0" + strDot + "82";
            venusDistance = "0" + strDot + "72";
            venusRotationPeriod = "243" + strDot + "02";
            venusRevolutionPeriod = "0" + strDot + "62";
            venusTemperatures = "864 F°";
            venusGravitationalPull = "0" + strDot + "91";
            venusMoons = "0";

            earthDiameter = "1" + strDot + "00";
            earthMass = "1" + strDot + "00";
            earthDistance = "1" + strDot + "00";
            earthRotationPeriod = "1" + strDot + "00";
            earthRevolutionPeriod = "1" + strDot + "00";
            earthTemperatures = "-128/136 F°";
            earthGravitationalPull = "1" + strDot + "00";
            earthMoons = "1";

            marsDiameter = "0" + strDot + "532";
            marsMass = "0" + strDot + "11";
            marsDistance = "1" + strDot + "52";
            marsRotationPeriod = "1" + strDot + "03";
            marsRevolutionPeriod = "1" + strDot + "88";
            marsTemperatures = "-189/63 F°";
            marsGravitationalPull = "0" + strDot + "38";
            marsMoons = "2";

            jupiterDiameter = "11" + strDot + "209";
            jupiterMass = "317" + strDot + "80";
            jupiterDistance = "5" + strDot + "20";
            jupiterRotationPeriod = "0" + strDot + "41";
            jupiterRevolutionPeriod = "11" + strDot + "86";
            jupiterTemperatures = "-193/63 F°";
            jupiterGravitationalPull = "2" + strDot + "54";
            jupiterMoons = "63";

            saturnDiameter = "9" + strDot + "449";
            saturnMass = "95" + strDot + "20";
            saturnDistance = "9" + strDot + "54";
            saturnRotationPeriod = "0" + strDot + "43";
            saturnRevolutionPeriod = "29" + strDot + "46";
            saturnTemperatures = "-285 F°";
            saturnGravitationalPull = "0" + strDot + "93";
            saturnMoons = "62";

            uranusDiameter = "4" + strDot + "007";
            uranusMass = "14" + strDot + "60";
            uranusDistance = "19" + strDot + "22";
            uranusRotationPeriod = "0" + strDot + "72";
            uranusRevolutionPeriod = "84" + strDot + "01";
            uranusTemperatures = "-357 F°";
            uranusGravitationalPull = "0" + strDot + "80";
            uranusMoons = "27";


            neptuneDiameter = "3" + strDot + "883";
            neptuneMass = "17" + strDot + "20";
            neptuneDistance = "30" + strDot + "06";
            neptuneRotationPeriod = "0" + strDot + "67";
            neptuneRevolutionPeriod = "164" + strDot + "80";
            neptuneTemperatures = "-360 F°";
            neptuneGravitationalPull = "1" + strDot + "20";
            neptuneMoons = "13";
        }

        void LoadIcons()
        {
            icoGPSon = Content.Load<Texture2D>("GPS_on");
            icoGPSoff = Content.Load<Texture2D>("GPS_off");
            icoGPSsearch = Content.Load<Texture2D>("GPS_search");
            icoDiameter = Content.Load<Texture2D>("diameter");
            icoMass = Content.Load<Texture2D>("mass");
            icoDistance = Content.Load<Texture2D>("remoteness");
            icoRotationPeriod = Content.Load<Texture2D>("day");
            icoRevolutionPeriod = Content.Load<Texture2D>("year");
            icoGravitationalPull = Content.Load<Texture2D>("gravitation");
            icoTemperatures = Content.Load<Texture2D>("temperatures");

            icoMoon = Content.Load<Texture2D>("ico_moon");
            icoPhobos = Content.Load<Texture2D>("ico_phobos");
            icoDeimos = Content.Load<Texture2D>("ico_deimos");
            icoIo = Content.Load<Texture2D>("ico_io");
            icoEuropa = Content.Load<Texture2D>("ico_europa");
            icoGanymede = Content.Load<Texture2D>("ico_ganymede");
            icoCallisto = Content.Load<Texture2D>("ico_callisto");
            icoTitan = Content.Load<Texture2D>("ico_titan");
            icoDione = Content.Load<Texture2D>("ico_dione");
            icoEnceladus = Content.Load<Texture2D>("ico_enceladus");
            icoIapetus = Content.Load<Texture2D>("ico_iapetus");
            icoTethys  = Content.Load<Texture2D>("ico_tethys");
            icoMimas = Content.Load<Texture2D>("ico_mimas");
            icoRhea = Content.Load<Texture2D>("ico_rhea");
            icoTitania = Content.Load<Texture2D>("ico_titania");
            icoOberon = Content.Load<Texture2D>("ico_oberon");
            icoAriel = Content.Load<Texture2D>("ico_ariel");
            icoUmbriel = Content.Load<Texture2D>("ico_umbriel");
            icoMiranda = Content.Load<Texture2D>("ico_miranda");
            icoTriton = Content.Load<Texture2D>("ico_triton");
            icoNereid = Content.Load<Texture2D>("ico_nereid");
            icoProteus = Content.Load<Texture2D>("ico_proteus");
        }
    }
}

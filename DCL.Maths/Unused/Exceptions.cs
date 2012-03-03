using System;

namespace DCL.Maths
{
    public class DimensionsDiscordanceException : System.Exception
    {
        public override string Message
        {
            get
            {
                return "Unable to proceed the operation because dimensions of matrixes are discordant.";
            }
        }

        public DimensionsDiscordanceException():base() {}
        public DimensionsDiscordanceException(string message) : base(message) { }
    }
}

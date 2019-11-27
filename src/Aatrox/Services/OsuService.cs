using OsuSharp;

namespace Aatrox.Services
{
    public sealed class OsuService
    {
        public OsuClient Osu { get; }

        public OsuService(OsuClient osu)
        {
            Osu = osu;
        }
    }
}

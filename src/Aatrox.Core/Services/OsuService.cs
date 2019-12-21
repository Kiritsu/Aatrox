using OsuSharp;

namespace Aatrox.Core.Services
{
    public sealed class OsuService
    {
        private readonly OsuClient _osu;

        public OsuService(OsuClient osu)
        {
            _osu = osu;
        }
    }
}

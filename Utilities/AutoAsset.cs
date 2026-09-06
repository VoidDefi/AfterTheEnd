using ReLogic.Content;
using Terraria.ModLoader;

namespace AfterTheEnd.Utilities
{
    public class AutoAsset<T> where T : class
    {
        public string Name { get; private set; }

        private Asset<T> asset;

        public Asset<T> Asset
        {
            get
            {
                if (asset == null)
                    asset = ModContent.Request<T>(Name, AssetRequestMode.ImmediateLoad);

                return asset;
            }
        }

        public T Value => Asset.Value;

        public AutoAsset(string name)
        {
            Name = name;
        }
    }
}

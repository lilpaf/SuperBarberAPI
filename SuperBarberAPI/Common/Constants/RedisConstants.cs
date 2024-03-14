namespace Common.Constants
{
    public class RedisConstants
    {
        private const string Prefix = "SuperBarber";
        public const string CitiesKeyRedis = $"{Prefix}:Cities";
        public const string NeighborhoodsKeyRedis = $"{Prefix}:Neighborhoods:{{0}}";
    }
}

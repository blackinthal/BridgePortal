using System.Linq;

namespace Bridge.Domain.Extensions
{
    public static class  Extensions
    {
        public static IQueryable<Contract> GetContractsByTrumpAndPlayingSide(this IQueryable<Contract> contracts,
            bool playedByNS, Trump trump)
        {
            var query =
                contracts.Where(
                    c =>
                        c.Trump == trump
                    );

            return (playedByNS ? query.PlayedByEW() : query.PlayedByNS());
        }

        public static IQueryable<Contract> PlayedByNS(this IQueryable<Contract> contracts)
        {
            return
                contracts
                .Where(c => c.PlayerPosition == PlayerPosition.North || c.PlayerPosition == PlayerPosition.South);
        }

        public static IQueryable<Contract> PlayedByEW(this IQueryable<Contract> contracts)
        {
            return
                contracts
                .Where(c => c.PlayerPosition == PlayerPosition.East || c.PlayerPosition == PlayerPosition.West);
        } 
    }
}

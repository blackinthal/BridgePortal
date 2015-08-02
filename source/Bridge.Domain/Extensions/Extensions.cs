using System.Collections.Generic;
using System.Linq;

namespace Bridge.Domain.Extensions
{
    public static class  Extensions
    {
        public static List<Contract> GetContractsByTrumpAndPlayingSide(this List<Contract> contracts,
            bool playedByNS, Trump trump)
        {
            return
                contracts.Where(
                    c =>
                        c.Trump == trump &&
                        ((playedByNS && (c.PlayerPosition == PlayerPosition.North || c.PlayerPosition == PlayerPosition.South))
                        ||
                        (!playedByNS && (c.PlayerPosition == PlayerPosition.West || c.PlayerPosition == PlayerPosition.East)))
                    ).ToList();
        }
    }
}

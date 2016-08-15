using System;
using System.Linq;
using Bridge.Domain.StaticModels;

namespace Bridge.Domain
{
    public class Contract
    {
        public Contract()
        {
            Trump = Trump.NoTrump;
        }

        public int Level { get; set; }

        public Trump Trump { get; set; }

        public PlayerPosition PlayerPosition { get; set; }

        public override string ToString()
        {
            return String.Format("{0}:{1}{2}", PlayerPosition, Level, Trump);
        }

        public string Display()
        {
            return String.Format("{0}{1}", Level, Trump) + (Doubled ? "x" : string.Empty) + (Redoubled ? "x" : string.Empty);
        }

        public string Notation()
        {
            return String.Format("{0}{1}", Level, Trump.Suit.ShortName);
        }

        public Contract(string contract, PlayerPosition declarer)
        {
            var value = 0;
            int.TryParse(contract[0].ToString(), out value);
            Level = value;
            PlayerPosition = declarer;
            var suit = Suit.Suits.FirstOrDefault(x => x.ShortName == new string(contract[1], 1));
            Trump = suit !=null ? new Trump(suit) : Trump.NoTrump;
            Doubled = contract.ToLower().Count(x => x == 'x') == 1;
            Redoubled = contract.ToLower().Count(x => x == 'x') == 2;
        }

        public bool Doubled { get; set; }
        public bool Redoubled { get; private set; }
        public bool Undoubled { get { return !Doubled && !Redoubled; } }
        public int TricksToBeMade { get { return Level + 6; } }
        public bool SmallSlam { get { return TricksToBeMade == 12; } }
        public bool BigSlam { get { return TricksToBeMade == 13; } }

        public Contract GetNextContract()
        {
            if (Level == 7 && Trump == Trump.NoTrump)
                return null;

            var denomination = Trump == Trump.NoTrump ? Level + 1 : Level;
            var trump = Trump.TrumpOrder.ElementAt((Trump.TrumpOrder.IndexOf(Trump) + 1) % 5);

            return new Contract{Trump = trump, Level = denomination};
        }

        public Contract Clone()
        {
            return new Contract
            {
                Doubled = this.Doubled,
                Trump = this.Trump,
                Level = this.Level,
                PlayerPosition = this.PlayerPosition
            };
        }

        public bool CanBidOver(Contract referenceContract)
        {
            if (Level > referenceContract.Level)
                return true;
            if (Level < referenceContract.Level)
                return false;

            var currentTrumpOrder = Trump.TrumpOrder.IndexOf(Trump);
            var refTrumpOrder = Trump.TrumpOrder.IndexOf(referenceContract.Trump);

            return currentTrumpOrder > refTrumpOrder;
        }
    }
}

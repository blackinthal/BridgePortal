using System;
using System.Linq;

namespace Bridge.Domain
{
    public class Contract
    {
        public Contract()
        {
            Trump = Trump.NoTrump;
        }

        public int Value { get; set; }

        public Trump Trump { get; set; }

        public PlayerPosition PlayerPosition { get; set; }

        public override string ToString()
        {
            return String.Format("{0}:{1}{2}", PlayerPosition, Value, Trump);
        }

        public Contract(string contract, PlayerPosition declarer)
        {
            var value = 0;
            int.TryParse(contract[0].ToString(), out value);
            Value = value;
            PlayerPosition = declarer;
            var suit = Suit.Suits.FirstOrDefault(x => x.ShortName == new string(contract[1], 1));
            Trump = suit !=null ? new Trump(suit) : Trump.NoTrump;
            Doubled = contract.ToLower().Count(x => x == 'x') == 1;
            Redoubled = contract.ToLower().Count(x => x == 'x') == 2;
        }

        public bool Doubled { get; set; }
        public bool Redoubled { get; private set; }
        public bool Undoubled { get { return !Doubled && !Redoubled; } }
        public int TricksToBeMade { get { return Value + 6; } }
        public bool SmallSlam { get { return TricksToBeMade == 12; } }
        public bool BigSlam { get { return TricksToBeMade == 13; } }

        public Contract GetNextContract()
        {
            if (Value == 7 && Trump == Trump.NoTrump)
                return null;

            var denomination = Trump == Trump.NoTrump ? Value + 1 : Value;
            var trump = Trump.TrumpOrder.ElementAt((Trump.TrumpOrder.IndexOf(Trump) + 1) % 5);

            return new Contract{Trump = trump, Value = denomination};
        }

        public Contract Copy()
        {
            return new Contract
            {
                Doubled = this.Doubled,
                Trump = this.Trump,
                Value = this.Value,
                PlayerPosition = this.PlayerPosition
            };
        }
    }
}

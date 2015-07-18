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

        public int Value { get; set; }

        public Trump Trump { get; set; }

        public PlayerPosition PlayerPosition { get; set; }

        public override string ToString()
        {
            return String.Format("{0}:{1}{2}", PlayerPosition, Value, Trump);
        }

        public Contract(string contract, PlayerPosition declarer)
        {
            Value = int.Parse(contract[0].ToString());
            PlayerPosition = declarer;
            var suit = Suit.Suits.FirstOrDefault(x => x.ShortName == new string(contract[1], 1));
            Trump = suit !=null ? new Trump(suit) : Trump.NoTrump;
        }

        public bool Doubled { get; set; }
        public bool Redoubled { get; set; }
        public bool Undoubled { get { return !Doubled && !Redoubled; } }
        public int TricksToBeMade
        {
            get { return Value + 6; }
        }
        public bool SmallSlam { get { return TricksToBeMade == 12; } }
        public bool BigSlam { get { return TricksToBeMade == 13; } }
    }
}

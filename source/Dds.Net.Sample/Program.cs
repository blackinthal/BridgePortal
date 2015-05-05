using System;
using System.Linq;
using System.Text;
using Bridge.Domain;
using Bridge.Domain.Utils;

namespace Dds.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dds = new DdsConnect();
            var pbnCode = "E:J832.KQ52.A852.A AK.T864.T7643.KT Q974.9.KJ.J98652 T65.AJ73.Q9.Q743";
            Console.WriteLine("Board: " + pbnCode);
            
            var game = BridgeHelper.GetGameFromPbn(pbnCode, "4S", "W");
            var res = dds.CalculateMakeableContracts(pbnCode);
            Console.WriteLine("Best results:");
            foreach (var contract in res)
            {
                Console.WriteLine(contract);
            }
            Console.WriteLine("------------- Game Starts ----------------");
            Console.WriteLine("Trump: " + game.Contract.Trump);
            //var player = game.Declarer;
            var player = PlayerPosition.North;
            while (game.CardsRemaning > 0)
            {
                var result = dds.SolveBoardPbnBestCard(game);
                Console.WriteLine(player + ": " + result.Card + ". Score: " + result.Score);
                player = game.PlayCard(result.Card, player);

                if (game.CurrentTrick.Deck.Count == 0)
                {
                    Console.WriteLine("Trick Winner: " + game.Tricks.Last().TrickWinner);
                }
            }

            Console.WriteLine("-----------Results----------");
            Console.WriteLine("South/North: " + game.NorthSouthTricksMade + " tricks");
            Console.WriteLine("East/West: " + game.EastWestTricksMade + " tricks");

            Console.ReadKey();
        }
    }
}
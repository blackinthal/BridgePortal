using System;
using System.Linq;
using Bridge.Domain;
using Bridge.Domain.Utils;

namespace Dds.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dds = new DoubleDummyModule();
            var pbnCode = "E:K9742.763.Q.K764 AT6.AJ98.A5.QT53 853.QT.KT742.J92 QJ.K542.J9863.A8";
            Console.WriteLine("Board: " + pbnCode);
            
        
            var res = DoubleDummyModule.CalculateMakeableContracts(pbnCode);
            Console.WriteLine("Best results:");
            foreach (var contract in res)
            {
                Console.WriteLine(contract);
            }

            var game = BridgeHelper.GetGameFromPbn(pbnCode, "4S", "W");

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
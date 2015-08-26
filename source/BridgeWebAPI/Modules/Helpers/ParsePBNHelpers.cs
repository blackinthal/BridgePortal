using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Bridge.Domain;
using Bridge.Domain.StaticModels;
using WebGrease.Css.Extensions;

namespace Bridge.WebAPI.Modules.Helpers
{
    public static class ParsePBNHelpers
    {
        public static int GetPlayerIdFromString(string dealer)
        {
            switch (dealer.ToLower().Trim())
            {
                case "n": return (int)SysPlayer.N;
                case "e": return (int)SysPlayer.E;
                case "w": return (int)SysPlayer.W;
                default: return (int)SysPlayer.S;
            }
        }

        /// <summary>
        /// We use LIN format ex: http://www.bridgebase.com/tools/handviewer.html?lin=md|1S2389JHTD3JC237KA,S7TH4QKD678TC4569,S456KAH25D25KACJQ,|rh||ah|Board%207|sv|e|
        /// </summary>
        /// <param name="pbnRepresentation"></param>
        /// <param name="boardNo"></param>
        /// <param name="vulnerability"></param>
        /// <returns></returns>
        public static string ConvertPBNToHandViewerInput(string pbnRepresentation, int boardNo, SysVulnerabilityEnum vulnerability)
        {
            var split = pbnRepresentation.Split(':');

            var dealer = split[0].ToLower();
            var hands = split[1].Split(' ');
            var order = new List<string> { "s", "w", "n", "e" };
            var suitOrder = new List<string> { "S", "H", "D", "C" };

            var indexOfDeclarer = order.IndexOf(dealer);
            var parsedHands = new string[4];

            var sb = new StringBuilder("http://www.bridgebase.com/tools/handviewer.html?lin=md|");

            sb.Append(String.Format("{0}", (indexOfDeclarer + 1)));

            for (var i = indexOfDeclarer; i < indexOfDeclarer + 4; i++)
            {
                var currentHand = hands[i - indexOfDeclarer];
                var suits = currentHand.Split('.');

                var hand = new StringBuilder();

                for (var j = 0; j < 4; j++)
                {
                    hand.Append(suitOrder[j]).Append(suits[j]);
                }

                parsedHands[i % 4] = hand.ToString();
            }

            for (var i = 0; i < 4; i++)
            {
                sb.Append(String.Format("{0},", parsedHands[i]));
            }

            sb.Append(String.Format("|rh||ah|Board {0}|sv|", boardNo));
            //It takes values of n, e, b, and - 
            switch (vulnerability)
            {
                case SysVulnerabilityEnum.All:
                    sb.Append("b|"); break;
                case SysVulnerabilityEnum.EW:
                    sb.Append("e|"); break;
                case SysVulnerabilityEnum.NS:
                    sb.Append("n|"); break;
                case SysVulnerabilityEnum.None:
                    sb.Append("-|"); break;
            }

            return sb.ToString();
        }

        public static string ExtractValue(string line)
        {
            var start = line.IndexOf('"');
            var end = line.LastIndexOf('"');

            return line.Substring(start + 1, end - start - 1);
        }

        public static Dictionary<string, int> ParseTableHeader(string header)
        {
            var parseResult = new Dictionary<string, int>();
            var columns = header.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            columns.ForEach(column =>
            {
                var regex = new Regex("[0-9]+");
                var columnMeta = column.Split(new[] { '\\' });
                var columnTitle = columnMeta[0];
                var valueLength = Int32.Parse(regex.Match(columnMeta[1]).Value);

                parseResult.Add(columnTitle, valueLength);
            });

            return parseResult;
        }

        public static Dictionary<string, string> ParseTableLine(string line, Dictionary<string, int> columnConfig)
        {
            var parseResult = new Dictionary<string, string>();
            var position = 0;
            columnConfig.ForEach(pair =>
            {
                var value = line.Substring(position, pair.Value).Trim().Replace("\"", string.Empty);
                position += pair.Value + 1;
                parseResult.Add(pair.Key, value);
            });

            return parseResult;
        }

        public static string ConvertToPbnBiddingSequence(Contract contract, int dealerId)
        {
            var declarerId = contract.PlayerPosition.ConvertToSysPlayer();

            if (dealerId <= 0 || declarerId <= 0)
                return string.Empty;

            var sb = new StringBuilder("|b");

            for (var i = dealerId; i < dealerId + 4; i++)
            {
                sb.Append("|mb");
                var currentPlayer = (i % 4);
                if (currentPlayer == 0)
                    currentPlayer = 4;
                if (currentPlayer == declarerId)
                {
                    sb.Append(string.Format("|{0}", contract.Notation()));
                    break;
                }
                sb.Append("|p");
            }

            if (contract.Doubled)
                sb.Append("|mb|x");
            if (contract.Redoubled)
                sb.Append("|mb|xx");

            sb.Append("|mb|p|mb|p|mb|p|");
            return sb.ToString();
        }
    }
}
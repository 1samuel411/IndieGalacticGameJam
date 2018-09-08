using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServerProj
{
    public class GameParent
    {

        public int round;
        public int funding;
        public Game game;

        public Game GenerateGame()
        {
            Game newGame = new Game();

            newGame.endTime = DateTime.UtcNow.AddSeconds(GetTargetTime()); 

            // Persist
            if (game != null)
            {
                newGame.attitude = game.attitude;
                newGame.cabinPressure = game.cabinPressure;
                newGame.speed = game.speed;
            }
            // Not Persist
            else
            {
                newGame.attitude = new Resource();
                newGame.attitude.name = "Attitude";
                newGame.attitude.symbol = GenerateSymbols(-90, 90, 5);
                // -90-90 degrees

                newGame.cabinPressure = new Resource();
                newGame.cabinPressure.name = "Cabin Pressure";
                newGame.attitude.symbol = GenerateSymbols(10, 20, 1);
                // 10 - 20 PSI

                newGame.speed = new Resource();
                newGame.speed.name = "Speed";
                newGame.attitude.symbol = GenerateSymbols(0, 1000, 100);
                // 0-1000 ft/s
            }

            newGame.alert = new Alert();
            int randResource = rand.Next(0, 2);
            if (randResource == 0)
            {
                newGame.alert.resource = newGame.attitude;
                newGame.alert.targetResourceValue = GetRandNumberInterval(-90, 90, 5);
            }
            else if (randResource == 1)
            {
                newGame.alert.resource = newGame.cabinPressure;
                newGame.alert.targetResourceValue = GetRandNumberInterval(10, 20, 1);
            }
            else if (randResource == 2)
            {
                newGame.alert.resource = newGame.speed;
                newGame.alert.targetResourceValue = GetRandNumberInterval(0, 1000, 100);
            }


            return newGame;
        }

        private Random rand = new Random();
        public int GetTargetTime()
        {
            int time = 0;
            switch(round)
            {
                case 0:
                    time = rand.Next(30, 35);
                    break;
                case 1:
                    time = rand.Next(29, 34);
                    break;
                case 2:
                    time = rand.Next(27, 33);
                    break;
                case 3:
                    time = rand.Next(25, 32);
                    break;
                case 4:
                    time = rand.Next(23, 31);
                    break;
                case 5:
                    time = rand.Next(22, 30);
                    break;
                case 6:
                    time = rand.Next(21, 27);
                    break;
                case 7:
                    time = rand.Next(20, 24);
                    break;
            }

            return time;
        }

        public List<Symbol> GenerateSymbols(int min, int max, int interval)
        {
            List<Symbol> symbols = new List<Symbol>();

            int symbolsToGenerate = GetRandNumberInterval(min, max, interval);

            string symbolsToGenerateStr = symbolsToGenerate.ToString();
            for(int i  = 0; i < symbolsToGenerateStr.Length; i++)
            {
                int x = int.Parse(symbolsToGenerateStr[i].ToString());
                Symbol symbol = new Symbol();
                symbol.unicodeValue = Symbol.characters[x].ToString();
                symbol.value = x;
                symbols.Add(symbol);
            }

            return symbols;
        }

        public int GetRandNumberInterval(int min, int max, int interval)
        {
            int symbolsToGen = rand.Next(min, max);

            // TODO: Add interval


            return symbolsToGen;
        }

    }
}

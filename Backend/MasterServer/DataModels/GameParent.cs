using MasterServerProj.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServerProj
{
    [Serializable]
    public class GameParent
    {

        public bool beatLast = false;
        public int round;
        public int funding;
        public bool ended;
        public Game game = new Game();
        public ControllerInput inputA;
        public ControllerInput inputB;

        public Game GenerateGame()
        {
            Game newGame = new Game();

            newGame.endTime = DateTime.UtcNow.AddSeconds(GetTargetTime());

            inputA = new ControllerInput();
            inputB = new ControllerInput();

            // Persist
            if (1 == 2)
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
                newGame.attitude.symbol = GenerateSymbols(0, 90, 5);
                // -90-90 degrees

                newGame.cabinPressure = new Resource();
                newGame.cabinPressure.name = "Cabin Pressure";
                newGame.cabinPressure.symbol = GenerateSymbols(10, 20, 1);
                // 10 - 20 PSI

                newGame.speed = new Resource();
                newGame.speed.name = "Speed";
                newGame.speed.symbol = GenerateSymbols(0, 1000, 100);
                // 0-1000 ft/s
            }

            newGame.alert = new Alert();
            int randResource = rand.Next(0, 3);
            if (randResource == 0)
            {
                newGame.alert.resource = newGame.attitude;
                newGame.alert.targetResourceValue = GetRandNumberInterval(0, 90, 5);
            }
            else if (randResource == 1)
            {
                newGame.alert.resource = newGame.cabinPressure;
                newGame.alert.targetResourceValue = GetRandNumberInterval(10, 20, 1);
            }
            else
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
                    time = rand.Next(19, 20);
                    break;
                case 1:
                    time = rand.Next(18, 19);
                    break;
                case 2:
                    time = rand.Next(17, 18);
                    break;
                case 3:
                    time = rand.Next(15, 18);
                    break;
                case 4:
                    time = rand.Next(13, 15);
                    break;
                case 5:
                    time = rand.Next(11, 13);
                    break;
                case 6:
                    time = rand.Next(10, 13);
                    break;
                case 7:
                    time = rand.Next(9, 13);
                    break;
            }

            return time;
        }

        public List<Symbol> GenerateSymbols(int min, int max, int interval)
        {
            List<Symbol> symbols = new List<Symbol>();

            int symbolsToGenerate = GetRandNumberInterval(min, max, interval);
            if (symbolsToGenerate < 0)
                symbolsToGenerate *= -1;
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
            int symbolsToGen = rand.Next(min / interval, max / interval) * interval;

            return symbolsToGen;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace SNetwork.Server
{
    public class CommandHandler
    {
        private static readonly List<Command> commands = new List<Command>();

        public static void RunCommand(string command, string commandText, Socket fromSocket, int id)
        {
            var foundCommand = commands.FirstOrDefault(x => x.commandName == command);
            if (foundCommand == null)
            {
                Console.WriteLine("[SNetworking] Invalid Request");
                Messaging.instance.SendInvalid(fromSocket, id);
                return;
            }
            foundCommand.callbackAction(commandText, fromSocket, id);
        }

        public static void RunCommand(string command, Socket fromSocket, int id)
        {
            if (command.StartsWith("!") || command.StartsWith("/"))
                command = command.Substring(1);
            command += " command is confirmed";
            var commandStrings = command.Split(' ');
            RunCommand(commandStrings[0], commandStrings[1], fromSocket, id);
        }

        public static void AddCommand(Command command)
        {
            commands.Add(command);
        }
    }

    public class Command
    {
        public Action<string, Socket, int> callbackAction;
        public string commandName;

        public Command(string commandName, Action<string, Socket, int> callbackAction)
        {
            this.callbackAction = callbackAction;
            this.commandName = commandName;
        }
    }
}
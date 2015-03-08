using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class Terminal
    {
        public static bool CommandYN(string message, params string[] messageParts)
        {
            return CommandYN(String.Format(message, messageParts));
        }
        public static bool CommandYN(string message)
        {
            Console.WriteLine(message);
            ConsoleKeyInfo cmd = Console.ReadKey(true);

            switch (cmd.KeyChar)
            {
                case 'y':
                case '\r':
                    return true;
                case 'n':
                case (char)27:
                    return false;
                default:
                    Console.WriteLine("Available options (case insensitive): y n");
                    return CommandYN(message);
            }
        }
        public static char Command(string message, char[] options)
        {
            Console.WriteLine(message);
            ConsoleKeyInfo cmd = Console.ReadKey(true);
            char c = Char.ToLower(cmd.KeyChar);

            for (int i = 0; i < options.Length; i++)
                if (c == options[i])
                    return c;

            Console.WriteLine("Available options (case insensitive): {0}", options.ToString());
            return Command(message, options);
        }

        public static string Input(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Util
{
    public class utils
    {
        public static string banner = @"
                                                          
                                                          
              .--.--.    ,--, ____                 
             /  /    '.,--.'|    ,--,          ,'  , `.               
            |  :  /`. /|  | :  ,--.'|       ,-+-,.' _ |
            ;  |  | --` :  : '  |  |,     ,-+-. ;   , ||               
             |  :  ; _ | ' |  `--'_    ,--.'|' |  ||,---.
             \  \    `.'  | |  ,' ,'|  |   |  ,', |  |/     \         
              `----.   |  | :  '  | |  |   | /  | |--/    /  |        
              __ \  \  '  : |__|  | :  |   : |  | , .    ' / |
             /  /`--'  |  | '.''  : | __ |   : |  |/ '   ;   /|        
            '--'.     /;  :    |  | '.' |   | |`-'   ' |  / |
              `--'---' |  ,   /;  :    |   ;/       |   :    |
                        ---`-' |  ,   /'-- - '         \   \  /         
                                -- -`-'                `----'
                    ~~ free swapper
                             by joshua 
                                    (@ulzi) ~~

        ";
        public static bool init()
        {
            string message = "Slatt! Are you ready!??";
            string title = "Slimy Swapper";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            return (result == DialogResult.Yes);
        }

        public static void success(string target, int attempts)
        {
            string message = $"Successfully Swapped : @{target} | Attempts: {attempts}";
            string title = "Slimy Swapper";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);

        }

        public static void fail(string target, int attempts)
        {
            string message = $"Rate Limit Reached! : @{target} | Attempts: {attempts}";
            string title = "Slimy Swapper";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);

        }
        public static string input(string arg)
        {
            Console.Write(arg);
            return Console.ReadLine();
        }
        public static string mask(string arg)
        {
            Console.Write(arg);
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                ConsoleKeyInfo keyInfo;
                keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.Write("\n");
            return pass;
        }
        
    }
}

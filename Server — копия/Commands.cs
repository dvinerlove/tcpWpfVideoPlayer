using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;

namespace Server
{
    class Commands
    {
        
        public Commands(SimpleTcpServer server)
        {
             
        }


        public string ReadCommand(string command)
        {
            string[] commandName = command.Split('|');
            switch (commandName[0])
            {
                case "Log":
                    return CheckLogin(commandName[1]);
                    break;
                default:
                    return "";
                    break;
            }
        }
        private string CheckLogin(string command)
        {
            string username = command.Split(' ')[0];
            string password = command.Split(' ')[1];
            return("LiginCgeck|true");
        }
        private void Send(string query)
        {

            
        }

    }
}

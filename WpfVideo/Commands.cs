
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVideo
{
    class Commands
    {
        string query; 
        public Commands( )
        {
             
        }
        public string ReadCommand(string command, string username)
        {
            string[] commandName = command.Split(' ');
            switch (commandName[0])
            {
                case "/set":
                        return "SET|"+ commandName[1] +" подрубил " + commandName[2]+"\n";
                default:
                    return "";


            }
        }
        public string GetUsers(string command)
        {
            if (command.Split('|')[0] == "IMH")
                return command.Split('|')[1];
            else return "";
        }
        public void Register(string username, string password)
        {
            if (username != "" && password != "")
            {
                query = "Reg|username password";
                //client.WriteLineAndGetReply(query, TimeSpan.Zero);
            }
        }
        public string Login(string username)
        {
            if (username != "")
            {
                return "MSG|"+username + ": подлупился.";
                //client.WriteLineAndGetReply(query, TimeSpan.Zero);
            }
            else return "";
        }
    }
}

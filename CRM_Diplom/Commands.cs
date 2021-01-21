using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_Diplom
{
    class Commands
    {
         SimpleTcpClient client;
        string query;
        public Commands(SimpleTcpClient client)
        {
            this.client = client;
        }
        public bool ReadCommand(string command)
        {
            string[] commandName = command.Split('|');
            switch (commandName[0])
            {
                case "LiginCgeck":
                    if (commandName[1]=="true")
                    {
                        return true;
                    }else return false;
                default:
                    return false;
                  

            }
        }
        public void Register(string username, string password)
        {
            if (username!=""&&password!="")
            {
                query = "Reg|username password";
                client.WriteLineAndGetReply(query, TimeSpan.Zero);
            }
        }
        public string Login(string username)
        {
            if (username != "")
            {
                return username + ": подулючился.";
                //client.WriteLineAndGetReply(query, TimeSpan.Zero);
            }
            else return "";
        }
    }
}

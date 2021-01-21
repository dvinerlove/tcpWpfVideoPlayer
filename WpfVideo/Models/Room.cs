using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoPlayer.Models
{
    public class Room
    {
        string id;
        string name;
        public string selectedVideo;
        public string isPlaying;
        public string speed;
        string position;
        public Room()
        {
            selectedVideo = "";
            isPlaying = "False";
            this.speed = "1";
        }
        public Room(string sv, string ip, string speed)
        {
            selectedVideo = sv;
            isPlaying = ip;
            this.speed = speed;
        }
        public string GetRoom()
        {
            if (selectedVideo != "")
            {

                return "ROOM|" + selectedVideo + "|" + isPlaying + "|" + speed + "|";
            }
            return "";

        }
        public void SetRoom(string room)
        {
            room = room.Replace("ROOM|", "");
            string[] vs = room.Split('|');
            if (vs[0] != "")
            {
                selectedVideo = vs[0];
                isPlaying = vs[1];
                speed = vs[2];
            }
        }
    }
}

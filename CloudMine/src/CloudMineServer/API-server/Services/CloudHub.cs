using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;

namespace CloudMineServer.Services
{
    //[EnableCors("AllowAnyOrigin")]
    public class CloudHub : Hub
    {
        private Uri url;
        private string connectedUser;

        public void DownloadFile(string connectedUser, string fileiD)
        {

        }
        //public static List<string> ConnectedUsers;

        //public void Send(string originatorUser, string message)
        //{
        //    Clients.All.messageReceived(originatorUser, message);
        //}

        //public void Connect(string newUser)
        //{
        //    if (ConnectedUsers == null)
        //        ConnectedUsers = new List<string>();

        //    ConnectedUsers.Add(newUser);
        //    Clients.Caller.getConnectedUsers(ConnectedUsers);
        //    Clients.Others.newUserAdded(newUser);
        //}


    }
}

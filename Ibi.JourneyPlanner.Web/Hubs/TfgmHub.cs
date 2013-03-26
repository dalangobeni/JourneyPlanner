using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Hubs
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR;

    public class Tfgm : Hub
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();

        public void Heartbeat()
        {
            // Call the addMessage method on all clients            
            Clients.Caller.addMessage("Connected");
        }

        private void CheckForUpdates()
        {
            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(
                waitHandle,

                // Method to execute
                (state, timeout) =>
                    {
                        // TODO: implement the functionality you want to be executed
                        // on every 5 seconds here
                        // Important Remark: This method runs on a worker thread drawn 
                        // from the thread pool which is also used to service requests
                        // so make sure that this method returns as fast as possible or
                        // you will be jeopardizing worker threads which could be catastrophic 
                        // in a web application. Make sure you don't sleep here and if you were
                        // to perform some I/O intensive operation make sure you use asynchronous
                        // API and IO completion ports for increased scalability
                    },
                
                    // optional state object to pass to the method
                null,
                
                // Execute the method after 30 seconds
                TimeSpan.FromSeconds(30),

                // Set this to false to execute it repeatedly every 5 seconds
                false);
        }

        public override Task OnConnected()
        {
            ConnectedIds.Add(Context.ConnectionId);
            Clients.All.addMessage("Total connections " + ConnectedIds.Count());
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.addMessage("Total connections " + ConnectedIds.Count());
            return base.OnDisconnected();
        }
    }
}
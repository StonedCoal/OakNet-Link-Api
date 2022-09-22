﻿using OakNetLink.Api.Communication;
using OakNetLink.Api.Packets;
using OakNetLink.Sessions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OakNetLink.Api;

namespace OakNetLink.Sessions
{
    public class Sessions : ONLPlugin
    {
        public Sessions() : base(1)
        {
            ONL.Event.Disconnection += (sender, args) => {
                if (args is ONL.Event.DisconnectEventArgs disargs)
                    SessionManager.EndPointDisconnected(disargs.endpoint);
            };
        }


        public override Dictionary<Type, Type> registerPackets()
        {
            var result = new Dictionary<Type, Type>();
            result.Add(typeof(SessionCreatePacket), typeof(SessionCreatePacketProcessor));
            result.Add(typeof(SessionCreateResponsePacket), typeof(SessionCreateResponsePacketProcessor));
            result.Add(typeof(SessionFetchListPacket), typeof(SessionFetchListPacketProcessor));
            result.Add(typeof(SessionListResponsePacket), typeof(SessionListResponsePacketProcessor));
            result.Add(typeof(SessionJoinRequestPacket), typeof(SessionJoinRequestPacketProcessor));
            result.Add(typeof(SessionJoinRequestResponsePacket), typeof(SessionJoinRequestResponsePacketProcessor));
            result.Add(typeof(SessionMemberConnectedPacket), typeof(SessionMemberConnectedPacketProcessor));
            return result;
        }


        public static class Event
        {
            /// <summary>
            /// This EventHandler will be called when the requested session creation process was successfull
            /// </summary>
            public static EventHandler? SessionCreationSuccess { get; set; }

            /// <summary>
            /// This EventHandler will be called when the requested session creation failed
            /// The passed object is the the errorMessage
            /// </summary>
            public static EventHandler? SessionCreationFailed { get; set; }

            /// <summary>
            /// This EventHandler will be called when the session list has been updated
            /// </summary>
            public static EventHandler? SessionListUpdated { get; set; }

            /// <summary>
            /// This EventHandler will be called when the session join request has been denied
            /// The passed object is the message as string
            /// </summary>
            public static EventHandler? SessionJoinDenied { get; set; }

            /// <summary>
            /// This EventHandler will be called when the session join was successfull
            /// </summary>
            public static EventHandler? SessionJoinSuccess { get; set; }
        }

        public static List<Session> AvailableSessions()
        {
            return SessionManager.AvailableSessions;
        }

        public static void FetchSessions()
        {
            SessionManager.FetchSessions();
        }

        public static void CreateNewSession(string name, string password)
        {
            var createSessionPacket = new SessionCreatePacket();
            createSessionPacket.SessionName = name.Replace(";", "_");
            createSessionPacket.SessionPassword = password;
            Communicator.instance.sendPacket(PacketProcessor.encodePacket(createSessionPacket), ONL.MasterServer.EndPoint, false, true, false);
        }

        public static void JoinSession(string name, string password)
        {
            var sessionJoinRequestPacket = new SessionJoinRequestPacket();
            sessionJoinRequestPacket.SessionName = name.Replace(";", "_");
            sessionJoinRequestPacket.SessionPassword = password;
            Communicator.instance.sendPacket(PacketProcessor.encodePacket(sessionJoinRequestPacket), ONL.MasterServer.EndPoint, false, true, false);
        }

        /// <summary>
        /// Use this function to send a broadcast to all endpoints in the session, except yourself
        /// </summary>
        public static void SendBroadcast(OakNetLink.Api.Packets.Packet packet, bool reliable)
        {
            foreach (var endpoint in OakNetEndPointManager.ConnectedEndpoints())
            {
                Communicator.instance.sendPacket(PacketProcessor.encodePacket(packet), endpoint, true, reliable, false);
            }
        }

        /// <summary>
        /// Use this function to send a packet to a specific endpoint in the session
        /// </summary>
        public static void SendPacket(OakNetLink.Api.Packets.Packet packet, bool reliable, OakNetEndPoint receiver)
        {
            if (!OakNetEndPointManager.ConnectedEndpoints().Contains(receiver))
                return;
            Communicator.instance.sendPacket(PacketProcessor.encodePacket(packet), receiver, false, reliable, false);
        }

    }
}
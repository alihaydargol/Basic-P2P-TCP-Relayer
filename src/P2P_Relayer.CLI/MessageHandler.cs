﻿using LiteNetLib;
using P2P_Relayer.Common;
using System;

namespace P2P_Relayer.CLI
{
    internal class MessageHandler
    {
        internal static void Handle(Client client, NetPacketReader reader)
        {
            if (!reader.TryGetByte(out byte rawOpcode))
            {
                Console.WriteLine("No Opcode in message.");
                return;
            }

            if (!Enum.IsDefined(typeof(Opcodes), rawOpcode))
            {
                Console.WriteLine("Raw Opcode is not defined in enum.");
                return;
            }

            switch ((Opcodes)rawOpcode)
            {
                case Opcodes.ActivateAck:
                    HandleActivateAck(client, reader);
                    break;

                case Opcodes.EventConnect:
                    {
                        if (reader.TryGetInt(out var id))
                            client.Rift.Connect(id);
                    }
                    break;
                case Opcodes.EventDisconnect:
                    {
                        if (reader.TryGetInt(out var id))
                            client.Rift.Disconnect(id);
                    }
                    break;
                case Opcodes.EventData:
                    {
                        if (reader.TryGetInt(out var id))
                            if (reader.TryGetBytesWithLength(out var data))
                                client.Rift.Send(id, data);
                    }
                    break;
            }
        }

        private static void HandleActivateAck(Client client, NetPacketReader reader)
        {
            if (!reader.TryGetString(out var token))
            {
                Console.WriteLine("No token in HandleActivateAck.");
                return;
            }

            client.NatPunchModule.SendNatIntroduceRequest(null, token);
        }
    }
}

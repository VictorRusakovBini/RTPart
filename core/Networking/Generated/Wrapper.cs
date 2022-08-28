using System;
using System.Collections.Generic;
using Google.Protobuf;
namespace core.Networking.Generated { 
    public enum PacketsIds {
        PckConnect = 0,
        PckConnectResult = 1,
        PckSendStat1 = 2,
        PckSendStat2 = 3,
        PckSendStat3 = 4,
        PckSendStat4 = 5,
    }
    public static class PacketsWrapper {
        private static readonly Dictionary<short, byte[]> ConversionCache = new Dictionary<short, byte[]>();
        static PacketsWrapper() {
            for (short i = 0; i < short.MaxValue; i++) {
                ConversionCache.Add(i, BitConverter.GetBytes(i));
            }
        }
        public static byte[] GetPacketIdBytes(IMessage m) {
            return ConversionCache[GetPacketId(m)];
        }
        public static byte[] GetPacketLengthBytes(short len) {
            return ConversionCache[len];
        }
        public static short GetPacketId(IMessage m) { 
            return m switch {
            PckConnect _ =>  (short)PacketsIds.PckConnect,
            PckConnectResult _ =>  (short)PacketsIds.PckConnectResult,
            PckSendStat1 _ =>  (short)PacketsIds.PckSendStat1,
            PckSendStat2 _ =>  (short)PacketsIds.PckSendStat2,
            PckSendStat3 _ =>  (short)PacketsIds.PckSendStat3,
            PckSendStat4 _ =>  (short)PacketsIds.PckSendStat4,
            _ => -1
            };
        }
        public static MessageParser GetPacketType(PacketsIds id){ 
            return id switch { 
               PacketsIds.PckConnect => PckConnect.Parser,
               PacketsIds.PckConnectResult => PckConnectResult.Parser,
               PacketsIds.PckSendStat1 => PckSendStat1.Parser,
               PacketsIds.PckSendStat2 => PckSendStat2.Parser,
               PacketsIds.PckSendStat3 => PckSendStat3.Parser,
               PacketsIds.PckSendStat4 => PckSendStat4.Parser,
             _ => null
            };
        }
    }
}

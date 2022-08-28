using System.Diagnostics;
using System.Text;

namespace builder;

public class WrapperBuilder
{
    public WrapperBuilder(List<Type> types)
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using Google.Protobuf;");
        
        builder.AppendLine("namespace core.Networking.Generated { ");
        
        builder.AppendLine("    public enum PacketsIds {");
        for (var i = 0; i < types.Count; i++)
        {
            builder.AppendLine($"        {types[i].Name} = {i},");
        }

        builder.AppendLine("    }");


        builder.AppendLine("    public static class PacketsWrapper {");


        builder.AppendLine("        private static readonly Dictionary<short, byte[]> ConversionCache = new Dictionary<short, byte[]>();");
        builder.AppendLine("        static PacketsWrapper() {");
        builder.AppendLine("            for (short i = 0; i < short.MaxValue; i++) {");
        builder.AppendLine("                ConversionCache.Add(i, BitConverter.GetBytes(i));");
        builder.AppendLine("            }");
        builder.AppendLine("        }");

        builder.AppendLine("        public static byte[] GetPacketIdBytes(IMessage m) {");
        builder.AppendLine("            return ConversionCache[GetPacketId(m)];");
        builder.AppendLine("        }");

        builder.AppendLine("        public static byte[] GetPacketLengthBytes(short len) {");
        builder.AppendLine("            return ConversionCache[len];");
        builder.AppendLine("        }");
        
        builder.AppendLine("        public static short GetPacketId(IMessage m) { ");
        builder.AppendLine("            return m switch {");
        
        foreach (var t in types)
        {
            builder.AppendLine($"            {t.Name} _ =>  (short)PacketsIds.{t.Name},");
        }

        builder.AppendLine("            _ => -1");
        builder.AppendLine("            };");
        builder.AppendLine("        }");
        
        builder.AppendLine("        public static MessageParser GetPacketType(PacketsIds id){ ");
        builder.AppendLine("            return id switch { ");
        
        foreach (var t in types)
        {
            builder.AppendLine($"               PacketsIds.{t.Name} => {t.Name}.Parser,");
        }
        builder.AppendLine("             _ => null");
        builder.AppendLine("            };");
        builder.AppendLine("        }");
        
        builder.AppendLine("    }");
        
        builder.AppendLine("}");

        var current = Directory.GetCurrentDirectory();
        var proj = Directory.GetParent(current)?.Parent?.Parent?.FullName;
        if (string.IsNullOrEmpty(proj))
        {
            Console.WriteLine("Cant' find project folder, process terminate!");
            return;
        }

        var wrapperPath = $"{proj}/Models/Wrapper.cs";
        
        if (File.Exists(wrapperPath))
        {
            File.Delete(wrapperPath);
        }
        
        File.WriteAllText(wrapperPath,builder.ToString());

        var srv = Directory.GetParent(current)?.Parent?.Parent?.Parent?.FullName;
        if (string.IsNullOrEmpty(proj))
        {
            Console.WriteLine("Cant' find target folder, process terminate!");
            return;
        }
        
        srv += "/core";
        File.Copy($"{proj}/Models/Packets.cs", $"{srv}/Networking/Generated/Packets.cs",true);
        File.Copy($"{proj}/Models/Wrapper.cs", $"{srv}/Networking/Generated/Wrapper.cs",true);


        var root = Directory.GetParent(current)?.Parent?.Parent?.Parent?.Parent?.FullName;
        if (string.IsNullOrEmpty(root))
        {
            return;
        }

        root += "/RtProject/Assets/Scripts";
        Console.WriteLine(root);
        
       File.Copy($"{proj}/Models/Packets.cs", $"{root}/Networking/Generated/Packets.cs",true);
       File.Copy($"{proj}/Models/Wrapper.cs", $"{root}/Networking/Generated/Wrapper.cs",true);
    }
}
using System.Reflection;
using builder;

const string packetsNameSpace = "core.Networking.Generated";

var packets = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.Namespace == packetsNameSpace && t.Name.StartsWith("Pck"))
    .ToList();

var wrapperBuilder = new WrapperBuilder(packets);
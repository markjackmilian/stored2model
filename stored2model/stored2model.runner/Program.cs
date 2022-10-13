using System.Runtime.CompilerServices;
using stored2model.core;

if (args.Length == 0 || args[0] == "-h" || args.Length <3)
{
    Console.WriteLine("arg0: connectionString");    
    Console.WriteLine("arg1: storedProcedure name");    
    Console.WriteLine("arg2: outputFileName");    
    Console.WriteLine("arg3[optional]: bool use nullable - default is false");    
    return;
}

var connectionString = args[0];
var storedName =  args[1];
var outPutFile = args[2];
var useNullable = args.Length > 3 && bool.Parse(args[3]);

var class1 = new Worker(connectionString, storedName,outPutFile,useNullable);
class1.Do();

Console.WriteLine($"{outPutFile} written!");

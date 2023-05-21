using System;
using RVEmuSharp;

var shellcode = new List<byte>();

//add x1,x2,x3 
//sub x1,x2,x3
//addi x1,x2,3

shellcode.AddRange(BitConverter.GetBytes((UInt32)0x003100b3));
shellcode.AddRange(BitConverter.GetBytes((UInt32)0x403100b3));
shellcode.AddRange(BitConverter.GetBytes((UInt32)0x00310093));


var emulator = new RVEmulator();

emulator.LoadProgram(shellcode.ToArray());

for (int i = 0; i < shellcode.Count / sizeof(UInt32); i++)
{
    var executed = emulator.RunOnce();
    Console.WriteLine($"Executed: {executed}");
}

Console.WriteLine($"Register 1: {emulator.GetRegister(0)}");

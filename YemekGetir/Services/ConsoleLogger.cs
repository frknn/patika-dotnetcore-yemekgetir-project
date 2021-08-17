using System;

namespace YemekGetir.Services
{
  public class ConsoleLogger : ILoggerService
  {
    public void Write(string message)
    {
      Console.WriteLine("[CL] - " + message);
    }
  }
}
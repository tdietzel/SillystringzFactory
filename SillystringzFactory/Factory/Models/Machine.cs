using System.Collections.Generic;

namespace Factory.Models
{
  public class Machine
  {
    public int MachineId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<EngineerMachine> JoinEntities { get; set; }
  }
}
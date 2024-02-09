using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Factory.Models;

namespace Factory.Controllers
{
  public class MachineController : Controller
  {
    private readonly FactoryContext _db;
    public MachineController(FactoryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List <Machine> model = _db.Machines.ToList();

      return View(model);
    }

    [HttpPost]
    public ActionResult Create(Machine machine)
    {
      _db.Machines.Add(machine);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }
    public ActionResult Create()
    {
      return View();
    }
    public ActionResult Detail(int id)
    {
      Machine selectedMachine = _db.Machines
        .Include(m => m.JoinEntities)
        .ThenInclude(join => join.Engineer)
      .FirstOrDefault(m => m.MachineId == id);

      return View(selectedMachine);
    }

    [HttpPost]
    public ActionResult AddEngineer(Machine machine, int engineerId)
    {
      #nullable enable
      EngineerMachine? joinEntity = _db.EngineerMachines.FirstOrDefault(j => j.EngineerId == engineerId && j.MachineId == machine.MachineId);
      #nullable disable

      if (joinEntity == null && engineerId != 0)
      {
        _db.EngineerMachines.Add(new EngineerMachine() { EngineerId = engineerId, MachineId = machine.MachineId });
        _db.SaveChanges();
      }

      return RedirectToAction("Detail", new { id = machine.MachineId });
    }
    public ActionResult AddEngineer(int id)
    {
      Machine selectedMachine = _db.Machines.Find(id);
      ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");

      return View(selectedMachine);
    }
  }
}
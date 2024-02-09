using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Factory.Models;

namespace Factory.Controllers
{
  public class EngineerController : Controller
  {
    private readonly FactoryContext _db;
    public EngineerController(FactoryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Engineer> model = _db.Engineers.ToList();
      return View(model);
    }

    [HttpPost]
    public ActionResult Create(Engineer engineer)
    {
      _db.Engineers.Add(engineer);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }
    public ActionResult Create()
    {
      return View();
    }

    public ActionResult Detail(int id)
    {
      Engineer selectedEngineer = _db.Engineers
        .Include(e => e.JoinEntities)
        .ThenInclude(join => join.Machine)
        .FirstOrDefault(e => e.EngineerId == id);
      return View(selectedEngineer);
    }

    [HttpPost]
    public ActionResult AddMachine(Engineer engineer, int machineId)
    {
      #nullable enable
      EngineerMachine? joinEntity = _db.EngineerMachines.FirstOrDefault(j => j.MachineId == machineId && j.EngineerId == engineer.EngineerId);
      #nullable disable

      if (joinEntity == null && machineId != 0)
      {
        _db.EngineerMachines.Add(new EngineerMachine() { MachineId = machineId, EngineerId = engineer.EngineerId });
        _db.SaveChanges();
      }

      return RedirectToAction("Detail", new { id = engineer.EngineerId });
    }

    public ActionResult AddMachine(int id)
    {
      Engineer selectedEngineer = _db.Engineers.FirstOrDefault(e => e.EngineerId == id);
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
      return View(selectedEngineer);
    }
  }
}
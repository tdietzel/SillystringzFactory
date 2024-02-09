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
      return View(_db.Machines.ToList());
    }

    public ActionResult Create()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Create(Machine machine)
    {
      _db.Machines.Add(machine);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    public ActionResult Detail(int id)
    {
      Machine selectedMachine = _db.Machines
        .Include(m => m.JoinEntities)
        .ThenInclude(join => join.Engineer)
      .FirstOrDefault(m => m.MachineId == id);

      return View(selectedMachine);
    }

    public ActionResult Edit(int machineId)
    {
      return View(_db.Machines.Find(machineId));
    }
    [HttpPost]
    public ActionResult Edit(Machine machine)
    {
      _db.Machines.Update(machine);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult Delete(int machineId)
    {
      Machine selectedMachine = _db.Machines.Find(machineId);
      _db.Machines.Remove(selectedMachine);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    public ActionResult AddEngineer(int id)
    {
      ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");

      return View(_db.Machines.Find(id));
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

    [HttpPost]
    public ActionResult DeleteEngineer(int joinId)
    {
      EngineerMachine joinEntry = _db.EngineerMachines.Find(joinId);
      _db.EngineerMachines.Remove(joinEntry);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }
  }
}
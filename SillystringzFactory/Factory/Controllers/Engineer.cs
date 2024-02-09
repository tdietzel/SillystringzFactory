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
      return View(_db.Engineers.ToList());
    }

    public ActionResult Create()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Create(Engineer engineer)
    {
      _db.Engineers.Add(engineer);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    public ActionResult Detail(int id)
    {
      Engineer selectedEngineer = _db.Engineers
        .Include(e => e.JoinEntities)
        .ThenInclude(join => join.Machine)
      .FirstOrDefault(e => e.EngineerId == id);

      return View(selectedEngineer);
    }

    public ActionResult Edit(int engineerId)
    {
      return View(_db.Engineers.Find(engineerId));
    }
    [HttpPost]
    public ActionResult Edit(Engineer engineer)
    {
      _db.Engineers.Update(engineer);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult Delete(int engineerId)
    {
      Engineer selectedEngineer = _db.Engineers.Find(engineerId);
      _db.Engineers.Remove(selectedEngineer);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }

    public ActionResult AddMachine(int id)
    {
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");

      return View(_db.Engineers.Find(id));
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

    [HttpPost]
    public ActionResult DeleteMachine(int joinId)
    {
      EngineerMachine joinEntry = _db.EngineerMachines.Find(joinId);
      _db.EngineerMachines.Remove(joinEntry);
      _db.SaveChanges();

      return RedirectToAction("Index");
    }
  }
}
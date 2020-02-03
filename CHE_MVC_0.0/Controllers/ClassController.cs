using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CHE_MVC_0._0.Models;

namespace CHE_MVC_0._0.Controllers

/*
 * Web GUI Controller
 *      
 */
{
    public class ClassController : Controller
    {
        private hmaha1DBContextGUI db = new hmaha1DBContextGUI();

        // GET: Class
        public ActionResult Index()
        {
            return View(db.GUIClasses.ToList());
        }

        // GET: Class/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.GUIClass guiClass = db.GUIClasses.Find(id);
            if (guiClass == null)
            {
                return HttpNotFound();
            }
            return View(guiClass);
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Class/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cid,name,pid,isAbstract")] Models.GUIClass guiClass)
        {
            // Server-side validation
            // Check CID != PID
            if (guiClass.cid == guiClass.pid)
            {
                ModelState.AddModelError("cid", "Class ID cannot be the same as Parent ID!");
            }

            // Checking if Parent Exists
            if (!db.GUIClasses.Any(item => item.cid == guiClass.pid))
            {
                ModelState.AddModelError("pid", "Parent class does not exist.");
            }

            // Server-side validation
            // Checking if CID is not used.
            if (db.GUIClasses.Any(item => item.cid == guiClass.cid))
            {
                ModelState.AddModelError("cid", "Class ID already in use.");
            }

            if (ModelState.IsValid)
            {
                if (guiClass.pid == null) guiClass.pid = 0;

                db.GUIClasses.Add(guiClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(guiClass);
        }

        // GET: Class/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GUIClass guiClass = db.GUIClasses.Find(id);
            if (guiClass == null)
            {
                return HttpNotFound();
            }
            return View(guiClass);
        }

        // POST: Class/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cid,name,pid,isAbstract")] GUIClass guiClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guiClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guiClass);
        }

        // GET: Class/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            GUIClass guiClass = db.GUIClasses.Find(id);
            if (guiClass == null)
            {
                return HttpNotFound();
            }

            
            string name = guiClass.name;

            // checking if target cid is listed as a parent ID for any other classes
            if (db.GUIClasses.Any(item => item.pid == id) == true)
            {
                // Create an error object 
                ErrorHandling eh = new ErrorHandling
                {
                    ErrorNo = 1,
                    TargetClassName = name
                };

                // Pass error object to Error view
                return RedirectToAction("Error", eh);
            }

            // Pass target class to Delete View()
            return View(guiClass);
        }

        // POST: Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GUIClass guiClass = db.GUIClasses.Find(id);
            db.GUIClasses.Remove(guiClass);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //public bool IsCidAvailable(int cid) // will return True if match was found
        public JsonResult IsCidAvailable(int cid) // Cangng the result to JSON as the validation is on client-side
        {
            /* Will check if CID already exists in DB
             * 
             * Negated: cus validation must fail is cid already exists
             * Json(): wraps the return (in this case bool) value into a json object;
             * JsonRequestBehavior.AllowGet: allows this method to be called as a GET request;
             */
            return Json(!db.GUIClasses.Any(item => item.cid == cid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsParentAvailable(int pid) 
        {
            /* 
             * Will check if parent id exists in DB
             */
            return Json(db.GUIClasses.Any(item => item.cid == pid), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult IsNameAvailable(string name)
        {
            // Will check if name already exists in DB

            return Json(!db.GUIClasses.Any(item => item.name == name), JsonRequestBehavior.AllowGet);
        }

        public bool IsAParent(int? cid) // will return true if current class ID is listed as a PID for any other class
        {
            return db.GUIClasses.Any(item => item.pid == cid);
        }


        public ActionResult IsParentError()
        {
            return View();
        }

        public ActionResult Error(ErrorHandling eh)
        {
            return View(eh);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private SummerSchoolMVCEntities db = new SummerSchoolMVCEntities();

        // GET: Students
        public ActionResult Index(string searchString)
        { // look in index/ student 
            // select ALL students
            //so we can use it whether or not we are filtering 
            var students = from item in db.Students
                           select item;

            // filtering students bases on search
            if (!String.IsNullOrEmpty(searchString))
            {
                 students = from item in students
                               where item.LastName.Contains(searchString) ||
                                     item.FirstName.Contains(searchString)
                               select item;
            }

            ViewBag.TotalEnrollmentFee = TotalFees();
            // setting max enrollment 
            ViewBag.MaximumEnrollment = 15;
            return View(students);
        }
      


        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }


        int EnrollStudent(string firstName, string lastName)
        {
            double cost = 200;


            // special case harry potter 
            if (lastName.ToLower() == "potter")
            {
                cost *= 0.5;
            }
            int numberOfStudents = db.Students.Count();
            // special case longbottom
            // SELECT COUNT (*) FROM Students; when using COUNT()
            if (lastName.ToLower() == "longbottom" && numberOfStudents <= 10)
            {
                cost = 0;
            }
            // special case first initial same as last initial
            if (firstName.ToLower()[0] == lastName.ToLower()[0])
            {
                cost = 0.9 * cost;
            }
            return (int)cost;
        }


        //calculate enrollment fee
        public decimal TotalFees()
        {
            ViewBag.Message = "Not allowed";
            decimal runningTotal = 0;

            foreach (Student student in db.Students)
            {
                runningTotal = runningTotal + student.EnrollmentFee;
            }
            return runningTotal;
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentID,FirstName,LastName,EnrollmentFee")] Student student)
        {

            // special case tom/riddle/voldemort
            if (student.LastName.ToLower() == "tom" ||
             student.FirstName.ToLower() == "tom" ||
             student.LastName.ToLower() == "riddle" ||
             student.FirstName.ToLower() == "riddle" ||
             student.FirstName.ToLower() == "voldemort" ||
             student.LastName.ToLower() == "voldemort")
            {
                return View("voldemort");
            }

            // special case malfoy
            if (student.LastName.ToLower() == "malfoy")
            {
                return View("malfoy");
            }

            student.EnrollmentFee = EnrollStudent(student.FirstName, student.LastName);
            if (ModelState.IsValid)
            {                
                db.Students.Add(student);
                db.SaveChanges();                
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }
    

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,FirstName,LastName,EnrollmentFee")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
    }
}

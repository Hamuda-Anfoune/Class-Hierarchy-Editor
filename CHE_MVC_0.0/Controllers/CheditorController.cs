using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CHE_MVC_0._0.Beans;
//using System.Web.Mvc;
using CHE_MVC_0._0.Models;
using Newtonsoft.Json;

namespace CHE_MVC_0._0.Controllers
{
    /*
     *   API CONTROLLER
     */

    public class CheditorController : ApiController
    {
        private hmaha1DBContextAPI db = new hmaha1DBContextAPI();
        CHEditor che = new CHEditor();
        
        /*
         * Task 1.a.1: AddClass
         * 
         *  GET => /cheditor/api/addclass?cid=1&name=Vehicle&abstract=true&pid=0
         *  
         *  Done!
         */

        [HttpGet]
        [ResponseType(typeof(APIClass))]
        public IHttpActionResult AddClass(int? cid, string name, bool Abstract, int? pid = null)
        // int? = nullable int, pid = null: makes it possible to not add pid in url
        {

            // setting as first level class
            if (pid == null) pid = 0;

            // Check CID != PID
            if (cid == pid)
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Class ID cannot be the same as Parent ID"
                };
                return Json(jro);
            }

            // Checking if CID and Name are not Nulls
            if (cid == null | name == null)
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Class ID and/or Name cannot be null"
                };
                return Json(jro);
            }

            // Checking if name is available
            if (!che.IsNameAvailable(name))
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Class name already exists"
                };
                return Json(jro);
            }

            // Checking if pid exists
            if (!che.IsParentAvailable((int)pid))
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Parent class does not exist"
                };
                return Json(jro);
            }

            // Adding request values to a class object
            APIClass AddClass = new APIClass
            {
                cid = (int)cid,
                name = name,
                isAbstract = Abstract,
                pid = pid
            };

            // Adding created obj to DBContext
            db.APIClasses.Add(AddClass);

            try
            {
                // Commiting changes to DB
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Checking if cid is available
                // Dublicate CID will invoke DbUpdateException as CID is a primary key
                if (!che.IsCidAvailable((int)cid))
                {
                    JsonResponseObject jro = new JsonResponseObject
                    {
                        result = false,
                        message = "Class ID already exists"
                    };
                    return Json(jro);
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = AddClass.cid }, AddClass);
        }

        /*
         * Task 1.a.b: AddClassJson  |  5 Marks
         * 
         *  POST => /cheditor/api/addClassJSON
         *          {"cid": "1","name": "Vehicle","abstract": "true"}
         * 
         *  TODO:
         *    
         *  DONE!
         *  
         */

        //[ResponseType(typeof(APIClass))]
        [HttpPost]
        [Route("cheditor/api/addclassjson")] // Need this because passed variable was changed from id to cid
        public IHttpActionResult AddClassJson(APIClass jsonReq)
        {
            //string jsonReq = "{ \"cid\": \"1\",\"name\": \"Vehicle\",\"abstract\": \"true\"}";

            // Deserialisong Json object into a class model object
            //APIClass classReq = JsonConvert.DeserializeObject<APIClass>(jsonReq);

            try
            {
                // Below statement returns throws an exception if invalid JSON obj was received

                // setting null pid to a first level class
                if (jsonReq.pid == null)
                    jsonReq.pid = 0;

            } catch(Exception)
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Sent JSON object not valid"
                };
                return Json(jro);
            }

            // Check CID != PID
            if (jsonReq.cid == jsonReq.pid)
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = "Class ID cannot be the same as Parent ID"
                };
                return Json(jro);
            }

            // Custom made Class model validity check
            // Validates manditory attributes
            // Validates CID and Name repototion and PID existance in DB
            string APIValidityCheck = che.APIClassValidityCheck(jsonReq.cid, jsonReq.name, jsonReq.pid);


            if (APIValidityCheck == "")
            {
                db.APIClasses.Add(jsonReq);
                try
                {
                    // Commiting changes to DB
                    db.SaveChanges();
                    return Json("{\"ret\": \"true\"}");
                }
                catch (DbUpdateException)
                {
                    JsonResponseObject jro = new JsonResponseObject
                    {
                        result = false,
                        message = "An error occured while committing changes to database."
                    };
                    return Json(jro);
                }
            }
            else
            {
                JsonResponseObject jro = new JsonResponseObject
                {
                    result = false,
                    message = APIValidityCheck
                };
                return Json(jro);
            }
        }


        /* /cheditor/api/Addclassesjson
         * {"cid": "1","name": "Vehicle","abstract": "true"}
         */

        // [ResponseType(typeof(APIClass))]
        [HttpPost]
        [Route("cheditor/api/addclassesjson")] // Need this because passed variable was changed from id to cid
        public IHttpActionResult Addclassesjson(APIClass jsonReq)
        {
            /*
            string jsonReq = "[{classes:[{ \"cid\": \"1\",\"name\": \"Vehicle\",\"abstract\": \"true\"}," +
                             "{ \"cid\": \"2\",\"name\": \"Vehicle2\",\"pid\": \"1\",\"abstract\": \"true\"}" +
                             "]}]";
             */

            JsonResponseObject jro = new JsonResponseObject
            {
                result = false,
                message = "This service is still under construction!"
            };
            return Json(jro);

            /*
            for(int i = 0; i<classReq.Count(); i++)
            {
                //ADD VALIDATION OF CID, NAME AND PID,
                // COPY THEM FROM ADDCLASS ABOVE


                db.APIClasses.Add(classReq[i]);
            }

            try
            {
                // Commiting changes to DB
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                
            }
            */
        }

        /*
         * 
         * Task 1.b: GetClass
         * 
         *  GET => /cheditor/api/getclass/9
         *  
         *  Returns JSON object
         *  
         * DONE!
         */

        // GET : /cheditor/api/getclass/9
        [ResponseType(typeof(APIClass))]
        public IHttpActionResult GetClass(int id)
        {
            APIClass foundClass = db.APIClasses.Find(id);
            if (foundClass == null)
            {
                JsonResponseObject faliureJsonResult = new JsonResponseObject()
                {
                    result = false,
                    message = "Target class ID does not exist"
                };
                return Json(faliureJsonResult);
            }

            return Json(foundClass); // ORIGINAL: works perfectly
        }

        /*
         * Task 1.c: DeleteClass
         * 
         *  GET => /cheditor/api/deleteclass/6
         *  
         *  TODO:
         *    Add validation:
         *      Target is not a parent.
         *      
         *    DONE!
         */

        // GET : /cheditor/api/deleteclass/4000
        [HttpGet]
        [ResponseType(typeof(APIClass))]
        [Route("cheditor/api/deleteclass/{cid}")] // Need this because passed variable was changed from id to cid
        public IHttpActionResult DeleteClass(int cid)
        {
            APIClass TargetClass = db.APIClasses.Find(cid);

            // Checking if target exists
            if (TargetClass == null)
            {
                JsonResponseObject faliureJsonResult = new JsonResponseObject()
                {
                    result = false,
                    message = "Target class ID does not exist"
                };
                return Json(faliureJsonResult);
            }

            // Checking if is not a parent
            if (che.IsAParent(cid))
            {
                JsonResponseObject faliureJsonResult = new JsonResponseObject()
                {
                    result = false,
                    message = "Target class is a parent, please delete its children first"
                };
                return Json(faliureJsonResult);
            }

            // Remove from context
            db.APIClasses.Remove(TargetClass);
            // Commit changes
            db.SaveChanges();

            return Ok(TargetClass);
        }

        /*
         * Task 1.d: SuperClasses  |  10 Marks
         * 
         *  GET => /cheditor/api/Superclasses/3333
         *  
         *  TODO:
         *    Wrap returned value => '{ list:' returned_Json_value '}'
         */

        [HttpGet]
        [ResponseType(typeof(APIClass))]
        [Route("cheditor/api/superclasses/{cid}")] // Need this because passed variable was changed from id to cid
        public IHttpActionResult Superclasses(int cid)
        //public string Superclasses(int cid)
        {
            //return che.GetSuperClasses(cid);

            // Creat list to store classes
            List<APIClass> list = new List<APIClass>();

            APIClass currentClass = db.APIClasses.Find(cid);

            int i = 1;
            while (i != 0) // Condition is always true, but all paths lead to a 'return' statement eventually!
            {
                if (currentClass == null) // If passed cid does not exist
                {
                    if (list == null) // if list is empty
                    {
                        // create an error response body to be srialized into json
                        JsonResponseObject faliureJsonResult = new JsonResponseObject()
                        {
                            result = false,
                            message = "No parent classes found"
                        };
                        // Serialise and return error response body
                        return Json(faliureJsonResult);
                    }
                    else
                    {
                        // Returning the list of classes
                        return Json(list);
                    }
                }

                // Find parent class where currentClass.pid = cid
                // Find uses passed value to lookup in the primary key field in the DBContext
                APIClass parentClass = db.APIClasses.Find(currentClass.pid);

                // If no parent class found
                if (parentClass == null)//
                {
                    // Return the list of classess as a JSON object
                    return Json(list);
                }
                // Add parent class to list
                list.Add(parentClass);

                // Set parent class as current, in preparation for hte next iteration
                currentClass = parentClass;

            }//END OF WHILE
            // Return the list of classess as a JSON object
            return Json(list); 
        }
    

        /*
         * Task 1.e: SubClasses  |  10 Marks
         * 
         *  GET => /cheditor/api/subclasses/4
         *  
         *  TODO:
         *    Gets the direct and indirect subclasses (all decendants)
         *  
         */
         [HttpGet]
        [Route("cheditor/api/subclasses/{cid}")] // Need this cus passed variable was changed from id to cid
        public IHttpActionResult SubClasses(int? cid)
        {

            if (!db.APIClasses.Any(e => e.cid == cid))
            {
                JsonResponseObject faliureJsonResult = new JsonResponseObject()
                {
                    result = false,
                    message = "Class ID does not exist"
                };
                return Json(faliureJsonResult);
            }

            return Json(che.GetSubClasses(cid));

            /*
            // Create a list
            List<APIClass> list = new List<APIClass>();

            // find the 

            APIClass currentClass = db.APIClasses.Find(cid);
            if (currentClass != null)
            {
                int? parentID = currentClass.pid;

                APIClass parentClass = db.APIClasses.Find(cid);

                if (parentClass == null)
                {
                    if (list == null)
                    {
                        JsonResponseObject faliureJsonResult = new JsonResponseObject()
                        {
                            result = false,
                            message = "No parent classes found"
                        };
                        return Json(faliureJsonResult);
                    }
                    else
                    {
                        return Json(list);
                    }
                }
            }
            list.Add(parentClass);
            SuperClasses((int)parentID);
            */
        }

        /*
         * Gets all classes
         * 
         * Done!
         */

        // GET: api/CHEditor
        public IHttpActionResult GetClasses()
        {
            return Json(db.APIClasses);
        }

        /*
         * Secondary Classes
         */



        /*
         * End of worked-on ones
         * 
         * 
         * 
         */





        /*
         * END OF HANDLED
         * 
         * 
         * 
        // GET: api/Cheditor
        public IQueryable<wt_cw3_class> Getwt_cw3_class()
        {
            return db.wt_cw3_class;
        }

        // GET: api/Cheditor/5
        [ActionName("getclass")]
        [ResponseType(typeof(wt_cw3_class))]
        public IHttpActionResult Getwt_cw3_class(int id)
        {
            wt_cw3_class wt_cw3_class = db.wt_cw3_class.Find(id);
            if (wt_cw3_class == null)
            {
                return NotFound();
            }

            return Ok(wt_cw3_class);
        }

        // PUT: api/Cheditor/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putwt_cw3_class(int id, wt_cw3_class wt_cw3_class)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wt_cw3_class.cid)
            {
                return BadRequest();
            }

            db.Entry(wt_cw3_class).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) // if db update resulted in 0 rows updated
            {
                if (!wt_cw3_classExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Cheditor
        [HttpGet]
        [ResponseType(typeof(wt_cw3_class))]
        public IHttpActionResult Postwt_cw3_class(wt_cw3_class wt_cw3_class)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.wt_cw3_class.Add(wt_cw3_class);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (wt_cw3_classExists(wt_cw3_class.cid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = wt_cw3_class.cid }, wt_cw3_class);
        }

        // DELETE: api/Cheditor/5
        [ResponseType(typeof(wt_cw3_class))]
        public IHttpActionResult Deletewt_cw3_class(int id)
        {
            wt_cw3_class wt_cw3_class = db.wt_cw3_class.Find(id);
            if (wt_cw3_class == null)
            {
                return NotFound();
            }

            db.wt_cw3_class.Remove(wt_cw3_class);
            db.SaveChanges();

            return Ok(wt_cw3_class);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool wt_cw3_classExists(int id)
        {
            return db.wt_cw3_class.Count(e => e.cid == id) > 0;
        }

        //[Route("cheditor/api/addclass")]
        [HttpGet]
        public IHttpActionResult GetAddClass(wt_cw3_class wt_cw3_class)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            if (db.wt_cw3_class.Any(item => item.cid == wt_cw3_class.cid))
            {
                return Conflict();
            }

            db.wt_cw3_class.Add(wt_cw3_class);
            db.SaveChanges();

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (wt_cw3_classExists(wt_cw3_class.cid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = wt_cw3_class.cid }, wt_cw3_class);
        }*/
    }
}
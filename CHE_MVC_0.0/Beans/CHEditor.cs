using CHE_MVC_0._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CHE_MVC_0._0.Beans
{
    public class CHEditor
    {
        private hmaha1DBContextAPI db = new hmaha1DBContextAPI();

        public string APIClassValidityCheck(int? cid, string name, int? pid)
        {
            string errorList = "";
            // Checking if CID and Name are not Nulls
            if (cid == null | name == null)
            {
                errorList += "Class ID and / or Name cannot be null";
            }

            // Checking if CID is available
            if (!IsCidAvailable((int)cid))
            {
                errorList += "Class ID already in use";
            }

            // Checking if name is available
            if (!IsNameAvailable(name))
            {
                if (errorList == "")
                    errorList += "Class name already exists";
                else
                    errorList += ", Class name already exists";
            }

            // Checking if pid exists
            if (!IsParentAvailable((int)pid))
            {
                if (errorList == "")
                    errorList += "Parent class does not exist";
                else
                    errorList += ", Parent class does not exist";
            }


            return errorList;
        }

        public bool ClassExists(int id)
        {
            return db.APIClasses.Count(e => e.cid == id) > 0;
        }

        public bool IsCidAvailable(int cid)
        {
            //  Will return false if CID is not availabe
            return !db.APIClasses.Any(item => item.cid == cid);
        }

        public bool IsNameAvailable(string name)
        {
            // Will return false if Name is not available
            return !db.APIClasses.Any(item => item.name == name);
        }

        public bool IsParentAvailable(int pid)
        {
            // Will return true if PID exists as some class' CID in DB
            return db.APIClasses.Any(item => item.cid == pid);
        }

        public bool IsAParent(int? cid) // will return true if current class ID is listed as a PID for any other class
        {
            // Will return true if class is a parent
            return db.APIClasses.Any(item => item.pid == cid);
        }

        List<APIClass> SubClasses = new List<APIClass>();
        // Serializing object to json data  
        JavaScriptSerializer js = new JavaScriptSerializer();

        /*
         * TODO:
         *   Fix output to match coursework description
         * 
         */
        public List<APIClass> GetSubClasses(int? cid)
        {

            //if(cid == null)
            //{
            //    // in case first passed object is not an actual cid
            //    if (SubClasses == null)
            //    {
            //        // return Json error object: wrong cid
            //    }
            //}
            //else
            //{
                List<APIClass> DirectChildrenlist = new List<APIClass>();
                // list of currnet class' children
                DirectChildrenlist = db.APIClasses.Where(e => e.pid == cid).ToList();

                // In case of no direct children
                if (DirectChildrenlist == null)
                {
                    // will finish this item and go to next one
                }
                else
                {
                    // Iterate through the cildren list
                    foreach (var child in DirectChildrenlist)
                    {
                        // Add currnet child to subclasses list
                        SubClasses.Add(child);

                        // pass child to class
                        GetSubClasses(child.cid);
                    }
                }
            //}

            return SubClasses;
            //return Json(SubClasses);
        }

        /*
            * REPLACED BY ONE IN CONTROLLER => SuperClasses(int cid)
             
        public string GetSuperClasses(int cid)
        {
            // Serializing object to json data  
            JavaScriptSerializer js = new JavaScriptSerializer();

            // Creat list to store classes
            List<APIClass> list = new List<APIClass>();

            APIClass currentClass = db.APIClasses.Find(cid);


            int i = 1;
            while (i != 0)
            {
            if (currentClass == null)
            {
                if (list == null)
                {
                    JsonResponseObject faliureJsonResult = new JsonResponseObject()
                    {
                        result = false,
                        message = "No parent classes found"
                    };

                    //string jsonString;
                    //jsonString = JsonSerializer.Serialize(faliureJsonResult);


                    string jsonData = js.Serialize(faliureJsonResult); // {"Name":"C-sharpcorner","Description":"Share Knowledge"}

                    return jsonData;
                }
                else
                {
                    return js.Serialize(list);
                        // JsonResult json = JsonSerializer.SerializeToUtf8Bytes(list, JsonSerializerOptions options = null);
                    }
            }

            // Find parent class where currentClass.pid = cid
            // Find uses passed value to lookup in the primary key field in the DBContext
            APIClass parentClass = db.APIClasses.Find(currentClass.pid);

            // If no parent class found
            if (parentClass == null)
            {
                // return the list of classess as a JSON object
                return js.Serialize(list);
            }
            // Add parent class to list
            list.Add(parentClass);

            // Call function again but pass the parent class this time
            currentClass = parentClass;

                //END OF WHILE
                //i = 0;
            }
            return js.Serialize(list);
        }
        */

    }
}
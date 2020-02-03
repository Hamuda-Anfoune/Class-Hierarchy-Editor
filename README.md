# Simple-Class-Tree
ASP.Net web app and API for creating and managing and querying a simple Class tree.

NOTES:
----------------------------

1. This project uses a remot SQL Server deployed on Azure.

2. Added a Root class with CID - 0 and PID = Null, because:
  - System checks the new class' PID before saving,
  - It has to match an existing CID,
  - therefore, a Root was needed to have all first level classes as its children.

3. The default output of the api was application/xml:
    the class BrowserJsonFormatter.cs configures default output to application/json
	it is registered and added as a formatter in WebApiConfig.cs.

4. ClassController is for the web GUI functionality

5. CheditorClass is for the API functionality

6. 



***********************************************************
TECHNOLOGIES & TECHNIQUES:
----------------------------
1. Remote JS/AJax (Client-side) model validation:
  - Location:	Models/GUIClassPartial
  - Usage:		input validation before submission
  - Source:		https://www.youtube.com/watch?v=Ll8VtDRj8L4&t=208s

2. Server-side model validation for posted Create() model
  - location:	Controllers/ClassController/Create() => POST
  - Usage:		Check CID!= PID, and check PID exists => in case client side JS is disabled.
  - source:		https://www.youtube.com/watch?v=9BMZkyukz0s

3. Json formatter:
  - Loocation:	~/BrowserJsonFormatter.cs at project main folder
				Register above class at App_Start/WebAPIConfig.cs
  - Usage:		formats all API output into JSON
  - Source:		Stackoverflow.com

4. Optional API Paramerters:
  - Location:	Controllers/cheditorController.cs/AddClass(int? cid, string name, bool Abstract, int? pid = null)
  - Usage:		value for pid can be neglected in the post method
  - Source:		https://stackoverflow.com/a/58639886/11904017

5. Entity Framework



***********************************************************
FUTURE WORK:
----------------------------

1. Optimise model validation for both controllers

2. Enable muliti classes addition using JSON

3. Manipulate JSON responses for API

4. Add a tree view for Index() in ClassController

5. Fix Name validation bug when trying to post to Edit().
